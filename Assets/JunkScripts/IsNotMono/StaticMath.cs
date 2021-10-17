using System.Collections;
using UnityEngine;

public class StaticMath
{
    public Vector3 PointToCellCenterXZ(Vector3 somePoint, Vector3 extents)
    {
        return new Vector3(Mathf.Ceil(somePoint.x) - extents.x, somePoint.y, Mathf.Ceil(somePoint.z) - extents.z);
    }
}
