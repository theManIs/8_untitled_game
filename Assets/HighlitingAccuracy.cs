using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighlitingAccuracy : MonoBehaviour
{
    public RectTransform accuracyCanvas;
    
    private PlayersHub playersHub;
    private Queue<RectTransform> _canvasRow = new Queue<RectTransform>();
    private Queue<RectTransform> _canvasRowNew = new Queue<RectTransform>();
    

    // Start is called before the first frame update
    public void OnEnable()
    {
        playersHub = FindObjectOfType<PlayersHub>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playersHub.MainCapsulePlayers.Count(item => item.ThisInstanceReady) > 0)
        {
            if (_canvasRowNew.Count > 0)
            {
                _canvasRow = new Queue<RectTransform>(_canvasRowNew);
                _canvasRowNew.Clear();
            }

            MainCapsulePlayer activePlayer = playersHub.MainCapsulePlayers.First(item => item.ThisInstanceReady);
            HashSet<Vector3> movementRange = activePlayer.MovementSet;
            MainCapsulePlayer[] nonActivePlayers = playersHub.MainCapsulePlayers.Where(item => !item.ThisInstanceReady).ToArray();
//            MainCapsulePlayer[] nonActivePlayers = playersHub.MainCapsulePlayers.Where(item => movementRange.Contains(item.playerSquare)).ToArray();

            foreach (MainCapsulePlayer capsule in nonActivePlayers)
            {
                float realDistance = Mathf.Abs(capsule.playerSquare.x - activePlayer.playerSquare.x) + Mathf.Abs(capsule.playerSquare.z - activePlayer.playerSquare.z);

                if (realDistance <= 3)
                {
                    RectTransform can;

                    if (_canvasRow.Count > 0)
                    {

                        can = _canvasRow.Dequeue();
                        _canvasRowNew.Enqueue(can);
                    }
                    else
                    {
                        can = Instantiate(accuracyCanvas, capsule.playerSquare + new Vector3(0, 1, 0), accuracyCanvas.rotation);
                        _canvasRowNew.Enqueue(can);
                    }

                    TextMeshProUGUI tx = can.GetComponentInChildren<TextMeshProUGUI>();
                    tx.text = realDistance.ToString("N");


//                    Debug.Log(capsule.name + " " + capsule.playerSquare + " " + realDistance);
                }

            }
        }
    }
}
