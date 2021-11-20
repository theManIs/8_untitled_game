using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateFuckingTrash
{
    public void RotateToMovementDirection(Transform originTransform, Vector3 targetPoint)
    {
        Vector3 newTargetPoint = new Vector3(targetPoint.x, originTransform.position.y, targetPoint.z);
        Vector3 backwardRotation = newTargetPoint - originTransform.position;
        Quaternion rot = Quaternion.FromToRotation(originTransform.forward, backwardRotation);
//        Vector3 eulers = rot.eulerAngles;
//        eulers.x = eulers.z = 0;
//        rot = Quaternion.Euler(eulers);
//        Debug.Log(backwardRotation + " " + newTargetPoint + " " + originTransform.position + " " + rot.eulerAngles);
        originTransform.Rotate(Vector3.up, rot.eulerAngles.y);
    }
}
