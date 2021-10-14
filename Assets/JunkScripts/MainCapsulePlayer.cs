using System;
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

    public void OnEnable()
    {
        _defaultMaterial = GetComponent<MeshRenderer>();
        _defaultColor = _defaultMaterial.material.color;
        _playerRequestOrder = FindObjectOfType<PlayerRequestOrder>();
        _constantConstraints = FindObjectOfType<ConstantConstraints>();
        _xCell = _constantConstraints.OneCell.x;
        _zCell = _constantConstraints.OneCell.z;
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
            Vector3 yPos = new Vector3(_playerRequestOrder.MoveToClick.x, transform.position.y, _playerRequestOrder.MoveToClick.z);

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

            if ((xDistance > .1f || xDistance < -.1f) && (zDistance > .1f || zDistance < -.1f))
            {
//                Debug.Log(xDistance > zDistance);
                if (xDistance > zDistance)
                {
                    _nextCell = transform.position + new Vector3(_xCell * Math.Sign(xDistance), 0, 0);
                    _moveToNextCell = true;

                }
                else
                {
                    _nextCell = transform.position + new Vector3(0, 0, _zCell * Math.Sign(zDistance));
                    _moveToNextCell = true;
                }
            }
            else
            {
                if (xDistance > .1f || xDistance < -.1f)
                {
                    _nextCell = transform.position + new Vector3(_xCell * Math.Sign(xDistance), 0, 0);
                    _moveToNextCell = true;

                }
                else if (zDistance > .1f || zDistance < -.1f)
                {
                    _nextCell = transform.position + new Vector3(0, 0, _zCell * Math.Sign(zDistance));
                    _moveToNextCell = true;
                }
            }

            Vector3 coordinates = _nextCell + new Vector3(0, 10, 0);
//            GameObject gm = new GameObject($"coordinates");
//            gm.transform.position = coordinates;

            if (Physics.Raycast(new Ray(coordinates, Vector3.down), out RaycastHit hitinfo))
            {
                Debug.Log(hitinfo.point);
                MeshRenderer mr = GetComponent<MeshRenderer>();

                _newElevation = new Vector3(0, hitinfo.point.y - transform.position.y + 1, 0);
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
                Debug.Log(_newElevation);
//                transform.position = _nextCell + _newElevation;
                transform.position = _nextCell;

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
}
