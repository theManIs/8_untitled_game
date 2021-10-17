
using UnityEngine;

public class ConstantConstraints : MonoBehaviour
{
    public Vector3 OneCell;
    public Vector3 OneCellExtents = new Vector3(.5f, 0, .5f);
    public Vector3 OneCellVerticalRayBar = new Vector3(0, 100, 0);
    public float MaxStepHeight = .5f;
    public float MaxStepDistance = 1;
    public int BaseMovementRange = 3;
    public float MapSizeX = 32;
    public float MapSizeZ = 32;
    public float MinPointX = -15;
    public float MinPointZ = -18;
}
