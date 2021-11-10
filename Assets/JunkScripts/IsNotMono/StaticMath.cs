using System.Collections.Generic;
using UnityEngine;

public class StaticMath
{
    public int GetRelativeQuarter(Vector3 a, Vector3 b)
    {
        if (a.x > b.x && a.z > b.z)
        {
            return 1;
        }
        else if (a.x < b.x && a.z > b.z)
        {
            return 2;
        } 
        else if (a.x < b.x && a.z < b.z)
        {
            return 3;
        } 
        else if (a.x > b.x && a.z < b.z)
        {
            return 4;
        } 

        return 0;
    }

    public float Max(float a, float b)
    {
        return a > b ? a : b;
    }

    public float Min(float a, float b)
    {
        return a > b ? b : a;
    }

    public float DiffMaxMin(float a, float b)
    {
        return Max(a, b) - Min(a, b);
    }

    public Vector3 PointToCellCenterXZ(Vector3 somePoint, Vector3 extents)
    {
        return new Vector3(Mathf.Ceil(somePoint.x) - extents.x, somePoint.y, Mathf.Ceil(somePoint.z) - extents.z);
    }
    public Vector3 PointToCellCenter(Vector3 somePoint, Vector3 extents)
    {
        return new Vector3(Mathf.Ceil(somePoint.x) - extents.x, HeightToHalfRoundFloat(somePoint.y) - extents.y, Mathf.Ceil(somePoint.z) - extents.z);
    }

    public Vector3 CellCenterToPointXZ(Vector3 somePoint)
    {
        return new Vector3(Mathf.Ceil(somePoint.x), somePoint.y, Mathf.Ceil(somePoint.z));
    }
    public List<Vector3> CellCenterToPointXZ(List<Vector3> somePoint)
    {
        List<Vector3> list = new List<Vector3>();

        foreach (Vector3 vector3 in somePoint)
        {
            list.Add(CellCenterToPointXZ(vector3));
        }

        return list;
    }

    public Vector3 HeightToHalfRound(Vector3 height)
    {
//        Debug.Log(height.y + " " + height.y % 1f + " " + (height.y % 1f > .5f));
        height.y = height.y.Equals(0.0f) ? 0 : (Mathf.Abs(height.y % 1f) <= .5f ? Mathf.Ceil(height.y) - .5f : Mathf.Ceil(height.y));
//        Debug.Log(height.y);
        return height;
    }

    public float HeightToHalfRoundFloat(float height)
    {
        return height.Equals(0.0f) ? 0 : (Mathf.Abs(height % 1f) <= .5f && Mathf.Abs(height % 1f) > 0 ? Mathf.Ceil(height) - .5f : Mathf.Ceil(height));
    }

    public float GetTriangleArea(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        return Mathf.Abs(v1.x * (v2.z - v3.z) + v2.x * (v3.z - v1.z) + v3.x * (v1.z - v2.z)) / 2;
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

    public static void ShowHashSet(List<Vector3> h3)
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
