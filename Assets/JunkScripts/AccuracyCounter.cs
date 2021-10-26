using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AccuracyCounter
{
    public Vector3[] Cells;

    public List<Vector3> GetIntersection(Vector3 sourcePoint, Vector3 targetPoint)
    {
        List<Vector3> intersectionList = new List<Vector3>();

        float shift = targetPoint.z - targetPoint.x;
//        z = x + shift

        float mX = sourcePoint.x < targetPoint.x ? sourcePoint.x : targetPoint.x;
        float maX = sourcePoint.x > targetPoint.x ? sourcePoint.x : targetPoint.x;

//        Debug.Log(mX + " " + maX + " " + mZ + " " + maZ);

        for (; mX <= maX; mX++)
        {
            float mZ = sourcePoint.z < targetPoint.z ? sourcePoint.z : targetPoint.z;
            float maZ = sourcePoint.z > targetPoint.z ? sourcePoint.z : targetPoint.z;

            for (; mZ <= maZ; mZ++)
            {
                Vector2[] plane =
                {
                    new Vector2(mX, mZ),
                    new Vector2(mX, mZ - 1), 
                    new Vector2(mX - 1, mZ - 1), 
                    new Vector2(mX - 1, mZ), 
                };

                bool intersects = false;

                foreach (Vector2 v2 in plane)
                {
                    if (v2.x + shift >= v2.y && v2.x + shift <= v2.y)
                    {
                        intersects = true;
                    }
                }

//                Debug.Log(mX + " " + mZ + " " + intersects);

                if (intersects)
                {
                    intersectionList.Add(new Vector3(mX, 0, mZ));
                }

//                Debug.Log(mX + " " + maX + " " + mZ + " " + maZ);
//                        Vector3 countedCell = new Vector3(mX, 0, mZ);
//                        
//                        float zSegmentTop = mZ;
//                        float zSegmentBot = mZ - 1;
//
//                        bool intersectTopX = mX + shift >= zSegmentBot && mX + shift <= zSegmentTop  ;
//                
//                        Debug.Log(intersectTopX + " " + countedCell + " " + (mX + shift));
            }
        }

        intersectionList = intersectionList.Where(item =>
            item.x != sourcePoint.x && item.z != sourcePoint.z || item.x != targetPoint.x && item.z != targetPoint.z).ToList();

        return intersectionList;
    }

    public List<Vector3> StripClosestCells(Vector3 sourcePoint, List<Vector3> obstaclesList)
    {
        return obstaclesList.Where(item =>
        {
            if (item.x == sourcePoint.x && item.z == sourcePoint.z + 1)
            {
                return false;
            }
            else if (item.x == sourcePoint.x && item.z == sourcePoint.z - 1)
            {
                return false;
            }
            else if (item.x == sourcePoint.x + 1 && item.z == sourcePoint.z)
            {
                return false;
            }
            else if (item.x == sourcePoint.x - 1 && item.z == sourcePoint.z)
            {
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

        foreach (Vector3 obstacleCell in obstaclesList)
        {
            Vector3 checkCell = Cells.First(item => item.x == obstacleCell.x && item.z == obstacleCell.z);
            float elevationDifference = Math.Abs(sourcePoint.y - checkCell.y);

            if (elevationDifference >= 1)
            {
                fullObstacle = true;
                finiteElevation = 1;
            }
            else if (elevationDifference < 1 && elevationDifference > 0 && !fullObstacle)
            {
                finiteElevation = elevationDifference;
            }
        }

        return finiteElevation;
    }


    public float GetStraightLineAccuracy(Vector3 sourcePoint, Vector3 targetPoint)
    {
        return SumObstacles(sourcePoint, StripClosestCells(sourcePoint, GetIntersection(sourcePoint, targetPoint)));

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
