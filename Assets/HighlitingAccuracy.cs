using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighlitingAccuracy : MonoBehaviour
{
    public RectTransform accuracyCanvas;

    private MainCapsulePlayer[] _mainCapsulePlayers;
    private MainCapsulePlayer _lastActivePlayer;
    private PoolOfRectTransform _rtPool;
    

    public void OnEnable()
    {
        _mainCapsulePlayers = FindObjectsOfType<MainCapsulePlayer>();
        _rtPool = new PoolOfRectTransform();
        _rtPool.SetCanonicalRect(accuracyCanvas);
    }

    public void Update()
    {
        _mainCapsulePlayers = FindObjectsOfType<MainCapsulePlayer>();

        _rtPool.RefreshQueue();

        if (_mainCapsulePlayers.Count(item => item.ThisInstanceReady) > 0)
        {

            MainCapsulePlayer activePlayer = _mainCapsulePlayers.First(item => item.ThisInstanceReady);
            Bounds bn = activePlayer.GetComponent<MeshRenderer>().bounds;
            Vector3 topShiftCoordinates = new Vector3(0, bn.extents.y + accuracyCanvas.rect.height / 2, 0);
            MainCapsulePlayer[] nonActivePlayers = _mainCapsulePlayers.Where(item => !item.ThisInstanceReady).ToArray();

            foreach (MainCapsulePlayer capsule in nonActivePlayers)
            {
                float realDistance = Mathf.Abs(capsule.playerSquare.x - activePlayer.playerSquare.x) + Mathf.Abs(capsule.playerSquare.z - activePlayer.playerSquare.z);

                RectTransform can = _rtPool.Dequeue();
                can.transform.SetParent(transform);
                can.transform.position = capsule.playerSquare + topShiftCoordinates;

                TextMeshProUGUI tx = can.GetComponentInChildren<TextMeshProUGUI>();

                if (realDistance <= 3)
                {
                    can.gameObject.SetActive(true);
                    tx.text = realDistance.ToString("N");
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
