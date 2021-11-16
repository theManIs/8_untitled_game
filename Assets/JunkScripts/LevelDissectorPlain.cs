using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDissectorPlain : MonoBehaviour
{
    public Transform MainGridBox;
    public Vector3 GridBounds = Vector3.zero;
    public Vector3[] AllCellsInOneArray;

    private StaticMath _sMath = new StaticMath();
    private ConstantConstraints _cc;

    void OnEnable()
    {
        _cc = FindObjectOfType<ConstantConstraints>();

//        AllCellsInOneArray = GetLevelDissected(_cc.LevelMins,_cc.LevelBounds);
    }

    public Vector3[] GetLevelDissected(Vector3 levelMins, Vector3 levelBounds)
    {
        if (AllCellsInOneArray.Length > 0)
        {
            return AllCellsInOneArray;
        }

        AllCellsInOneArray = DissectLevel(levelMins, levelBounds);

        return AllCellsInOneArray;
    }

    private Vector3[] DissectLevel(Vector3 levelMins, Vector3 levelBounds)
    {
        List<Vector3> lv3 = new List<Vector3>();

        for (int x = 0; x < levelBounds.x; x++)
        {
            for (int z = 0; z < levelBounds.z; z++)
            {
                Vector3 currentTile = new Vector3(x + levelMins.x, levelMins.y, z + levelMins.z);
                Vector3 raySource = currentTile + new Vector3(0, 100, 0) - new Vector3(.5f, 0, .5f);

                if (Physics.Raycast(new Ray(raySource, Vector3.down), out RaycastHit hitinfo, 1000))
                {

                    currentTile.y = _sMath.RoundByThousand(hitinfo.point.y);

//                    if (currentTile.x.Equals(5f) && currentTile.z.Equals(-3f))
//                    {
//                        Debug.Log(currentTile.y + " " + _sMath.HeightToHalfRound(currentTile).y + " " + (currentTile.y % 1f));
//                        Debug.Break();
//                    }

                    lv3.Add(_sMath.HeightToHalfRound(currentTile));
//                    lv3.Add(currentTile);
                }
                else
                {
//                    lv3.Add(currentTile);
                }
            }
        }

        return lv3.ToArray();
    }
}
