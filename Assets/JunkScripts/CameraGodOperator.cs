using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGodOperator : MonoBehaviour
{
    public Color ColorAutoFocus = Color.blue;
    public float LockDistanceZ = 3f;
    public float Speed = .2f;
    public Vector4 CameraClamp = new Vector4(0,10, -4, 6);
    public Vector4 CameraRotationSet0 = new Vector4(1, 1, 1, 1);
    public LevelDissectorPlain Ldap;
    public int BoundsOffset = 3;

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
                if (mcp.InnateTraits != null && mcp.InnateTraits.BaseColor == ColorAutoFocus)
                {
                    _autoLock = true;

                    Vector3 cameraLockOn = transform.position;

                    cameraLockOn.x = mcp.transform.position.x;
                    cameraLockOn.z = mcp.transform.position.z - LockDistanceZ;

                    transform.position = cameraLockOn;
//                    Debug.Log(cameraLockOn);    
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
                newPos = transform.position + new Vector3(_input.Vr * Speed * axisOrientation, 0, _input.Hr * Speed * verticalOrientation);
            }
            else
            {
                newPos = transform.position + new Vector3(_input.Hr * Speed * axisOrientation, 0, _input.Vr * Speed * verticalOrientation);
            }

            newPos.x = Mathf.Clamp(newPos.x, CameraClamp.x, CameraClamp.y);
            newPos.z = Mathf.Clamp(newPos.z, CameraClamp.z, CameraClamp.w);

            transform.position = newPos;
        }
    }

    public void CameraClampOnDemand(Vector3 newPos)
    {
        newPos.x = Mathf.Clamp(newPos.x, CameraClamp.x, CameraClamp.y);
        newPos.z = Mathf.Clamp(newPos.z, CameraClamp.z, CameraClamp.w);

        transform.position = newPos;

    }
}
