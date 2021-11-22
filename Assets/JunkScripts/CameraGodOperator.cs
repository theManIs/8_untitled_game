using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGodOperator : MonoBehaviour
{
    public Color ColorAutoFocus = Color.blue;
    public float LockDistanceZ = 3f;
    public float LockDistanceY = 2f;
    public float Speed = .1f;
    public Vector4 CameraClamp = new Vector4(0,10, -4, 6);
    public LevelDissectorPlain Ldap;
    public int BoundsOffset = 3;

    public Transform MainCamera;

    public bool ReverseHorizontal = false;
    public bool ReverseVertical = false;
    public bool ReverseAxis = false;

    private InputGodTransporter _input;
    private bool _autoLock = false;

    // Start is called before the first frame update
    void Start()
    {
        _input = FindObjectOfType<InputGodTransporter>();
    }

    public void OnInnateTraitsDiscovered()
    {
        if (!_autoLock)
        {
            foreach (MainCapsulePlayer mcp in PlayersAccomodation.ListOfPlayers)
            {
                if (!_autoLock && mcp.InnateTraits != null && mcp.InnateTraits.BaseColor == ColorAutoFocus)
                {
                    _autoLock = true;

                    Vector3 cameraLockOn = MainCamera.position;

                    cameraLockOn.x = mcp.transform.position.x;
                    cameraLockOn.z = mcp.transform.position.z - LockDistanceZ;
                    cameraLockOn.y = mcp.transform.position.y + LockDistanceY;

                    MainCamera.position = cameraLockOn;
//                    Debug.Log(Vector3.Distance(mcp.transform.position, MainCamera.position) + " " + mcp.transform.position);
//
//                    Vector3 v3 = transform.localPosition;
//                    v3.z += 40;
//                    transform.localPosition = v3;

//                    Debug.Log(Vector3.Distance(mcp.playerSquare, transform.position));
                }
            }

            CameraClamp.x = Ldap.MinX - BoundsOffset;
            CameraClamp.y = Ldap.MaxX - BoundsOffset;
            CameraClamp.z = Ldap.MinZ - BoundsOffset;
            CameraClamp.w = Ldap.MaxZ - BoundsOffset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        OnInnateTraitsDiscovered();

        if (!_input.Hr.Equals(0.0f) || !_input.Vr.Equals(0.0f))
        {
            Vector3 newPos;
            int axisOrientation = ReverseHorizontal ? -1 : 1;
            int verticalOrientation = ReverseVertical ? -1 : 1;

            if (ReverseAxis)
            {
                newPos = MainCamera.position + new Vector3(_input.Vr * Speed * axisOrientation, 0, _input.Hr * Speed * verticalOrientation);
            }
            else
            {
                newPos = MainCamera.position + new Vector3(_input.Hr * Speed * axisOrientation, 0, _input.Vr * Speed * verticalOrientation);
            }

            newPos.x = Mathf.Clamp(newPos.x, CameraClamp.x, CameraClamp.y);
            newPos.z = Mathf.Clamp(newPos.z, CameraClamp.z, CameraClamp.w);

            MainCamera.position = newPos;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, 90);

            if (transform.rotation.eulerAngles.y == 90)
            {
                ReverseAxis = true;
                ReverseVertical = true;
                ReverseHorizontal = false;
            }

            if (transform.rotation.eulerAngles.y == 180)
            {
                ReverseAxis = false;
                ReverseVertical = true;
                ReverseHorizontal = true;
            }

            if (transform.rotation.eulerAngles.y == 270)
            {
                ReverseAxis = true;
                ReverseVertical = false;
                ReverseHorizontal = true;
            }

            if (transform.rotation.eulerAngles.y == 0)
            {
                ReverseAxis = false;
                ReverseVertical = false;
                ReverseHorizontal = false;
            }

            CameraClampOnDemand(transform.position);
        }
    }

    public void CameraClampOnDemand(Vector3 newPos)
    {
        newPos.x = Mathf.Clamp(newPos.x, CameraClamp.x, CameraClamp.y);
        newPos.z = Mathf.Clamp(newPos.z, CameraClamp.z, CameraClamp.w);

        MainCamera.position = newPos;

    }

}
