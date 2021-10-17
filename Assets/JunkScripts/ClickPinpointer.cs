using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClickPinpointer : MonoBehaviour
{
    private PlayerRequestOrder _pro;
    private static Transform _squareMarker;
    private bool _mouseOver = true;

    public void OnMouseDown()
    {

        MeshRenderer cl = GetComponent<MeshRenderer>();

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitinfo))
        {


//            Debug.Log(hitinfo.point);

//
            double x = Math.Ceiling(hitinfo.point.x);
            double z = Math.Ceiling(hitinfo.point.z);

            _pro.MoveToClick = new Vector3((float)(x - .5f), hitinfo.point.y, (float)(z - .5f));
            _pro.NewMove = true;

//            Debug.Log($"{_pro.MoveToClick}");
        }

    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitinfo2, 100, ~LayerMask.GetMask("Playable")))
//        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitinfo2, 100))
        {
            if (hitinfo2.point.z > -18)
            {
                float x = (float)(Math.Ceiling(hitinfo2.point.x) - .5f);
                float z = (float)(Math.Ceiling(hitinfo2.point.z) - .5f);

                _squareMarker.position = new Vector3(x, hitinfo2.point.y + .002f, z);
                _squareMarker.GetComponent<MeshRenderer>().material.color = Color.green;

//                    Debug.Log($"{hitinfo2.point}");
            }
        }
        
    }

    public void OnEnable()
    {
        if (_squareMarker is null)
        {
            Transform originalSquareMarker = FindObjectOfType<SquareInstance>().transform;
            _squareMarker = Instantiate(originalSquareMarker, Vector3.zero, originalSquareMarker.rotation);
        }

        _pro = FindObjectOfType<PlayerRequestOrder>();
    }
}
