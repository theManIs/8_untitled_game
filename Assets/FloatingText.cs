using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float DestroyTime = 3f;
    public Vector3 DefaultOffset = new Vector3(0, .5f, 0);
    public Vector3 RandomizeIntensity = new Vector3(.5f, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        transform.position += DefaultOffset;
        transform.position += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x), 0, 0);
        GetComponentInChildren<TextMeshPro>().color = Random.ColorHSV();
        Destroy(gameObject, DestroyTime);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
