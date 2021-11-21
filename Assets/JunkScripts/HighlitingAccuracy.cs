using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class HighlitingAccuracy : MonoBehaviour
{
    public RectTransform accuracyCanvas;

    private MainCapsulePlayer[] _mainCapsulePlayers;
    private MainCapsulePlayer _lastActivePlayer;
    private PoolOfRectTransform _rtPool;
    private ConstantConstraints _cc;
    private LevelDissectorPlain _ldp;
    private AccuracyCounter _ac = new AccuracyCounter();
    private StaticMath _sMath = new StaticMath();
    private float _realAccuracy = 0f;

    public void OnEnable()
    {
        _mainCapsulePlayers = FindObjectsOfType<MainCapsulePlayer>();
        _rtPool = new PoolOfRectTransform();
        _rtPool.SetCanonicalRect(accuracyCanvas);
        _cc = FindObjectOfType<ConstantConstraints>();
        _ldp = FindObjectOfType<LevelDissectorPlain>();
//        Debug.Log(_ldp.GetLevelDissected(_cc.LevelBounds));
        _ac = new AccuracyCounter{ Cells =  _ldp.GetLevelDissected(_cc.LevelMins, _cc.LevelBounds) };

//        StartCoroutine(ChangeAccuracyRoutine());
    }

    public void ChangeAccuracyRoutine()
//    public IEnumerator ChangeAccuracyRoutine()
    {
//        for (;;)
//        {
            _mainCapsulePlayers = FindObjectsOfType<MainCapsulePlayer>();

            if (_mainCapsulePlayers.Count(item => item.ThisInstanceReady) > 0)
            {
                MainCapsulePlayer activePlayer = _mainCapsulePlayers.First(item => item.ThisInstanceReady);
                MainCapsulePlayer[] nonActivePlayers = _mainCapsulePlayers.Where(item => !item.ThisInstanceReady).ToArray();

                foreach (MainCapsulePlayer capsule in nonActivePlayers)
                {
                    _realAccuracy = _ac.GetStraightLineAccuracy(_sMath.CellCenterToPointXZ(activePlayer.playerSquare), _sMath.CellCenterToPointXZ(capsule.playerSquare));
                    _realAccuracy -= activePlayer.InnateTraits.Accuracy / 100f;
//                    Debug.Log(_realAccuracy);
                }
            }

//            yield return new WaitForSeconds(1);
//        }
    }

    public void Update()
    {
        _mainCapsulePlayers = FindObjectsOfType<MainCapsulePlayer>();

        _rtPool.RefreshQueue();

        if (_mainCapsulePlayers.Count(item => item.ThisInstanceReady) > 0)
        {
            MainCapsulePlayer activePlayer = _mainCapsulePlayers.First(item => item.ThisInstanceReady);
            Bounds bn = activePlayer.GetComponent<Collider>().bounds;
            Vector3 topShiftCoordinates = new Vector3(0, bn.size.y + accuracyCanvas.rect.height / 2, 0);
            MainCapsulePlayer[] nonActivePlayers = _mainCapsulePlayers.Where(item => !item.ThisInstanceReady).ToArray();

            foreach (MainCapsulePlayer capsule in nonActivePlayers)
            {

//                Debug.Log(activePlayer.playerSquare + " " + _sMath.CellCenterToPointXZ(activePlayer.playerSquare) + " " + _sMath.CellCenterToPointXZ(capsule.playerSquare));
                //                float realDistance = Mathf.Abs(capsule.playerSquare.x - activePlayer.playerSquare.x) + Mathf.Abs(capsule.playerSquare.z - activePlayer.playerSquare.z);
//                float realDistance = _ac.GetStraightLineAccuracy(_sMath.CellCenterToPointXZ(activePlayer.playerSquare), _sMath.CellCenterToPointXZ(capsule.playerSquare));
                float realDistance = _realAccuracy;

                int showPercentage = Convert.ToInt32((1 - realDistance) * 100);
                RectTransform can = _rtPool.Dequeue();
                can.transform.SetParent(transform);
                can.transform.position = capsule.playerSquare + topShiftCoordinates;

                TextMeshProUGUI tx = can.GetComponentInChildren<TextMeshProUGUI>();

                if (realDistance <= _cc.RangedWeaponDistance)
                {
                    can.gameObject.SetActive(true);
                    tx.text = showPercentage.ToString("D") + (showPercentage >= 100 ? "" : "%");
                }
                else
                {
                    can.gameObject.SetActive(false);
                }
            }

            _lastActivePlayer = activePlayer;
        } 
        else if (_lastActivePlayer != null)
        {
            _rtPool.SetActive(false);

            _lastActivePlayer = null;
        }

    }
}
