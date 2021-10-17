using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainCapsulePlayer : MonoBehaviour
{
    private MeshRenderer _defaultMaterial;
    private Color _defaultColor;
    private PlayerRequestOrder _playerRequestOrder;
    
    private ConstantConstraints _constantConstraints;
    private float _xCell = 1;
    private float _zCell = 1;

    private Vector3 _moveEndPoint;
    private bool _isMovingToEnd = false;
    private bool _moveToNextCell = false;
    private Vector3 _nextCell;
    private float _deltaSum = 0f;
    private Vector3 _startCellPosition;
    private bool _hasChangedEndPoint = false;
    private Vector3 _changedEndPoint;
    private Vector3 _newElevation;
    private Bounds _meshBounds;

    private HashSet<Vector3> _movementHashSet = new HashSet<Vector3>();
    private Transform[] _movementHighlight = new Transform[0];
    private Vector3 _previousPosition = Vector3.zero;
    private Transform _colorSquareInstance;
    private bool _showHideLock = false;

    private bool _thisInstanceReady = false;

    public void OnEnable()
    {
        _defaultMaterial = GetComponent<MeshRenderer>();
        _defaultColor = _defaultMaterial.material.color;
        _playerRequestOrder = FindObjectOfType<PlayerRequestOrder>();
        _constantConstraints = FindObjectOfType<ConstantConstraints>();
        _xCell = _constantConstraints.OneCell.x;
        _zCell = _constantConstraints.OneCell.z;
        _meshBounds = GetComponent<MeshRenderer>().bounds;

        Transform originalSquare = FindObjectOfType<SquareInstance>().transform;
        _colorSquareInstance = Instantiate(originalSquare, new Vector3(0, -100, 0), originalSquare.rotation);

//        ShowRange();
    }

    public void OnMouseEnter()
    {
        if (!_thisInstanceReady)
        {
            _defaultMaterial.material.color = Color.cyan;
        }
    }

    public void OnMouseExit()
    {
        if (!_thisInstanceReady)
        {
            _defaultMaterial.material.color = _defaultColor;
        }
    }

    void OnMouseDown()
    {
        if (_defaultMaterial.material.color == Color.red)
        {
            _defaultMaterial.material.color = _defaultColor;
            _thisInstanceReady = false;

            HideRange();
        }
        else
        {
            _defaultMaterial.material.color = Color.red;
            _thisInstanceReady = true;

            ShowRange();
        }
    }

//    void OnMouseExit()
//    {
//        _defaultMaterial.material.color = _defaultColor;
//    }

    // Update is called once per frame
    void Update()
    {
        if (_thisInstanceReady && _playerRequestOrder.NewMove)
        {
            _playerRequestOrder.NewMove = false;
            Vector3 yPos = new Vector3(_playerRequestOrder.MoveToClick.x, _playerRequestOrder.MoveToClick.y + _meshBounds.extents.y, _playerRequestOrder.MoveToClick.z);

            if (_moveToNextCell)
            {
                _changedEndPoint = yPos;
                _hasChangedEndPoint = true;
            }
            else
            {
                _moveEndPoint = yPos;
                _isMovingToEnd = true;
            }

            HideRange();
        }

        if (_isMovingToEnd)
        {
            _isMovingToEnd = false;

            if (_hasChangedEndPoint)
            {
                _hasChangedEndPoint = false;
                _moveEndPoint = _changedEndPoint;
                _changedEndPoint = Vector3.zero;
            }

            float xDistance = _moveEndPoint.x - transform.position.x;
            float zDistance = _moveEndPoint.z - transform.position.z;
//            Debug.Log(_moveEndPoint);
//            Debug.Log(xDistance);

            _nextCell = FindNextCell(transform.position, xDistance, zDistance);

            if (_nextCell != Vector3.zero)
            {
                _moveToNextCell = true;

                Vector3 coordinates = _nextCell + new Vector3(0, 10, 0);
                //            GameObject gm = new GameObject($"coordinates");
                //            gm.transform.position = coordinates;

                if (Physics.Raycast(new Ray(coordinates, Vector3.down), out RaycastHit hitinfo))
                {
                    _newElevation = new Vector3(0, hitinfo.point.y - transform.position.y + _meshBounds.extents.y, 0);
                    _nextCell += _newElevation;
                }

                _startCellPosition = transform.position;
            }
            else
            {

                _nextCell = Vector3.zero;
                _moveEndPoint = Vector3.zero;
                _moveToNextCell = false;
                _isMovingToEnd = false;

                ResetRange();
            }
        }

        if (_moveToNextCell)
        {
            _deltaSum += Time.deltaTime;

            transform.position = Vector3.Lerp(_startCellPosition, _nextCell, _deltaSum);

//            if (Vector3.Distance(_nextCell, transform.position) < 0.1f)
            if (_nextCell == transform.position)
            {
//                transform.position = _nextCell + _newElevation;
                transform.position = _nextCell;

//                Debug.Log(_nextCell + " " + Vector3.Distance(_moveEndPoint, transform.position) + " " + _meshBounds.extents.y);
                if (Vector3.Distance(_moveEndPoint, transform.position) < 0.1f)
                {
                    _nextCell = Vector3.zero;
                    _moveEndPoint = Vector3.zero;
                    _moveToNextCell = false;

                    ResetRange();
                }
                else
                {
                    _isMovingToEnd = true;
                }

                _newElevation = Vector3.zero;
                _deltaSum = 0f;
            }
        }
    }

    public Vector3 FindNextCell(Vector3 curPosition, float xDistance, float zDistance)
    {
        Vector3 newPosition = Vector3.zero;
        Vector3 curTile = new Vector3(curPosition.x, curPosition.y, curPosition.z);
        curTile.y -= _meshBounds.extents.y;
        Vector3 xMovement = new Vector3(_xCell * Math.Sign(xDistance), 0, 0);
        Vector3 zMovement = new Vector3(0, 0, _zCell * Math.Sign(zDistance));
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
                    newPosition = curPosition + new Vector3(_xCell * Math.Sign(xDistance), 0, 0);
                }
                else
                {
                    newPosition = curPosition + new Vector3(0, 0, _zCell * Math.Sign(zDistance));
                }
            }
            else
            {
                if (xDistance > .1f || xDistance < -.1f)
                {
                    newPosition = curPosition + new Vector3(_xCell * Math.Sign(xDistance), 0, 0);

                }
                else if (zDistance > .1f || zDistance < -.1f)
                {
                    newPosition = curPosition + new Vector3(0, 0, _zCell * Math.Sign(zDistance));
                }
            }
        }
        else if (hasXMovement)
        //        else if (true)
        {
//            Debug.Log((xMovement + curTile) + " " + (zMovement + curTile) + " " + curPosition);
            newPosition = curPosition + new Vector3(_xCell * Math.Sign(xDistance), 0, 0);
        }
        else if (hasZMovement)
        {
//            Debug.Log((xMovement + curTile) + " " + (zMovement + curTile) + " " + curPosition);
            newPosition = curPosition + new Vector3(0, 0, _zCell * Math.Sign(zDistance));
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

        return newPosition;
    }

    private void ResetRange()
    {
        HideRange();
        ShowRange();
    }

    private void HideRange()
    {
        if (_showHideLock)
        {
            _showHideLock = false;

            foreach (Transform transform1 in _movementHighlight)
            {
                Destroy(transform1.gameObject);
            }
        }
    }

    private void ShowRange()
    {
        _showHideLock = true;

        Vector3 currentTile = transform.position - new Vector3(0, _meshBounds.extents.y, 0);
        _movementHashSet = StepCellAdder(new HashSet<Vector3>{ currentTile }, _constantConstraints.BaseMovementRange);
        _movementHashSet.RemoveWhere((item) => item.x == currentTile.x && item.z == currentTile.z);

        int iterator = 0;
        Transform si = _colorSquareInstance;
        _movementHighlight = new Transform[_movementHashSet.Count];

        foreach (Vector3 vector3 in _movementHashSet)
        {
            Vector3 highlightPoint = vector3;
            highlightPoint.y += .001f;
            Transform newSi = Instantiate(si, highlightPoint, si.rotation);
            newSi.GetComponent<MeshRenderer>().material.color = Color.yellow;
            newSi.transform.parent = transform;

            _movementHighlight[iterator] = newSi;
            iterator++;
        }
    }


    private HashSet<Vector3> StepCellAdder(HashSet<Vector3> _previousHashSet, int iterator)
    {
        iterator--;

        HashSet<Vector3> interHashSet = new HashSet<Vector3>(_previousHashSet);

        foreach (Vector3 recursiVector3 in _previousHashSet)
        {
            interHashSet = AddToHashSet(interHashSet, recursiVector3, _constantConstraints.MaxStepDistance);
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

    private HashSet<Vector3> AddToHashSetIf(HashSet<Vector3> generalPath, Vector3 pointInSpace, float shiftDistance, Vector3 direction)
    {
        Vector3 directedStep = ClampBounds(pointInSpace + direction * shiftDistance);

        if (Math.Abs(directedStep.y - pointInSpace.y) <= _constantConstraints.MaxStepHeight)
        {
            generalPath.Add(directedStep);
        }

        return generalPath;
    }

    private Vector3 ClampBounds(Vector3 pointInSpace)
    {
        pointInSpace.x = Mathf.Clamp(pointInSpace.x, _constantConstraints.MinPointX,
            _constantConstraints.MinPointX + _constantConstraints.MapSizeX);

        pointInSpace.z = Mathf.Clamp(pointInSpace.z, _constantConstraints.MinPointZ,
            _constantConstraints.MinPointZ + _constantConstraints.MapSizeZ);

        pointInSpace.y = GetElevation(pointInSpace);

        return pointInSpace;
    }

    private float GetElevation(Vector3 pointInSpace)
    {
        Vector3 coordinates = pointInSpace + new Vector3(0, 10, 0);

        Physics.Raycast(new Ray(coordinates, Vector3.down), out RaycastHit raycastHit);

        float elevationHeight = raycastHit.point.y;

        return elevationHeight;
    }
}
