using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CamerRotationIniter : MonoBehaviour
{
    public CameraGodOperator CameraGodOperator;
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, 90);

            if (transform.rotation.eulerAngles.y == 90)
            {
                CameraGodOperator.ReverseAxis = true;
                CameraGodOperator.ReverseVertical = true;
                CameraGodOperator.ReverseHorizontal = false;
            }

            if (transform.rotation.eulerAngles.y == 180)
            {
                CameraGodOperator.ReverseAxis = false;
                CameraGodOperator.ReverseVertical = true;
                CameraGodOperator.ReverseHorizontal = true;
            }

            if (transform.rotation.eulerAngles.y == 270)
            {
                CameraGodOperator.ReverseAxis = true;
                CameraGodOperator.ReverseVertical = false;
                CameraGodOperator.ReverseHorizontal = true;
            }

            if (transform.rotation.eulerAngles.y == 0)
            {
                CameraGodOperator.ReverseAxis = false;
                CameraGodOperator.ReverseVertical = false;
                CameraGodOperator.ReverseHorizontal = false;
            }

            CameraGodOperator.CameraClampOnDemand(CameraGodOperator.transform.position);
        }
    }
}
