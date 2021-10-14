using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGodOperator : MonoBehaviour
{
    public float Speed = .2f;
    public Vector4 CameraClamp = new Vector4(0,10, -4, 6);

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
            Vector3 newPos = transform.position + new Vector3(_input.Hr * Speed, 0, _input.Vr * Speed);

            newPos.x = Mathf.Clamp(newPos.x, CameraClamp.x, CameraClamp.y);
            newPos.z = Mathf.Clamp(newPos.z, CameraClamp.z, CameraClamp.w);

            transform.position = newPos;
        }
    }
}
