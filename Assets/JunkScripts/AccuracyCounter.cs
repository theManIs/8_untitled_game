using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AccuracyCounter
{
    public Vector3[] Cells;

    private StaticMath sm = new StaticMath();
    private float shift = 1;
    private float coef = 1;
    private int xSign = -1;
    private Vector3 extent = new Vector3(.5f, 0, .5f);

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
        Debug.Log(xSign + " " + shift + " " + sourcePoint + " " + targetPoint + " coef:" + coef + " " + xProjection + " " + zProjection);
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
        return (int)coef == 0 ? z : ScoreSzf(x);
    }
    private float ScoreSxf(float z)
    {
        return (float)(z - shift) / (float)(xSign * coef);
    }

    private float ScoreSxf(float z, float x)
    {
        return (int)coef == 0 ? x : ScoreSxf(z);
    }

    private float ScoreShift(float x, float z)
    {
        return z - x * xSign * coef;
    }

    public List<Vector3> GetIntersection(Vector3 sourcePoint, Vector3 targetPoint)
    {

        HashSet<Vector3> tangency = new HashSet<Vector3>();
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
                        // Debug.Log(plane[i].x + " > " + touchPin + " > " + plane[i + 1].x + " shift:" + shift + " coef:" + coef);
                        if (touchPin >= plane[i].x && touchPin <= plane[i + 1].x || touchPin >= plane[i + 1].x && touchPin <= plane[i].x)
                        {
                            intersectionList.Add(plane[0]);
                            tangency.Add(new Vector3(touchPin, 0, plane[i].z));
                        }
                    }

                    if (i == 0 || i == 2)
                    {
                        float touchPin = ScoreSzf(plane[i].x, plane[i].z);
                        // Debug.Log(plane[i].x + " > " + touchPin + " > " + plane[i + 1].x + " shift:" + shift + " coef:" + coef);
                        if (touchPin >= plane[i].z && touchPin <= plane[i + 1].z || touchPin >= plane[i + 1].z && touchPin <= plane[i].z)
                        {
                            intersectionList.Add(plane[0]);
                            tangency.Add(new Vector3(plane[i].x, 0, touchPin));
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
        
//        Debug.Log(BugHell.ShowV3(tangency));
//        Debug.Log(BugHell.ShowV3(intersectionList));
        return intersectionList.ToList();
    }

    public List<Vector3> StripClosestCells(Vector3 sourcePoint, Vector3 targetPoint, List<Vector3> obstaclesList)
    {
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

    public float SumObstacles(Vector3 sourcePoint, List<Vector3> obstaclesList)
    {
        float finiteElevation = 0;
        bool fullObstacle = false;
//        Debug.Log(sourcePoint.x + " " + sourcePoint.y + " " + sourcePoint.z);
//        Debug.Log("obstaclesList " + BugHell.ShowV3(obstaclesList));
        foreach (Vector3 obstacleCell in obstaclesList)
        {
            Vector3 checkCell = Cells.First(item => item.x == obstacleCell.x && item.z == obstacleCell.z);
            float elevationDifference = sm.HeightToHalfRoundFloat(sm.HeightToHalfRoundFloat(checkCell.y) - sm.HeightToHalfRoundFloat(sourcePoint.y));
//            Debug.Log(checkCell.y.ToString("N") + " " + sm.HeightToHalfRoundFloat(checkCell.y).ToString("N") + " " + Mathf.Abs(3 % 1f));
//            Debug.Log(sourcePoint.y.ToString("N") + " " + sm.HeightToHalfRoundFloat(sourcePoint.y).ToString("N") + " " + Mathf.Abs(1 % 1f));
//            Debug.Log(elevationDifference);

            if (elevationDifference >= 1)
            {
//                Debug.Log("checkCell " + checkCell + "\nelevationDifference " + elevationDifference + "\nsourcePoint.y " + sourcePoint.y + "\ncheckCell.y " +  checkCell.y + "\nsourcePoint.y - checkCell.y " + (sourcePoint.y - checkCell.y));
                fullObstacle = true;
                finiteElevation = 1;
            }
            else if (elevationDifference < 1 && elevationDifference > 0 && !fullObstacle)
            {
//                Debug.Log(elevationDifference);
//                Debug.Log("checkCell " + checkCell + "\nelevationDifference " + elevationDifference + "\nsourcePoint.y " + sourcePoint.y + "\ncheckCell.y " + checkCell.y + "\nsourcePoint.y - checkCell.y " + (sourcePoint.y - checkCell.y));
                finiteElevation = elevationDifference;
            }
        }

        return finiteElevation;
    }


    public float GetStraightLineAccuracy(Vector3 sourcePoint, Vector3 targetPoint)
    {
        Vector3 sp = sm.PointToCellCenterXZ(sourcePoint, new Vector3(.5f, 0, .5f));
        Vector3 tp = sm.PointToCellCenterXZ(targetPoint, new Vector3(.5f, 0, .5f));

        RecountEquation(sp, tp);
//        Debug.Log(ScoreStraightZ(sp) + " " + ScoreStraightZ(tp));
        Debug.DrawLine(ScoreStraightZ(sp), ScoreStraightZ(tp), Color.magenta);
        Debug.Break();

        return SumObstacles(sourcePoint, StripClosestCells(sourcePoint, targetPoint, GetIntersection(sourcePoint, targetPoint)));

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
