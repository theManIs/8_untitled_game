using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeReciever : MonoBehaviour
{
    private MeshRenderer _defaultMaterial;
    private Color _defaultColor;
    private PlayerRequestOrder _playerRequestOrder;

    void Start()
    {
        _defaultMaterial = GetComponent<MeshRenderer>();
        _defaultColor = _defaultMaterial.material.color;
        _playerRequestOrder = FindObjectOfType<PlayerRequestOrder>();
    }

    void OnMouseDown()
    {
        _playerRequestOrder.NewMove = true;
        _playerRequestOrder.MoveToClick = transform.position;
    }

    void OnMouseOver()
    {
        _defaultMaterial.material.color = Color.blue;
    }

    void OnMouseExit()
    {
        _defaultMaterial.material.color = _defaultColor;
    }
}
