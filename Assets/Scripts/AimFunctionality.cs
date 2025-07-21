using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;

public class AimFunctionality : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     private InputSystem_Actions inputSystemAction;
    [SerializeField] private Transform lookAt;
    private float delRotX;
    private float delRotY;
    
    [SerializeField] private float maxRotY;
    [SerializeField] private float sensitivityX=1;
    [SerializeField] private float sensitivityY=1;
    private Vector2 cachedInput;

    public enum CameraControlMode
    {
        Freelook,
        FollowPlayer
    }


    void Start()
    {
        inputSystemAction = new InputSystem_Actions();
        inputSystemAction.Player.Enable();
       
    }

    // Update is called once per frame
    void Update()
    {


        cachedInput += inputSystemAction.Player.Look.ReadValue<Vector2>();




    }
    private void FixedUpdate()
    {
        Vector2 delVector = cachedInput;
        cachedInput = Vector2.zero;



        delRotX += delVector.x * sensitivityX;

        delRotY += delVector.y * sensitivityY;

        delRotY = Mathf.Clamp(delRotY, -maxRotY, maxRotY);
        if (lookAt)
        {
            lookAt.transform.localEulerAngles = new Vector3(-delRotY, 0, 0);
            
            lookAt.transform.parent.localEulerAngles = new Vector3(0, delRotX, 0);
           
        }

        
    }
}
