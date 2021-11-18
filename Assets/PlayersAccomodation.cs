using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersAccomodation : MonoBehaviour
{
    public static List<MainCapsulePlayer> ListOfPlayers = new List<MainCapsulePlayer>();

    public static void AddPlayer(MainCapsulePlayer mcp)
    {
        HighlitingAccuracy ac = FindObjectOfType<HighlitingAccuracy>();

        mcp.AccuracyRecount += ac.ChangeAccuracyRoutine;
    }
}
