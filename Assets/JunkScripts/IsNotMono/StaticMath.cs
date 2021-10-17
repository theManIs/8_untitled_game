using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMath
{
    public Vector3 PointToCellCenterXZ(Vector3 somePoint, Vector3 extents)
    {
        return new Vector3(Mathf.Ceil(somePoint.x) - extents.x, somePoint.y, Mathf.Ceil(somePoint.z) - extents.z);
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
