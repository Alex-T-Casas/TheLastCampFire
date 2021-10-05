using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp : MonoBehaviour
{
    [SerializeField] Transform objectToMove;
    [SerializeField] float transitionTime;
    Coroutine MoveingCoroutine;

    public Transform StartTrans;
    public Transform EndTrans;
    // Start is called before the first frame update

    /*private void OnTriggernEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            other.transform.SetParent(null);
        }
    }*/

    public void MoveTo(Transform Dest)
    {
        if(MoveingCoroutine != null)
        {
            StopCoroutine(MoveingCoroutine);
            MoveingCoroutine = null;
        }
        MoveingCoroutine = StartCoroutine(MoveToTrans(Dest, transitionTime));
    }

    IEnumerator MoveToTrans(Transform Dest, float TransitionTime)
    {
        float timmer = 0f;
        while(timmer < TransitionTime)
        {
            timmer += Time.deltaTime;
            objectToMove.position = Vector3.Lerp(objectToMove.position, Dest.position, timmer / TransitionTime);
            objectToMove.rotation = Quaternion.Lerp(objectToMove.rotation, Dest.rotation, timmer / TransitionTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
