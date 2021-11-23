using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float DestroyTime = 3f;
    public Vector3 DefaultOffset = new Vector3(0, .5f, 0);
    public Vector3 RandomizeIntensity = new Vector3(.1f, .1f, 0);

    private TextMeshPro _tmp;

    public void OnEnable()
    {
        transform.position += DefaultOffset;
        transform.position += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x), 0, 0);
        _tmp = GetComponentInChildren<TextMeshPro>();
        _tmp.color = Random.ColorHSV();
        Destroy(gameObject, DestroyTime);
    }

    public void SetText(string text)
    {
        _tmp.text = text;
    }

    public void SetWidth(Vector2 xySize)
    {
        transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = xySize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
