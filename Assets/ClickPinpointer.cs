using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClickPinpointer : MonoBehaviour
{
    private PlayerRequestOrder _pro;

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

            Debug.Log($"{_pro.MoveToClick}");
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        _pro = FindObjectOfType<PlayerRequestOrder>();
    }
}
