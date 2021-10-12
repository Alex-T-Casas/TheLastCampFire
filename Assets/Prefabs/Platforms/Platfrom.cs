using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platfrom : MonoBehaviour, Toggleable
{
    [SerializeField] Transform objectToMove;
    [SerializeField] float transitionTime;
    Coroutine MoveingCoroutine;

    public Transform StartTrans;
    public Transform EndTrans;

    public void ToggleOn()
    {
        MoveTo(true);
    }

    public void ToggleOff()
    {
        MoveTo(false);
    }

    public void MoveTo(bool ToEnd)
    {
        if(ToEnd)
        {
            MoveTo(EndTrans);
        }else
        {
            MoveTo(StartTrans);
        }
    }

    public void MoveTo(Transform Destination)
    {
        if(MoveingCoroutine != null)
        {
            StopCoroutine(MoveingCoroutine);
            MoveingCoroutine = null;
        }
        MoveingCoroutine = StartCoroutine(MoveToTrans(Destination, transitionTime));
    }

    IEnumerator MoveToTrans(Transform Destination, float TransitionTime)
    {
        Vector3 StartPos = transform.position;
        Vector3 EndPos = Destination.position;
        Quaternion StartRot = transform.rotation;
        Quaternion EndRot = Destination.rotation;

        float timmer = 0f;
        while(timmer < TransitionTime)
        {
            timmer += Time.deltaTime;
            objectToMove.position = Vector3.Lerp(StartPos, EndPos, timmer / TransitionTime);
            objectToMove.rotation = Quaternion.Lerp(StartRot, EndRot, timmer / TransitionTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
