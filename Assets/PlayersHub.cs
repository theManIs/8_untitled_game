using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersHub : MonoBehaviour
{
    public MainCapsulePlayer[] MainCapsulePlayers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MainCapsulePlayers = FindObjectsOfType<MainCapsulePlayer>();

        foreach (MainCapsulePlayer capsulePlayer in MainCapsulePlayers)
        {
            StaticMath smath = new StaticMath();
            MeshRenderer mr = capsulePlayer.GetComponent<MeshRenderer>();

            capsulePlayer.playerSquare = smath.PointToCellCenterXZ(capsulePlayer.transform.position, mr.bounds.extents);
        }
    }
}
