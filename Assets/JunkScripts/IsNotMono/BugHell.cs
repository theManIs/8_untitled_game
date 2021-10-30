using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugHell
{
    public static string ShowV3(HashSet<Vector3> h3)
    {
        string debubMessage = "";
        int iterator = 0;

        foreach (Vector3 vector3 in h3)
        {
            debubMessage += $"{iterator} {vector3} \n";
            iterator++;
        }

        return debubMessage;
    }

    public static string ShowV3(List<Vector3> h3)
    {
        string debubMessage = "";
        int iterator = 0;

        foreach (Vector3 vector3 in h3)
        {
            debubMessage += $"{iterator} {vector3} \n";
            iterator++;
        }

        return debubMessage;
    }
}