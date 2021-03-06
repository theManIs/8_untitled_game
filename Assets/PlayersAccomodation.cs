using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersAccomodation : MonoBehaviour
{
    public static List<MainCapsulePlayer> ListOfPlayers = new List<MainCapsulePlayer>();

    public static List<Vector3> GetSquarePositions()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (MainCapsulePlayer mainCapsulePlayer in ListOfPlayers)
        {
            positions.Add(mainCapsulePlayer.playerSquare);
        }

        return positions;
    }

    public static void AddPlayer(MainCapsulePlayer mcp)
    {
        HighlitingAccuracy ac = FindObjectOfType<HighlitingAccuracy>();

        mcp.AccuracyRecount += ac.ChangeAccuracyRoutine;

        foreach (MainCapsulePlayer mcpExisted in ListOfPlayers)
        {
            mcp.CheckInInstance += mcpExisted.InstanceCheckOut;
            mcpExisted.CheckInInstance += mcp.InstanceCheckOut;
        }

        ListOfPlayers.Add(mcp);
    }

    public static void DelPlayer(MainCapsulePlayer mcp)
    {
        HighlitingAccuracy ac = FindObjectOfType<HighlitingAccuracy>();

        mcp.AccuracyRecount -= ac.ChangeAccuracyRoutine;

        foreach (MainCapsulePlayer mcpExisted in ListOfPlayers)
        {
            mcp.CheckInInstance -= mcpExisted.InstanceCheckOut;
            mcpExisted.CheckInInstance -= mcp.InstanceCheckOut;
        }

        ListOfPlayers.Remove(mcp);
    }
}
