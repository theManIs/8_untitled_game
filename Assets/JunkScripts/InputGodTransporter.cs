using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGodTransporter : MonoBehaviour
{
    public float Hr => Input.GetAxis("Horizontal");
    public float Vr => Input.GetAxis("Vertical");
}
