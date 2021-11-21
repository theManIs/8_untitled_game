using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

public class PathFinderByCells
{
//    public ConstantConstraints Cc;
    public StaticMath SMath;
    public LevelDissectorPlain Ldap;
    public Transform ColorSquareObject;
    public CharacterInnateTraits Cit;

    private Transform[] _movementHighlight = new Transform[0];
    private HashSet<Vector3> _movementHashSet = new HashSet<Vector3>();
    private bool _showHideLock = false;

    public Vector3 FindNextCell(Vector3 curPosition, Vector3 moveEndPoint)
    {
        float xDistance = moveEndPoint.x - curPosition.x;
        float zDistance = moveEndPoint.z - curPosition.z;

        Vector3 newPosition = Vector3.zero;
        Vector3 curTile = new Vector3(curPosition.x, curPosition.y, curPosition.z);
        Vector3 xMovement = new Vector3(Ldap.MaxStepDistance * Math.Sign(xDistance), 0, 0);
        Vector3 zMovement = new Vector3(0, 0, Ldap.MaxStepDistance * Math.Sign(zDistance));
        Vector3 xNextStep = xMovement + curTile;
        Vector3 zNextStep = zMovement + curTile;
        bool hasXMovement = false;
        bool hasZMovement = false;

        foreach (var vector3 in _movementHashSet)
        {
            if (curTile.x == vector3.x && curTile.z == vector3.z)
            {
                continue;
            }

            if (vector3.x == xNextStep.x && vector3.z == xNextStep.z)
            {
                hasXMovement = true;
                //                Debug.Log("X " + vector3);
            }

            if (vector3.x == zNextStep.x && vector3.z == zNextStep.z)
            {
                hasZMovement = true;
                //                Debug.Log("Z " + vector3);
            }
        }

        if (hasXMovement && hasZMovement)
        //        if (false)
        {
            if ((xDistance > .1f || xDistance < -.1f) && (zDistance > .1f || zDistance < -.1f))
            {
                if (Math.Abs(xDistance) > Math.Abs(zDistance))
                {
                    newPosition = curPosition + new Vector3(Ldap.MaxStepDistance * Math.Sign(xDistance), 0, 0);
                }
                else
                {
                    newPosition = curPosition + new Vector3(0, 0, Ldap.MaxStepDistance * Math.Sign(zDistance));
                }
            }
            else
            {
                if (xDistance > .1f || xDistance < -.1f)
                {
                    newPosition = curPosition + new Vector3(Ldap.MaxStepDistance * Math.Sign(xDistance), 0, 0);

                }
                else if (zDistance > .1f || zDistance < -.1f)
                {
                    newPosition = curPosition + new Vector3(0, 0, Ldap.MaxStepDistance * Math.Sign(zDistance));
                }
            }
        }
        else if (hasXMovement)
        //        else if (true)
        {
            //            Debug.Log((xMovement + curTile) + " " + (zMovement + curTile) + " " + curPosition);
            newPosition = curPosition + new Vector3(Ldap.MaxStepDistance * Math.Sign(xDistance), 0, 0);
        }
        else if (hasZMovement)
        {
            //            Debug.Log((xMovement + curTile) + " " + (zMovement + curTile) + " " + curPosition);
            newPosition = curPosition + new Vector3(0, 0, Ldap.MaxStepDistance * Math.Sign(zDistance));
        }
        //        else
        //        {
        //            foreach (var vector3 in _movementHashSet)
        //            {
        //                    Debug.Log("X " + vector3);
        //            }
        //            Debug.Break();
        //        }
        //
        //        if (newPosition == Vector3.zero)
        //        {
        //            Debug.Log(newPosition);
        //            foreach (var vector3 in _movementHashSet)
        //            {
        //                Debug.Log("X " + vector3);
        //            }
        //            Debug.Break();
        //        }

        if (newPosition != Vector3.zero)
        {
            newPosition.y = SMath.GetTargetCell(Ldap.AllCellsInOneArray, SMath.CellCenterToPointXZ(newPosition)).y;
        }

        return newPosition;
    }

    public HashSet<Vector3> AddToHashSetIf(HashSet<Vector3> generalPath, Vector3 pointInSpace, float shiftDistance, Vector3 direction)
    {
        pointInSpace.y = SMath.RoundByThousand(pointInSpace.y);
        Vector3 directedStep = ClampBounds(pointInSpace + direction * shiftDistance);

//        Debug.Log(Math.Abs(directedStep.y - pointInSpace.y) + " " + directedStep.y + " " + pointInSpace.y);
        if (Math.Abs(directedStep.y - pointInSpace.y) <= Ldap.MaxStepHeight)
        {
            generalPath.Add(directedStep);
        }

        return generalPath;
    }

