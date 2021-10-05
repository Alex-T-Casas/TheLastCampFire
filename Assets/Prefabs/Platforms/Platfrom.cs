using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platfrom : MonoBehaviour
{
    [SerializeField] Transform objectToMove;
    [SerializeField] float transitionTime;
    Coroutine MoveingCoroutine;

    public Transform StartTrans;
    public Transform EndTrans;
    // Start is called before the first frame update
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
        float timmer = 0f;
        while(timmer < TransitionTime)
        {
            timmer += Time.deltaTime;
            objectToMove.position = Vector3.Lerp(objectToMove.position, Destination.position, timmer / TransitionTime);
            objectToMove.rotation = Quaternion.Lerp(objectToMove.rotation, Destination.rotation, timmer / TransitionTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
