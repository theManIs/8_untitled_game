using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AccuracyCounter
{
    public Vector3[] Cells;

    private StaticMath sm = new StaticMath();
    private float shift = 1;
    private float coef = 1;
    private int xSign = -1;
    private readonly Vector3 _extent = new Vector3(.5f, 0, .5f);
    private readonly float _maxObstacleImpact = 1f;
    private readonly float _triangleAreaMultiplier = 2f;
    private HashSet<Vector3[]> _tangency = new HashSet<Vector3[]>();

    private void RecountEquation(Vector3 sourcePoint, Vector3 targetPoint)
    {
//        xSign = targetPoint.x > sourcePoint.x ? 1 : -1;
        int quarter = sm.GetRelativeQuarter(sourcePoint, targetPoint);
        xSign = quarter == 1 || quarter == 3 ? 1 : -1;

//        shift = xSign == 1 ? sourcePoint.z - sourcePoint.x : sourcePoint.z + sourcePoint.x;
//        shift = 7f;
//        float xProjection = Math.Abs(sourcePoint.x - targetPoint.x);
        float xProjection = sourcePoint.x > targetPoint.x ? sourcePoint.x - targetPoint.x : targetPoint.x - sourcePoint.x;
//        float zProjection = Math.Abs(sourcePoint.z - targetPoint.z);
        float zProjection = sourcePoint.z > targetPoint.z ? sourcePoint.z - targetPoint.z : targetPoint.z - sourcePoint.z;
        coef = xProjection == 0 || zProjection == 0 ? 0 : zProjection / xProjection;
//        coef = 1;
        shift = ScoreShift(sourcePoint.x, sourcePoint.z);
//        Debug.Log(xSign + " " + shift + " " + sourcePoint + " " + targetPoint + " coef:" + coef + " " + xProjection + " " + zProjection);
    }

    private Vector3 ScoreStraightZ(Vector3 xPoint)
    {
        return new Vector3(xPoint.x, xPoint.y, (int)coef == 0 ? xPoint.z : ScoreSzf(xPoint.x));
    }

    private float ScoreSzf(float x)
    {
        return x * xSign * coef + shift;
    }

    private float ScoreSzf(float x, float z)
    {
        return coef.Equals(0) ? z : ScoreSzf(x);
    }
    private float ScoreSxf(float z)
    {
        return (float)(z - shift) / (float)(xSign * coef);
    }

    private float ScoreSxf(float z, float x)
    {
        return coef.Equals(0) ? x : ScoreSxf(z);
    }

    private float ScoreShift(float x, float z)
    {
        return z - x * xSign * coef;
    }

    private float SetImpactEquation(float xMulti, float triangleArea, float elevationDifference)
    {
//        Debug.Log(triangleArea);  
        return xMulti.Equals(1f) || xMulti.Equals(0f) ? _maxObstacleImpact : Mathf.Clamp01(_maxObstacleImpact * triangleArea + _maxObstacleImpact / 2.0f);
    }

    public List<Vector3> GetIntersection(Vector3 sourcePoint, Vector3 targetPoint)
    {

//        HashSet<Vector3[]> tangency = new HashSet<Vector3[]>();
        HashSet<Vector3> intersectionList = new HashSet<Vector3>();

//        float mX = sourcePoint.x < targetPoint.x ? sourcePoint.x : targetPoint.x;
        float baseX = sm.Min(sourcePoint.x, targetPoint.x);
//        float maX = sourcePoint.x > targetPoint.x ? sourcePoint.x : targetPoint.x;
//        int xI = Convert.ToInt32(sm.DiffMaxMin(sourcePoint.x, targetPoint.x));

        for (int xi = 0; xi <= Convert.ToInt32(sm.DiffMaxMin(sourcePoint.x, targetPoint.x)); xi++)
        {
//            float baseZ = sourcePoint.z < targetPoint.z ? sourcePoint.z : targetPoint.z;
            float baseZ = sm.Min(sourcePoint.z,targetPoint.z);
//            float maZ = sourcePoint.z > targetPoint.z ? sourcePoint.z : targetPoint.z;
//            int zI = Convert.ToInt32(maZ - baseZ);

            for (int zi = 0; zi <= Convert.ToInt32(sm.DiffMaxMin(sourcePoint.z, targetPoint.z)); zi++)
            {
//                Debug.Log(new Vector3(mX, 0, mZ));
                Vector3[] plane =
                {
                    new Vector3(xi + baseX, 0, zi + baseZ),
                    new Vector3(xi + baseX, 0, zi + baseZ - 1), 
                    new Vector3(xi + baseX - 1, 0, zi + baseZ - 1),
                    new Vector3(xi + baseX - 1, 0, zi + baseZ),
                    new Vector3(xi + baseX, 0, zi + baseZ), 
                };

                for (int i = 0; i < plane.Length - 1; i++)
                {
                    Debug.DrawLine(plane[i], plane[i + 1], Color.blue);

                    if (i == 1 || i == 3)
                    {
                        float touchPin = ScoreSxf(plane[i].z, plane[i].x);
//                        Debug.Log(plane[i].x + " > " + touchPin + " > " + plane[i + 1].x + " x * xSign * coef + shift " + plane[i].x + " * " + xSign + " * " + coef + " + " + shift);
                        if (touchPin >= plane[i].x && touchPin <= plane[i + 1].x || touchPin >= plane[i + 1].x && touchPin <= plane[i].x)
                        {
                            intersectionList.Add(plane[0]);
                            _tangency.Add(new Vector3[2]{ new Vector3(touchPin, 0, plane[i].z), plane[0]});
                        }
                    }
                    
                    if (i == 0 || i == 2)
                    {
                        float touchPin = ScoreSzf(plane[i].x, plane[i].z);
//                        Debug.Log(plane[i].z + " > " + touchPin + " > " + plane[i + 1].z + " x * xSign * coef + shift " + plane[i].x + " * " + xSign + " * " + coef + " + " + shift);
                        if (touchPin >= plane[i].z && touchPin <= plane[i + 1].z || touchPin >= plane[i + 1].z && touchPin <= plane[i].z)
                        {
                            intersectionList.Add(plane[0]);
                            _tangency.Add(new Vector3[2] { new Vector3(plane[i].x, 0, touchPin), plane[0] });
                        }
                    }
                }

//                if (ScoreSzf(plane[0].x, plane[0].z) >= plane[0].z && ScoreSzf(plane[0].x, plane[0].z) <= plane[1].z
//                    || ScoreSzf(plane[0].x, plane[0].z) >= plane[1].z && ScoreSzf(plane[0].x, plane[0].z) <= plane[0].z)
//                {
//                    intersectionList.Add(new Vector3(plane[0].x, 0, plane[0].z));
//                    tangency.Add(new Vector3(plane[0].x, 0, ScoreSzf(plane[0].x, plane[0].z)));
//                }

//                float touchPin = ScoreSxf(plane[1].z, plane[1].x);
////                Debug.Log(plane[1].x + " > " + touchPin + " > " + plane[2].x + " coef:" + coef);
//                if (touchPin >= plane[1].x && touchPin <= plane[2].x || touchPin >= plane[2].x && touchPin <= plane[1].x)
//                {
//                    intersectionList.Add(plane[1]);
//                    tangency.Add(new Vector3(touchPin, 0, plane[1].z));
//                }

//                if (ScoreSzf(plane[2].x, plane[2].z) >= plane[2].z && ScoreSzf(plane[2].x, plane[2].z) <= plane[3].z
//                    || ScoreSzf(plane[2].x, plane[2].z) >= plane[3].z && ScoreSzf(plane[2].x, plane[2].z) <= plane[0].z)
//                {
//                    intersectionList.Add(new Vector3(plane[2].x, 0, plane[2].z));
//                    tangency.Add(new Vector3(plane[2].x, 0, ScoreSzf(plane[2].x, plane[2].z)));
//                }
////
//                touchPin = ScoreSxf(plane[3].z, plane[3].x);
////                Debug.Log(plane[3].x + " > " + touchPin + " > " + plane[4].x + " shift:" + shift);
//                if (touchPin >= plane[3].x && touchPin <= plane[4].x || touchPin >= plane[4].x && touchPin <= plane[3].x)
//                {
//                    intersectionList.Add(plane[3]);
//                    tangency.Add(new Vector3(touchPin, 0, plane[3].z));
//                }
            }
        }
        
//        Debug.Log(BugHell.ShowV3(_tangency));
        return intersectionList.ToList();
    }

    public List<Vector3> StripClosestCells(Vector3 sourcePoint, Vector3 targetPoint, List<Vector3> obstaclesList)
    {
//        Debug.Log(BugHell.ShowV3(obstaclesList));
        return obstaclesList.Where(item =>
        {
            if (item.x == sourcePoint.x && item.z == sourcePoint.z + 1)
            {
//                Debug.Log(sourcePoint);
                return false;
            }
            else if (item.x == sourcePoint.x && item.z == sourcePoint.z - 1)
            {
//                Debug.Log(sourcePoint);
                return false;
            }
            else if (item.x == sourcePoint.x + 1 && item.z == sourcePoint.z)
            {
//                Debug.Log(sourcePoint);
                return false;
            }
            else if (item.x == sourcePoint.x - 1 && item.z == sourcePoint.z)
            {
//                Debug.Log(sourcePoint);
                return false;
            }
            else if (item.x == sourcePoint.x && item.z == sourcePoint.z)
            {
//                Debug.Log(sourcePoint);
                return false;
            }
            else if (item.x == targetPoint.x && item.z == targetPoint.z)
            {
//                Debug.Log(targetPoint);
                return false;
            }
            else
            {
                return true;
            }
        }).ToList();
    }

    public float SumObstacles(Vector3 sourcePoint, List<Vector3[]> obstaclesList)
    {
        float finiteElevation = 0;

        foreach (Vector3[] obstacleCell in obstaclesList)
        {
            Vector3 checkCell = Cells.First(item => item.x.Equals(obstacleCell[1].x) && item.z.Equals(obstacleCell[1].z));

            float elevationDifference = sm.HeightToHalfRoundFloat(sm.HeightToHalfRoundFloat(checkCell.y) - sm.HeightToHalfRoundFloat(sourcePoint.y));
            float possibleImpact = SetImpactEquation(obstacleCell[0].y, obstacleCell[0].x, elevationDifference);
            float halfImpact = possibleImpact / 2;
//            Debug.Log("possibleImpact:" + possibleImpact + "checkCell " + checkCell + "\nelevationDifference " + elevationDifference + "\nsourcePoint.y " + sourcePoint.y + "\ncheckCell.y " + checkCell.y + "\nsourcePoint.y - checkCell.y " + (sourcePoint.y - checkCell.y));
            if (elevationDifference >= 1 && possibleImpact > finiteElevation)
            {
                finiteElevation = possibleImpact;
            }
            else if (elevationDifference < 1 && elevationDifference > 0 && halfImpact > finiteElevation)
            {
                finiteElevation = halfImpact;
            }
        }

        return finiteElevation;
    }

    public Vector3[] GetCell(Vector3 v)
    {
        return new Vector3[] {
            new Vector3(v.x, 0, v.z),
            new Vector3(v.x, 0, v.z - 1),
            new Vector3(v.x - 1, 0, v.z - 1),
            new Vector3(v.x - 1, 0, v.z),
            new Vector3(v.x, 0, v.z),
        };
    }

    private List<Vector3[]> ScoreImpact(List<Vector3> obstaclesList )
    {
        HashSet<Vector3[]> localTangency = new HashSet<Vector3[]>();
        HashSet<Vector3[]> triangles = new HashSet<Vector3[]>();


        foreach (Vector3 vector3Se in obstaclesList)
        {
            Vector3[] startTriangle = new Vector3[5] { new Vector3(0, coef, 0), vector3Se, Vector3.zero,  Vector3.zero, Vector3.zero, };

            foreach (Vector3[] tangencyOne in _tangency)
            {
                if (vector3Se.x.Equals(tangencyOne[1].x) && vector3Se.z.Equals(tangencyOne[1].z))
                {
                    if (startTriangle[2] == Vector3.zero || startTriangle[2] == tangencyOne[0])
                    {
                        startTriangle[2] = tangencyOne[0];
                    }
                    else if (startTriangle[3] == Vector3.zero || startTriangle[3] == tangencyOne[0])
                    {
                        startTriangle[3] = tangencyOne[0];
                    }
                }
            }

            startTriangle[4].x = startTriangle[2].x % 1f == 0 ? startTriangle[2].x : startTriangle[3].x;
            startTriangle[4].z = startTriangle[2].z % 1f == 0 ? startTriangle[2].z : startTriangle[3].z;

            triangles.Add(startTriangle);
        }

        foreach (Vector3[] v3vecs in triangles)
        {
            foreach (Vector3 eachGet in GetCell(v3vecs[1]))
            {
                if (eachGet.x.Equals(v3vecs[4].x) && eachGet.z.Equals(v3vecs[4].z))
                {
                    v3vecs[0].z = 1;
                    v3vecs[0].x = sm.GetTriangleArea(v3vecs[2], v3vecs[3], v3vecs[4]);
//                    Debug.Log(v3vecs[1] + " "  + sm.GetTriangleArea(v3vecs[2], v3vecs[3], v3vecs[4]));
                }
            }

            if (v3vecs[0].z.Equals(0))
            {
                v3vecs[0].x = 1;
//                float a = 1 * (sm.Min(v3vecs[2].x - Mathf.Floor(v3vecs[2].x), v3vecs[2].x - Mathf.Ceil(v3vecs[2].x)) + sm.Min(v3vecs[3].x - Mathf.Floor(v3vecs[3].x), v3vecs[3].x - Mathf.Ceil(v3vecs[3].x)));
//                Debug.Log(v3vecs[2].z + " " + Mathf.Floor(v3vecs[2].z));
//                Debug.Log(v3vecs[1] + " " + sm.Min(v3vecs[2].x - Mathf.Floor(v3vecs[2].x), v3vecs[2].x - Mathf.Ceil(v3vecs[2].x)) + " " + sm.Min(v3vecs[3].x - Mathf.Floor(v3vecs[3].x), v3vecs[3].x - Mathf.Ceil(v3vecs[3].x)) + " " + a);
            }
        }

//        foreach (Vector3[] vectorArray in _tangency)
//        {
//            Vector3 obstacleCell = vectorArray[1];
//            Vector3 v1 = Vector3.zero;
//            Vector3 v2 = Vector3.zero;
//            List<Vector3[]> secondVectorList = _tangency.Where(item => item[1].x == obstacleCell.x && item[1].z == obstacleCell.z).ToList();
//            Vector3 v3 = new Vector3();
//
//            foreach (Vector3[] secV in secondVectorList)
//            {
//                if (secV[0].x == v1.x && secV[0].z == v1.z || secV[0].x == v2.x && secV[0].z == v2.z)
//                {
//                    // do nothing
//                }
//                else
//                {
//                    if (v1 == Vector3.zero)
//                    {
//                        v1 = secV[0];
//                    }
//                    else if (v2 == Vector3.zero)
//                    {
//                        v2 = secV[0];
//                    }
//
//                    v3.x = secV[0].x % 1f == 0 ? secV[0].x : v3.x;
//                    v3.z = secV[0].z % 1f == 0 ? secV[0].z : v3.z;
//                }
//            }
//
//            if (obstaclesList.Contains(obstacleCell))
//            {
//                if (localTangency.Count(item => item[1].x.Equals(obstacleCell.x) && item[1].z.Equals(obstacleCell.z)) == 0)
//                {
//                    float triangleArea = Mathf.Abs(v1.x * (v2.z - v3.z) + v2.x * (v3.z - v1.z) + v3.x * (v1.z - v2.z)) / 2;
////                    Debug.Log(v1.x * (v2.z - v3.z) + " " + v2.x * (v3.z - v1.z) + " " + v3.x * (v1.z - v2.z));
////                    Debug.Log(v1 + " " + v2 + " " + v3);
//                    localTangency.Add(new Vector3[5] { new Vector3(triangleArea, coef, 0), obstacleCell, v1, v2, v3});
//                }
//            }
//            
//        }
//        Debug.Log(BugHell.ShowV3(obstaclesList));
//        Debug.Log(BugHell.ShowV3(_tangency));
//        Debug.Log(BugHell.ShowV3(localTangency));
        Debug.Log(BugHell.ShowV3(triangles));
//        return localTangency.ToList();
        return triangles.ToList();
    }

    public float GetStraightLineAccuracy(Vector3 sourcePoint, Vector3 targetPoint)
    {
        Vector3 sp = sm.PointToCellCenterXZ(sourcePoint, new Vector3(.5f, 0, .5f));
        Vector3 tp = sm.PointToCellCenterXZ(targetPoint, new Vector3(.5f, 0, .5f));

        RecountEquation(sp, tp);
//        Debug.Log(ScoreStraightZ(sp) + " " + ScoreStraightZ(tp));
        Debug.DrawLine(ScoreStraightZ(sp), ScoreStraightZ(tp), Color.magenta);
        Debug.Break();

        List<Vector3> obstaclesList = StripClosestCells(sourcePoint, targetPoint, GetIntersection(sourcePoint, targetPoint));
        List<Vector3[]> obstaclesImpact = ScoreImpact(obstaclesList);

        return SumObstacles(sourcePoint, obstaclesImpact);

//        Debug.Break();
//        return GetStraightLineAccuracyX(sourcePoint, targetPoint) + 
//                       GetStraightLineAccuracyZ(sourcePoint, targetPoint);
    }

    public float GetStraightLineAccuracyX(Vector3 sourcePoint, Vector3 targetPoint)
    {
        float finiteElevation = 0;
        bool fullObstacle = false;

        if (sourcePoint.x == targetPoint.x)
        {

            float sign = Mathf.Sign(targetPoint.z - sourcePoint.z);
            int stepsToReach = Math.Abs(Convert.ToInt32(targetPoint.z - sourcePoint.z));

            for (int i = 1; i < stepsToReach; i++)
            {
                Vector3 countedCell = new Vector3(sourcePoint.x, 0,sourcePoint.z + i * sign);
                Vector3 checkCell = Cells.First(item => item.x == countedCell.x && item.z == countedCell.z);
                float elevationDifference = Math.Abs(sourcePoint.y - checkCell.y);

                if (elevationDifference >= 1)
                {
                    fullObstacle = true;
                    finiteElevation = elevationDifference;
                } 
                else if (elevationDifference < 1 && elevationDifference > 0 && !fullObstacle)
                {
                    finiteElevation = elevationDifference;
                }
            }
        }

        return finiteElevation;
    }



    public float GetStraightLineAccuracyZ(Vector3 sourcePoint, Vector3 targetPoint)
    {
        float finiteElevation = 0;
        bool fullObstacle = false;
        if (sourcePoint.z == targetPoint.z)
        {
//            Debug.Log("1 " + sourcePoint + " " + targetPoint);
            float sign = Mathf.Sign(targetPoint.x - sourcePoint.x);
            int stepsToReach = Math.Abs(Convert.ToInt32(targetPoint.x - sourcePoint.x));
//            Debug.Log(stepsToReach);
            for (int i = 1; i < stepsToReach; i++)
            {
                Vector3 countedCell = new Vector3(sourcePoint.x + i * sign, sourcePoint.y, sourcePoint.z);
                Vector3 checkCell = Cells.First(item => item.x == countedCell.x && item.z == countedCell.z);
//                Debug.Log("2 " + checkCell);
                float elevationDifference = Math.Abs(sourcePoint.y - checkCell.y);
//                Debug.Log(elevationDifference);
                if (elevationDifference >= 1)
                {
                    fullObstacle = true;
                    finiteElevation = elevationDifference;
                }
                else if (elevationDifference < 1 && elevationDifference > 0 && !fullObstacle)
                {
                    finiteElevation = elevationDifference;
                }
            }
        }

        return finiteElevation;
    }
}
