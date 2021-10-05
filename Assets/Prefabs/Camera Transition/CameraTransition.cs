using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTransition : MonoBehaviour
{
    [SerializeField] float TransitionTime = 1.0f;
    [SerializeField] CinemachineVirtualCamera DestinationCam;
    [SerializeField] CinemachineBrain cinemachineBrain;

    private void Start()
    {
        Camera.main.GetComponent<CinemachineBrain>(); 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControler>() != null)
        {
            cinemachineBrain.m_DefaultBlend.m_Time = TransitionTime;
            DestinationCam.Priority = 11;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerControler>() != null)
        {
            cinemachineBrain.m_DefaultBlend.m_Time = TransitionTime;
            DestinationCam.Priority = 9;
        }
    }
}
