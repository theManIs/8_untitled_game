using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MainCapsulePlayer : MonoBehaviour
{
    public event Action AccuracyRecount;
    public event Action CheckInInstance;

//    public Transform OriginSquare;
    public Vector3 playerSquare;
    public bool ThisInstanceReady = false;
    public Transform TransformToAnimate;
    public CharacterInnateTraits InnateTraits;
    public SkinnedMeshRenderer DefaultMaterial;
    public float TemporaryAccuracy = 0;
    public bool TemporaryHideAccuracy = false;
    public GameObject FloatingText;
    public Transform ShotgunToAppear;

    //    private Color _defaultColor;
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
//    private MainCapsulePlayer _lastAim;
//    private bool _hiddenShotGun = false;

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

        _pfc = new PathFinderByCells { Ldap = _ldp, SMath = _smath, ColorSquareObject = _colorSquareInstance };
        PlayersAccomodation.AddPlayer(this);
        _aft = new AnimateFuckingTrash { A = GetComponentInChildren<Animator>() };

//        ShowRange();
    }

    public void SetInnateTraits(CharacterInnateTraits cit)
    {
        InnateTraits = cit;
        _pfc.Cit = cit;
        DefaultMaterial.material.color = InnateTraits.BaseColor;
    }

    public void OnMouseEnter()
    {
        if (!ThisInstanceReady)
        {
            DefaultMaterial.material.color = InnateTraits.HoverColor;
        }
    }

    public void OnMouseExit()
    {
        if (!ThisInstanceReady)
        {
            DefaultMaterial.material.color = InnateTraits.BaseColor;
        }
    }

    public void SetShotTo(MainCapsulePlayer mcpAggressor)
    {

        GameObject go = Instantiate(FloatingText, transform.position, Quaternion.identity, transform);
        go.GetComponentInChildren<TextMeshPro>().text = Convert.ToInt32(mcpAggressor.InnateTraits.BaseDamage).ToString();
        TemporaryHideAccuracy = true;
        InnateTraits.TemporaryHealth -= (int)mcpAggressor.InnateTraits.BaseDamage;

        AudioSource asa = gameObject.GetComponent<AudioSource>();
        asa.clip = InnateTraits.ShotSound;
        asa.Play();
    }

    public IEnumerator GunRoutine(MainCapsulePlayer mcpAggressor)
    {

        mcpAggressor.KickOne(this);
        yield return new WaitForSeconds(.4f);
        mcpAggressor.ShotgunToAppear.gameObject.SetActive(true);
//        _hiddenShotGun = true;
//        Debug.Log("show it " + ShotgunToAppear.gameObject.activeSelf);
        yield return new WaitForSeconds(.6f);
        SetShotTo(mcpAggressor);
        AccuracyRecount?.Invoke();
        Invoke(nameof(ShowAccuracy), FloatingText.GetComponent<FloatingText>().DestroyTime);
        //        Debug.Log("shot it ");
        yield return new WaitForSeconds(.6f);
        mcpAggressor.ShotgunToAppear.gameObject.SetActive(false);
//        Debug.Log("hide it " + ShotgunToAppear.gameObject.activeSelf);
    }

    void OnMouseDown()
    {
        bool normalChange = true;

        foreach (MainCapsulePlayer mcp in PlayersAccomodation.ListOfPlayers)
        {
            if (mcp.ThisInstanceReady && mcp != this && InnateTraits.BaseColor != mcp.InnateTraits.BaseColor)
            {
                normalChange = false;

                if (InnateTraits.CheckDistance(playerSquare, mcp.playerSquare))
                {
                    StartCoroutine(GunRoutine(mcp));
                }
//                Debug.Break();
            }
        }

        if (normalChange)
        {
            if (DefaultMaterial.material.color == InnateTraits.ActiveColor)
            {
                InstanceCheckOut();
            }
            else
            {
                CheckInInstance?.Invoke();

                DefaultMaterial.material.color = InnateTraits.ActiveColor;
                ThisInstanceReady = true;
                _playerRequestOrder.NewMove = false;

                _pfc.ShowRange( playerSquare, PlayersAccomodation.GetSquarePositions().ToArray());
            }

            AccuracyRecount?.Invoke();
        }
    }

    public void ShowAccuracy()
    {
        TemporaryHideAccuracy = false;
        
        AccuracyRecount?.Invoke();
    }

    public void InstanceCheckOut()
    {
        DefaultMaterial.material.color = InnateTraits.BaseColor;
        ThisInstanceReady = false;

        _pfc.HideRange();
    }

    public void KickOne(MainCapsulePlayer mcp)
    {
        _aft.SetKickState();
        _aft.RotateToMovementDirection(TransformToAnimate, mcp.transform.position);
//        Debug.Log("KIck one");
    }

//    void OnMouseExit()
//    {
//        _defaultMaterial.material.color = _defaultColor;
//    }

    // Update is called once per frame
    void Update()
    {
        if (InnateTraits.TemporaryHealth <= 0)
        {
            _aft.SetDeathState(true);
            PlayersAccomodation.DelPlayer(this);
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            this.enabled = false;
        }

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
                _aft.SetMoveState(true);
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

                _aft.SetMoveState(false);
            }

//            AccuracyRecount?.Invoke();
        }

        if (_moveToNextCell)
        {
            _deltaSum += Time.deltaTime;

            transform.position = Vector3.Lerp(_startCellPosition, _nextCellCenter, _deltaSum * InnateTraits.AnimationPlaySpeed);

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
