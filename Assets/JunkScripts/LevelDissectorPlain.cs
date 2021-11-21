using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class LevelDissectorPlain : MonoBehaviour
{
    [Header("User Set")]
    public Vector3 OneCell = new Vector3(1, 0, 1);
    public Vector3 OneCellExtents = new Vector3(.5f, 0, .5f);
    public Vector3 OneCellVerticalRayBar = new Vector3(0, 100, 0);
    public float MaxStepHeight = .5f;
    public float MaxStepDistance = 1;
    public float RangedWeaponDistance = 5;

    [Header("Auto Set")]
    public float MinX = 0;
    public float MinZ = 0;
    public float MaxX = 0;
    public float MaxZ = 0;
    public float MapSizeX;
    public float MapSizeZ;
    public Vector3 LevelBounds;
    public Vector3 LevelMins;
    public Vector3 LevelMaxs;
    public Vector3[] AllCellsInOneArray;

    private Bounds _mapBounds;
    private StaticMath _sMath = new StaticMath();
//    private ConstantConstraints _cc;

    void OnEnable()
    {
//        _cc = FindObjectOfType<ConstantConstraints>();

//        AllCellsInOneArray = GetLevelDissected(_cc.LevelMins,_cc.LevelBounds);

        MeshRenderer mr = GetComponent<MeshRenderer>();
        _mapBounds = mr.bounds;

        MinX = mr.bounds.min.x + OneCell.x;
        MaxX = mr.bounds.max.x + OneCell.x;
        MinZ = mr.bounds.min.z + OneCell.z;
        MaxZ = mr.bounds.max.z + OneCell.z;
        MapSizeX = mr.bounds.size.x;
        MapSizeZ = mr.bounds.size.z;
        LevelBounds = _mapBounds.size;
        LevelMins = _mapBounds.min;
        LevelMins.x += OneCell.x;
        LevelMins.z += OneCell.z;
        LevelMaxs = _mapBounds.max;
        LevelMaxs.x += OneCell.x;
        LevelMaxs.z += OneCell.z;

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
