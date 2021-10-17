using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGodOperator : MonoBehaviour
{
    public float Speed = .2f;
    public Vector4 CameraClamp = new Vector4(0,10, -4, 6);
    public Vector4 CameraRotationSet0 = new Vector4(1, 1, 1, 1);

    public bool ReverseHorizontal = false;
    public bool ReverseVertical = false;
    public bool ReverseAxis = false;


    private InputGodTransporter _input;

    // Start is called before the first frame update
    void Start()
    {
        _input = FindObjectOfType<InputGodTransporter>();
    }

    // Update is called once per frame
    void Update()
    {
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
