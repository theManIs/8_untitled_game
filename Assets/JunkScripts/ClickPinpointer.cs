using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPinpointer : MonoBehaviour
{
    private PlayerRequestOrder _pro;
    private static Transform _squareMarker;
    private StaticMath _sm = new StaticMath();
    private LevelDissectorPlain _ldp;
    private Vector3 _twoThousandth = new Vector3(0, .002f, 0);
    private Vector3 _thisCell = Vector3.zero;

    public void OnMouseDown()
    {
//        MeshRenderer cl = GetComponent<MeshRenderer>();

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitinfo))
        {
            bool moveAndClick = true;
//            Debug.Log(hitinfo.point);
            double x = Math.Ceiling(hitinfo.point.x);
            double z = Math.Ceiling(hitinfo.point.z);

            _pro.MoveToClick = new Vector3((float)(x - .5f), hitinfo.point.y, (float)(z - .5f));
            
//            Debug.Log($"{_pro.MoveToClick}");
            foreach (MainCapsulePlayer mcp in PlayersAccomodation.ListOfPlayers)
            {
                if (mcp.playerSquare.x.Equals(_pro.MoveToClick.x) && mcp.playerSquare.z.Equals(_pro.MoveToClick.z))
                {
                    mcp.OnMouseDown();
                    moveAndClick = false;
                }
            }

            if (moveAndClick)
            {
                _pro.NewMove = true;
            }
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
//                float x = (float)(Math.Ceiling(hitinfo2.point.x) - .5f);
//                float z = (float)(Math.Ceiling(hitinfo2.point.z) - .5f);
//                _squareMarker.position = new Vector3(x, hitinfo2.point.y + .002f, z);

//                Vector3 squareMarker = hitinfo2.point;
                //                squareMarker.y = Mathf.Round(squareMarker.y * 100) / 100f;
                Vector3 squareMarker = _sm.PointToCellCenterRounded(hitinfo2.point, new Vector3(.5f, 0, .5f));
//                Debug.Log(squareMarker.y + " " + squareMarker);

                _thisCell.Set((float)Math.Ceiling(hitinfo2.point.x), 0, (float)Math.Ceiling(hitinfo2.point.z));

                foreach (Vector3 v3 in _ldp.AllCellsInOneArray)
                {
                    if (v3.x.Equals(_thisCell.x) && v3.z.Equals(_thisCell.z))
                    {
                        squareMarker.y = v3.y;
//                        Debug.Log(squareMarker.y + " " + _thisCell);
                    }
                }

//                squareMarker.y += .002f;
                _squareMarker.position = squareMarker + _twoThousandth;

                _squareMarker.GetComponent<MeshRenderer>().material.color = Color.green;
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

        _ldp = FindObjectOfType<LevelDissectorPlain>();
        _pro = FindObjectOfType<PlayerRequestOrder>();
    }
}
