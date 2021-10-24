using System.Collections.Generic;
using UnityEngine;

public class StaticMath
{
    public Vector3 PointToCellCenterXZ(Vector3 somePoint, Vector3 extents)
    {
        return new Vector3(Mathf.Ceil(somePoint.x) - extents.x, somePoint.y, Mathf.Ceil(somePoint.z) - extents.z);
    }
    public Vector3 PointToCellCenter(Vector3 somePoint, Vector3 extents)
    {
        return new Vector3(Mathf.Ceil(somePoint.x) - extents.x, somePoint.y - extents.y, Mathf.Ceil(somePoint.z) - extents.z);
    }

    public Vector3 CellCenterToPointXZ(Vector3 somePoint)
    {
        return new Vector3(Mathf.Ceil(somePoint.x), somePoint.y, Mathf.Ceil(somePoint.z));
    }

    public Vector3 HeightToHalfRound(Vector3 height)
    {
//        Debug.Log(height.y + " " + height.y % 1f + " " + (height.y % 1f > .5f));
        height.y = height.y.Equals(0.0f) ? 0 : (Mathf.Abs(height.y % 1f) <= .5f ? Mathf.Ceil(height.y) - .5f : Mathf.Ceil(height.y));
//        Debug.Log(height.y);
        return height;
    }

    public static void ShowHashSet(HashSet<Vector3> h3)
    {
        string debubMessage = "";
        int iterator = 0;

        foreach (Vector3 vector3 in h3)
        {
            debubMessage += $"{iterator} {vector3} \n";
            iterator++;
        }

        Debug.Log(debubMessage);
    }
}
