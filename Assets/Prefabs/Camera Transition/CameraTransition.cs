using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTransition : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera DestinationCam;
    [SerializeField] CinemachineBrain cinemachineBrain;

    private void Start()
    {
        Camera.main.GetComponent<CinemachineBrain>(); 
    }

    public void ChangeToCamera(int pryority, float time)
    {
        cinemachineBrain.m_DefaultBlend.m_Time = time;
        DestinationCam.Priority = pryority;
    }
}
