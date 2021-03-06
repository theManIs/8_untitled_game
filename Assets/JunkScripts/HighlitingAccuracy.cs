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
//    private ConstantConstraints _cc;
    private LevelDissectorPlain _ldp;
    private AccuracyCounter _ac = new AccuracyCounter();
    private StaticMath _sMath = new StaticMath();

    public void OnEnable()
    {
        _mainCapsulePlayers = FindObjectsOfType<MainCapsulePlayer>();
        _rtPool = new PoolOfRectTransform();
        _rtPool.SetCanonicalRect(accuracyCanvas);
//        _cc = FindObjectOfType<ConstantConstraints>();
        _ldp = FindObjectOfType<LevelDissectorPlain>();
        //        Debug.Log(_ldp.GetLevelDissected(_cc.LevelBounds));
        _ac = new AccuracyCounter { Cells = _ldp.GetLevelDissected(_ldp.LevelMins, _ldp.LevelBounds) };

        StartCoroutine(RecountAccuracyCounter());
    }

//    public void ChangeAccuracyRoutineDeprecated()
////    public IEnumerator ChangeAccuracyRoutine()
//    {
//        Debug.Log("ChangeAccuracyRoutine");
////        for (;;)
////        {
////            _mainCapsulePlayers = FindObjectsOfType<MainCapsulePlayer>();
//        _mainCapsulePlayers = PlayersAccomodation.ListOfPlayers.ToArray();
//
//            if (_mainCapsulePlayers.Count(item => item.ThisInstanceReady) > 0)
//            {
//                MainCapsulePlayer activePlayer = _mainCapsulePlayers.First(item => item.ThisInstanceReady);
//                MainCapsulePlayer[] nonActivePlayers = _mainCapsulePlayers.Where(item => !item.ThisInstanceReady).ToArray();
//
//                foreach (MainCapsulePlayer capsule in nonActivePlayers)
//                {
//                    _realAccuracy = _ac.GetStraightLineAccuracy(_sMath.CellCenterToPointXZ(activePlayer.playerSquare), _sMath.CellCenterToPointXZ(capsule.playerSquare));
//                    _realAccuracy -= activePlayer.InnateTraits.Accuracy / 100f;
//                    capsule.TemporaryAccuracy = GetAccuracyPercentage(_realAccuracy);
////                    Debug.Log(_realAccuracy);
//                }
//            }
//
////            yield return new WaitForSeconds(1);
////        }
//    }

    public float GetAccuracyPercentage(float realDistance)
    {
        return Convert.ToInt32((1 - realDistance) * 100);
    }

    public IEnumerator RecountAccuracyCounter()
    {
        for (;;)
        {
            yield return new WaitForSeconds(.05f);

            if (_ac.Cells.Length == 0)
            {
                _ac = new AccuracyCounter { Cells = _ldp.GetLevelDissected(_ldp.LevelMins, _ldp.LevelBounds) };
//                Debug.Log("RecountAccuracyCounter");
                break;
            }
        }

    }

    public float CountAccuracyInternal(MainCapsulePlayer activePlayer, MainCapsulePlayer capsule)
    {
        return capsule.InnateTraits.CountAccuracy(capsule.CountObstacleAccuracy(activePlayer, capsule));
    }

    public void ChangeAccuracyRoutine()
    {
//        Debug.Log("ChangeAccuracyRoutine");
        _mainCapsulePlayers = PlayersAccomodation.ListOfPlayers.ToArray();
//        _mainCapsulePlayers = FindObjectsOfType<MainCapsulePlayer>();

        _rtPool.RefreshQueue();

        
        

        if (_mainCapsulePlayers.Count(item => item.ThisInstanceReady) > 0)
        {
            MainCapsulePlayer activePlayer = _mainCapsulePlayers.First(item => item.ThisInstanceReady);
            Bounds bn = activePlayer.GetComponent<Collider>().bounds;
            Vector3 topShiftCoordinates = new Vector3(0, bn.size.y + accuracyCanvas.rect.height / 2, 0);
            MainCapsulePlayer[] nonActivePlayers = _mainCapsulePlayers.Where(item => !item.ThisInstanceReady).ToArray();

            if (activePlayer)
            {
                RectTransform can = _rtPool.Dequeue();
                can.transform.position = activePlayer.playerSquare + topShiftCoordinates;
                HealthAndAccuracy ha = can.GetComponent<HealthAndAccuracy>();
                ha.On(false);
                ha.SetAction(activePlayer.InnateTraits.RuntimeActionPoints);
                can.gameObject.SetActive(true);
            }

            foreach (MainCapsulePlayer capsule in nonActivePlayers)
            {
//                Debug.Log(activePlayer.playerSquare + " " + _sMath.CellCenterToPointXZ(activePlayer.playerSquare) + " " + _sMath.CellCenterToPointXZ(capsule.playerSquare));
//                float realDistance = capsule.InnateTraits.CountDistance(activePlayer.playerSquare, capsule.playerSquare);
//                float realDistance = _ac.GetStraightLineAccuracy(_sMath.CellCenterToPointXZ(activePlayer.playerSquare), _sMath.CellCenterToPointXZ(capsule.playerSquare));
//                float realDistance = _realAccuracy;
                float showPercentage = CountAccuracyInternal(activePlayer, capsule);

//                int showPercentage = Convert.ToInt32((1 - realDistance) * 100);
//                Debug.Log(capsule.name + " " + showPercentage.ToString("F0") + (showPercentage >= 100 ? "" : "%"));
//                if (capsule.TemporaryHideAccuracy)
//                {
//                    can.gameObject.SetActive(false);
//                }
//                else
//                {
                    if (activePlayer.InnateTraits.CheckDistance(activePlayer.playerSquare, capsule.playerSquare) && !capsule.TemporaryHideAccuracy && activePlayer.InnateTraits.IsEnemy(capsule.InnateTraits))
                    {
                        RectTransform can = _rtPool.Dequeue();
        //                can.transform.SetParent(transform);
                        can.transform.position = capsule.playerSquare + topShiftCoordinates;

//                        TextMeshProUGUI tx = can.GetComponentInChildren<TextMeshProUGUI>();
                        HealthAndAccuracy ha = can.GetComponent<HealthAndAccuracy>();
                        can.gameObject.SetActive(true);
                        
//                        tx.text = showPercentage.ToString("F0") + (showPercentage >= 100 ? "" : "%");
                        ha.SetHealth(capsule.InnateTraits.TemporaryHealth, capsule.InnateTraits.Health);
                        ha.SetAccuracy(showPercentage.ToString("F0") + (showPercentage >= 100 ? "" : "%"));
                        ha.On(true);
                    }
//                    else
//                    {
//                        can.gameObject.SetActive(false);
//                    }
//                }
            }

            _rtPool.SetActive(false);

            _lastActivePlayer = activePlayer;
        } 
        else if (_lastActivePlayer != null)
        {
            _rtPool.SetActive(false);

            _lastActivePlayer = null;
        }

    }
}
