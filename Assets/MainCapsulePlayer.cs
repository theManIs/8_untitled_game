using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainCapsulePlayer : MonoBehaviour
{
    private MeshRenderer _defaultMaterial;
    private Color _defaultColor;
    private PlayerRequestOrder _playerRequestOrder;

    private Vector3 _moveEndPoint;
    private bool _isMovingToEnd = false;
    private bool _moveToNextCell = false;
    private Vector3 _nextCell;
    private float _deltaSum = 0f;
    private Vector3 _startCellPosition;
    private bool _hasChangedEndPoint = false;
    private Vector3 _changedEndPoint;

    void Start()
    {
        _defaultMaterial = GetComponent<MeshRenderer>();
        _defaultColor = _defaultMaterial.material.color;
        _playerRequestOrder = FindObjectOfType<PlayerRequestOrder>();
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
            Vector3 yPos = new Vector3(0, transform.position.y, 0);

            if (_moveToNextCell)
            {
                _changedEndPoint = _playerRequestOrder.MoveToClick + yPos;
                _hasChangedEndPoint = true;
            }
            else
            {
                _moveEndPoint = _playerRequestOrder.MoveToClick + yPos;
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
            Debug.Log(xDistance);

            if ((xDistance > .1f || xDistance < -.1f) && (zDistance > .1f || zDistance < -.1f))
            {
                Debug.Log(xDistance > zDistance);
                if (xDistance > zDistance)
                {
                    _nextCell = transform.position + new Vector3(1.05f * Math.Sign(xDistance), 0, 0);
                    _moveToNextCell = true;

                }
                else
                {
                    _nextCell = transform.position + new Vector3(0, 0, 1.05f * Math.Sign(zDistance));
                    _moveToNextCell = true;
                }
            }
            else
            {
                if (xDistance > .1f || xDistance < -.1f)
                {
                    _nextCell = transform.position + new Vector3(1.05f * Math.Sign(xDistance), 0, 0);
                    _moveToNextCell = true;

                }
                else if (zDistance > .1f || zDistance < -.1f)
                {
                    _nextCell = transform.position + new Vector3(0, 0, 1.05f * Math.Sign(zDistance));
                    _moveToNextCell = true;
                }
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
                if (Vector3.Distance(_moveEndPoint, transform.position) < 0.1f)
                {
                    transform.position = _nextCell;
                    _moveToNextCell = false;
                    _nextCell = Vector3.zero;
                    _moveEndPoint = Vector3.zero;
                }
                else
                {
                    transform.position = _nextCell;
                    _isMovingToEnd = true;
                }

                _deltaSum = 0f;
            }
        }
    }
}
