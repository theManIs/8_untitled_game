
using UnityEngine;

public class ConstantConstraints : MonoBehaviour
{
    public Vector3 OneCell;
    public Vector3 OneCellExtents = new Vector3(.5f, 0, .5f);
    public Vector3 OneCellVerticalRayBar = new Vector3(0, 100, 0);
    public int OneCellStep = 1;
    public float MaxStepHeight = .5f;
    public float MaxStepDistance = 1;
    public int BaseMovementRange = 3;
    public float RangedWeaponDistance = 5;
    public float MapSizeX = 32;
    public float MapSizeZ = 32;
    public float MinX = -15;
    public float MinZ = -18;
    public float MaxX = 17;
    public float MaxZ = 14;
    public Vector3 LevelBounds = new Vector3(32, 10, 32);
    public Vector3 LevelMins = new Vector3(-15, 0, -18);
}
