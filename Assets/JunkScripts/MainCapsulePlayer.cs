using System;
using System.Collections.Generic;
using UnityEngine;

public class MainCapsulePlayer : MonoBehaviour
{
    public event Action AccuracyRecount;

//    public Transform OriginSquare;
    public Vector3 playerSquare;
    public bool ThisInstanceReady = false;
    public Transform TransformToAnimate;
    public CharacterInnateTraits InnateTraits;

    private MeshRenderer _defaultMaterial;
    private Color _defaultColor;
    private PlayerRequestOrder _playerRequestOrder;
    
//    private ConstantConstraints _cCon;
//    private float _xCell = 1;
//    private float _zCell = 1;

    private Vector3 _moveEndPoint;
    private bool _isMovingToEnd = false;
    private bool _moveToNextCell = false;
    private Vector3 _nextCellCenter;
    private float _deltaSum = 0f;
    private Vector3 _startCellPosition;
    private bool _hasChangedEndPoint = false;
    private Vector3 _changedEndPoint;
//    private Vector3 _newElevation;
    private Bounds _meshBounds;

//    private HashSet<Vector3> _movementHashSet = new HashSet<Vector3>();
//    private Vector3 _previousPosition = Vector3.zero;
    private static Transform _colorSquareInstance;
//    private bool _showHideLock = false;
    private StaticMath _smath;
    private LevelDissectorPlain _ldp;
    private PathFinderByCells _pfc;
    private AnimateFuckingTrash _aft;

    public void OnEnable()
    {
        _defaultMaterial = GetComponentInChildren<MeshRenderer>();
        _defaultColor = _defaultMaterial.material.color;
        _playerRequestOrder = FindObjectOfType<PlayerRequestOrder>();
//        _cCon = FindObjectOfType<ConstantConstraints>();
//        _xCell = _cCon.OneCell.x;
//        _zCell = _cCon.OneCell.z;
        _meshBounds = GetComponent<Collider>().bounds;
        _smath = new StaticMath();
        _ldp = FindObjectOfType<LevelDissectorPlain>();

        if (_colorSquareInstance == null)
        {
            SquareInstance si = FindObjectOfType<SquareInstance>();
            _colorSquareInstance = Instantiate(si.transform, new Vector3(0, -100, 0), si.transform.rotation);
            _colorSquareInstance.transform.parent = si.transform;
        }

        _pfc = new PathFinderByCells { Cc = FindObjectOfType<ConstantConstraints>(), Ldap = _ldp, SMath = _smath, ColorSquareObject = _colorSquareInstance };
        PlayersAccomodation.AddPlayer(this);
        _aft = new AnimateFuckingTrash();

//        ShowRange();
    }

    public void SetInnateTraits(CharacterInnateTraits cit)
    {
        InnateTraits = cit;
        _pfc.Cit = cit;
    }

    public void OnMouseEnter()
    {
        if (!ThisInstanceReady)
        {
            _defaultMaterial.material.color = Color.cyan;
        }
    }

    public void OnMouseExit()
    {
        if (!ThisInstanceReady)
        {
            _defaultMaterial.material.color = _defaultColor;
        }
    }

    void OnMouseDown()
    {
        if (_defaultMaterial.material.color == Color.red)
        {
            _defaultMaterial.material.color = _defaultColor;
            ThisInstanceReady = false;

            _pfc.HideRange();
        }
        else
        {
            _defaultMaterial.material.color = Color.red;
            ThisInstanceReady = true;
            _playerRequestOrder.NewMove = false;

            AccuracyRecount?.Invoke();

            _pfc.ShowRange(playerSquare);
        }
    }

//    void OnMouseExit()
//    {
//        _defaultMaterial.material.color = _defaultColor;
//    }

    // Update is called once per frame
    void Update()
    {
        playerSquare = _smath.PointToCellCenterRounded(transform.position, new Vector3(.5f, 0, .5f));
//        if (ThisInstanceReady)
//        {
//            Debug.Log(playerSquare.x + " " + playerSquare.y + " " + playerSquare.z);
//            Debug.Log(_meshBounds.extents.x + " " + _meshBounds.extents.y + " " + _meshBounds.extents.z);
//            Debug.Log(transform.position.x + " " + transform.position.y + " " + transform.position.z);
//        }

        if (ThisInstanceReady && _playerRequestOrder.NewMove)
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

            _pfc.HideRange();
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

//            _nextCellCenter = FindNextCell(transform.position, _moveEndPoint);
            _nextCellCenter = _pfc.FindNextCell(transform.position, _moveEndPoint);
            _startCellPosition = transform.position;

            if (_nextCellCenter != Vector3.zero)
            {
                _moveToNextCell = true;

                _aft.RotateToMovementDirection(TransformToAnimate, _nextCellCenter);
            }
            else
            {
                _nextCellCenter = Vector3.zero;
                _moveEndPoint = Vector3.zero;
                _moveToNextCell = false;

                if (ThisInstanceReady)
                {
                    _pfc.ResetRange(playerSquare);
                }
            }

            AccuracyRecount?.Invoke();
        }

        if (_moveToNextCell)
        {
            _deltaSum += Time.deltaTime;

            transform.position = Vector3.Lerp(_startCellPosition, _nextCellCenter, _deltaSum);

            if (_nextCellCenter == transform.position)
            {
                transform.position = _nextCellCenter;

                _isMovingToEnd = true;
                _moveToNextCell = false;
                _deltaSum = 0f;

                AccuracyRecount?.Invoke();
            }
        }
    }
}