    public Vector3 ClampBounds(Vector3 pointInSpace)
    {
        pointInSpace.x = Mathf.Clamp(pointInSpace.x, Ldap.MinX, Ldap.MinX + Ldap.MapSizeX);
        pointInSpace.z = Mathf.Clamp(pointInSpace.z, Ldap.MinZ, Ldap.MinZ + Ldap.MapSizeZ);

//        Debug.Log(pointInSpace + " " + SMath.GetTargetCell(Ldap.AllCellsInOneArray, SMath.CellCenterToPointXZ(pointInSpace)).y + " " + SMath.RoundByThousand(GetElevation(pointInSpace)));
//        pointInSpace.y = GetElevation(pointInSpace);
        pointInSpace.y = SMath.GetTargetCell(Ldap.AllCellsInOneArray, SMath.CellCenterToPointXZ(pointInSpace)).y;


        return pointInSpace;
    }

//    public float GetElevation(Vector3 pointInSpace)
//    {
//        Vector3 coordinates = pointInSpace + new Vector3(0, 10, 0);
//
//        Physics.Raycast(new Ray(coordinates, Vector3.down), out RaycastHit raycastHit);
//
//        float elevationHeight = raycastHit.point.y;
//
//        //        Debug.Log(elevationHeight + " " + _smath.GetTargetCell(_ldp.AllCellsInOneArray, _smath.CellCenterToPointXZ(pointInSpace)).y);
//
//        return elevationHeight;
//        //        return _smath.GetTargetCell(_ldp.AllCellsInOneArray, _smath.CellCenterToPointXZ(pointInSpace)).y;
//    }

    public HashSet<Vector3> StepCellAdder(HashSet<Vector3> _previousHashSet, int iterator)
    {
        iterator--;

        HashSet<Vector3> interHashSet = new HashSet<Vector3>(_previousHashSet);

        foreach (Vector3 recursiVector3 in _previousHashSet)
        {
            interHashSet = AddToHashSet(interHashSet, recursiVector3, Ldap.MaxStepDistance);
        }

        if (iterator != 0)
        {
            return StepCellAdder(interHashSet, iterator);
        }

        return interHashSet;
    }

    private HashSet<Vector3> AddToHashSet(HashSet<Vector3> generalPath, Vector3 pointInSpace, float shiftDistance)
    {
        //        Vector3 rightStep = ClampBounds(pointInSpace + Vector3.right * shiftDistance);
        //
        //        if (Math.Abs(rightStep.y - pointInSpace.y) <= .5f)
        //        {
        //            generalPath.Add(rightStep);
        //        }

        generalPath = AddToHashSetIf(generalPath, pointInSpace, shiftDistance, Vector3.right);

        Vector3 leftStep = ClampBounds(pointInSpace + Vector3.left * shiftDistance);

        if (Math.Abs(leftStep.y - pointInSpace.y) <= .5f)
        {
            generalPath.Add(leftStep);
        }

        Vector3 forwardStep = ClampBounds(pointInSpace + Vector3.forward * shiftDistance);

        if (Math.Abs(forwardStep.y - pointInSpace.y) <= .5f)
        {
            //            Debug.Log(forwardStep + " " + pointInSpace);
            generalPath.Add(forwardStep);
        }

        Vector3 backStep = ClampBounds(pointInSpace + Vector3.back * shiftDistance);

        if (Math.Abs(backStep.y - pointInSpace.y) <= .5f)
        {
            generalPath.Add(backStep);
        }

        return generalPath;
    }

    public void ResetRange(Vector3 tilePosition)
    {
        HideRange();
        ShowRange(tilePosition);
    }

    public void HideRange()
    {
        if (_showHideLock)
        {
            _showHideLock = false;

            foreach (Transform transform1 in _movementHighlight)
            {
                Object.Destroy(transform1.gameObject);
            }
        }
    }

    public void ShowRange(Vector3 tilePosition)
    {
        _showHideLock = true;

        Vector3 currentTile = tilePosition;
        _movementHashSet = StepCellAdder(new HashSet<Vector3> { currentTile }, Cit.MovementRange);
        _movementHashSet = RemovePosition(RemoveOutOfBounds(_movementHashSet), currentTile);

        int iterator = 0;
        Transform si = ColorSquareObject;
        _movementHighlight = new Transform[_movementHashSet.Count];

        foreach (Vector3 vector3 in _movementHashSet)
        {
            Vector3 highlightPoint = vector3;
            highlightPoint.y += .001f;
            Transform newSi = Object.Instantiate(si, highlightPoint, si.rotation);
            newSi.GetComponent<MeshRenderer>().material.color = Color.yellow;
//            newSi.transform.parent = transform;

            _movementHighlight[iterator] = newSi;
            iterator++;
        }
        //        Debug.Log(BugHell.ShowV3(_movementHashSet));
        //        Debug.Log(BugHell.ShowV3(_movementHighlight));
        //        Debug.Log(currentTile);
    }

    private HashSet<Vector3> RemoveOutOfBounds(HashSet<Vector3> setToCut)
    {
        setToCut.RemoveWhere(point => point.x <= Ldap.MinX || point.x >= Ldap.MaxX);
        setToCut.RemoveWhere(point => point.z <= Ldap.MinZ || point.z >= Ldap.MaxZ);

        return setToCut;
    }

    private HashSet<Vector3> RemovePosition(HashSet<Vector3> setToCut, Vector3 posToRemove)
    {
        setToCut.RemoveWhere(point => point.x.Equals(posToRemove.x) && point.z.Equals(posToRemove.z));

        return setToCut;
    }

}
