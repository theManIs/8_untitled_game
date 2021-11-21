using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlayerPositioning : MonoBehaviour
{
    public CharacterInnateTraits[] CharacterInnateTraits;
    public LevelDissectorPlain Ldp;

    private bool _initOnce = false;

    // Start is called before the first frame update
    void LocatePlayers()
    {
//        _cc = FindObjectOfType<ConstantConstraints>();
        StaticMath smath = new StaticMath();
        int iterator = 0;

        foreach (CharacterInnateTraits cit in CharacterInnateTraits)
        {
            Transform trn = Instantiate(cit.StartingInstance.transform, cit.PositionToInstantiate, cit.StartingInstance.transform.rotation);
            trn.gameObject.SetActive(true);
            Vector3 verticalAlign = trn.position;
            trn.GetComponent<MainCapsulePlayer>().SetInnateTraits(cit);
            cit.Recount();

            verticalAlign = smath.PointToCellCenterXZ(verticalAlign, Ldp.OneCellExtents);

            if (Physics.Raycast(new Ray(trn.position + Ldp.OneCellVerticalRayBar, Vector3.down), out RaycastHit hitinfo))
            {
                verticalAlign.y = hitinfo.point.y + 1;

                trn.position = verticalAlign;
            }

            trn.name = cit.UnitName;

            iterator++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_initOnce)
        {
            if (Ldp.AllCellsInOneArray.Length != 0)
            {
                LocatePlayers();

                _initOnce = true;
            }
        }
    }
}
