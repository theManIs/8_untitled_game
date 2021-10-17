using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDissectorPlain : MonoBehaviour
{
    public Transform MainGridBox;
    public Vector3 GridBounds = Vector3.zero;

    void OnEnable()
    {
        MeshRenderer mr = MainGridBox.GetComponent<MeshRenderer>();
        GridBounds = mr.bounds.size;


    }
}
