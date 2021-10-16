using System;
using System.Collections.Generic;
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

    public void OnEnable()
    {
        _defaultMaterial = GetComponent<MeshRenderer>();
        _defaultColor = _defaultMaterial.material.color;
        _playerRequestOrder = FindObjectOfType<PlayerRequestOrder>();
        _constantConstraints = FindObjectOfType<ConstantConstraints>();
        _xCell = _constantConstraints.OneCell.x;
        _zCell = _constantConstraints.OneCell.z;
        _meshBounds = GetComponent<MeshRenderer>().bounds;
    }

    void OnMouseDown()
    {
        if (_defaultMaterial.material.color == Color.red)
        {
            _defaultMaterial.material.color = _defaultColor;
        }
        else
        {
            _defaultMaterial.material.color = Color.red;
        }
    }

//    void OnMouseExit()
//    {
//        _defaultMaterial.material.color = _defaultColor;
//    }

    // Update is called once per frame
    void Update()
    {
        if (_playerRequestOrder.NewMove)
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

        return newPosition;
    }

//    private HashSet<Vector3> AddToHashSet(HashSet<Vector3> generalPath, Vector3 pointInSpace, float shiftDistance)
//    {
//        generalPath.Add(pointInSpace + new Vector3(shiftDistance, 0, 0));
//        generalPath.Add(pointInSpace + new Vector3(-shiftDistance, 0, 0));
//        generalPath.Add(pointInSpace + new Vector3(0, 0, shiftDistance));
//        generalPath.Add(pointInSpace + new Vector3(0, 0, -shiftDistance));
//
//        return generalPath;
//    }
}
