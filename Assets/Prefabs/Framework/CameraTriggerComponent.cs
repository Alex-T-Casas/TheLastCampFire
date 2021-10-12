using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerComponent : MonoBehaviour
{
    [SerializeField] float TransitionTime = 1.0f;
    CameraTransition cameraTransition;

    private void Start()
    {
        cameraTransition = GetComponent<CameraTransition>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControler>() != null)
        {
            cameraTransition.ChangeToCamera(11, TransitionTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerControler>() != null)
        {
            cameraTransition.ChangeToCamera(9, TransitionTime);
        }
    }
}
