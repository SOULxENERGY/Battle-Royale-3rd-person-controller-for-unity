using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;



public class CmCameraSwitch : MonoBehaviour
{
    public static CmCameraSwitch singleton;
    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera airPlaneCam;
    [SerializeField] private CinemachineCamera playerThirdPersonCam;

    public CinemachineCamera AirPlaneCam => airPlaneCam;
    public CinemachineCamera PlayerThirdPersonCam => playerThirdPersonCam;



    [Header("Priority Settings")]
    [SerializeField] private int activePriorityValue = 10;
    [SerializeField] private int inactivePriorityValue = 0;

    private CinemachineCamera currentCam;

    private void Awake()
    {
        singleton=this;
    }
    void Start()
    {
        // Safety check + initial switch
        if (airPlaneCam != null)
        {
            currentCam = airPlaneCam;
        }
        else
        {
            Debug.LogWarning("Airplane camera not assigned.");
        }
    }

    public void SwitchCmCam(CinemachineCamera newCam,Transform trackingTarget)
    {

        if (newCam == null & currentCam==newCam)
        {
            Debug.LogWarning("Attempted to switch to a null camera.");
            return;
        }

        if (currentCam != null)
        {
            currentCam.Priority = inactivePriorityValue;
        }
        if (newCam.Target.TrackingTarget == null)
        {
            newCam.Target.TrackingTarget = trackingTarget;
        }
        

        newCam.Priority = activePriorityValue;
        currentCam = newCam;
    }
}

