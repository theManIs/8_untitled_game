using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlayerPositioning : MonoBehaviour
{
    public MainCapsulePlayer StartingWorkpiece;
    public Vector3[] PlayerInstances;
    public string[] PlayerNames;

    private ConstantConstraints _cc;

    // Start is called before the first frame update
    void Start()
    {
        _cc = FindObjectOfType<ConstantConstraints>();
        StaticMath smath = new StaticMath();
        int iterator = 0;

        foreach (Vector3 playerInstance in PlayerInstances)
        {
            Transform trn = Instantiate(StartingWorkpiece.transform, playerInstance, StartingWorkpiece.transform.rotation);
            trn.gameObject.SetActive(true);
            Vector3 verticalAlign = trn.position;

            verticalAlign = smath.PointToCellCenterXZ(verticalAlign, _cc.OneCellExtents);

            if (Physics.Raycast(new Ray(trn.position + _cc.OneCellVerticalRayBar, Vector3.down), out RaycastHit hitinfo))
            {
                verticalAlign.y = hitinfo.point.y + 1;

                trn.position = verticalAlign;
            }

            if (PlayerNames.Length > iterator)
            {
                trn.name = PlayerNames[iterator];
            }

            iterator++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
