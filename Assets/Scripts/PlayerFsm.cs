using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.IO;

public class PlayerFsm : MonoBehaviour
{

    public static PlayerFsm singleton;
    private FSM<PlayerFsm> fsm;
    public PlayerJumpingDownState playerJumpingDownState = new PlayerJumpingDownState();
    public PlayerParachuteState playerParachuteState= new PlayerParachuteState();
    public PlayerMovementState playerMovementState = new PlayerMovementState();
    public PlayerJumpState playerJumpState = new PlayerJumpState();
   
    public Transform lookAtObjectOfThirdPersonCamera;
    public Animator playerAnimator;
    public PhysicsBasedVelocityCalculator physicsBasedVelocityCalculator;
    public Transform actual3dModelOfPlayer;
    public CharacterController cc;
    public event Action parachuteEvent;
    public event Action jumpEvent;
    public PlayerInput playerInput;
    public AudioSource audioSrc;
    [SerializeField] private UIDocument uiDoc;
    public Vector3Field velocityFieldUi;
    public Vector3Field inputFieldUi;
    public Label skyDiveCount;
    public Inventory inventory1;











    private void Awake()
    {
        singleton = this;
    }
    void Start()
    {
       
        //instantiating state machine
        fsm = new FSM<PlayerFsm>();
       
        //Activating initial state 
        fsm.ChangeState(playerJumpingDownState, this);



        //ui tookit related
        //getting ref of ui doc
        uiDoc = GameObject.Find("UIDocument").GetComponent<UIDocument>();

        //getting ref of root element
        var root = uiDoc.rootVisualElement;
        

        //getting ref of velocityfieldui from uitookiteditor
        velocityFieldUi = root.Q<Vector3Field>("Velocity");

        //getting ref of skydivecountlabel from uitookiteditor
        skyDiveCount = root.Q<Label>("SkyDiveCount");

        //getting ref of velocityfieldui from uitookiteditor
        inputFieldUi = root.Q<Vector3Field>("Input");

       
        //reading inventory data and this method  will auto set the data to inventory 1 global field
        ReadInventoryData();

        //setting skydivecount from that inventory1 field data
        skyDiveCount.text = $"SkyDiveCount : {inventory1.NoOfTimesLandedOnGround} (saved as json in a file)";

        //enabling visibility of skydivedata ui
        skyDiveCount.style.display = DisplayStyle.Flex;




    }

    
    void Update()
    {
       //updating state machine
        fsm.Update(Time.deltaTime, this);

        //setting input value  to input ui
       inputFieldUi.value = new Vector3(playerInput.actions["Move"].ReadValue<Vector2>().x, 0, playerInput.actions["Move"].ReadValue<Vector2>().y);
    }

    void FixedUpdate()
    {
        fsm.FixedUpdate(Time.fixedDeltaTime, this);
    }

    public void InvokeParachuteEvent(InputAction.CallbackContext ctx)
    {
    
        if (parachuteEvent != null && ctx.performed)
        {
            parachuteEvent.Invoke();
        }
        
    }

    public void InvokeJumpEvent(InputAction.CallbackContext ctx)
    {
        if (jumpEvent != null & ctx.performed)
        {
            jumpEvent.Invoke();
        }
    }

    public void ChangeState(FsmBaseState<PlayerFsm>newState)
    {
        fsm.ChangeState(newState, this);
    }

    public Vector3 RoundVector3(Vector3 input)
    {

        return new Vector3(
        Mathf.RoundToInt(input.x ) ,
        Mathf.RoundToInt(input.y ) ,
        Mathf.RoundToInt(input.z ) 
    );
    }


    public void SaveInventoryData()
    {
        string json = JsonUtility.ToJson(inventory1);
        string path = Application.persistentDataPath + "/InventoryData.json";

        File.WriteAllText(path, json);
        
        skyDiveCount.text = $"SkyDiveCount : {inventory1.NoOfTimesLandedOnGround} (saved as json in a file)";
      
     
    }


    public void ReadInventoryData()
    {
        string path = Application.persistentDataPath + "/InventoryData.json";
        if (File.Exists(path))
        {
            Debug.Log("Updated inventory With Saved Data");
            string json = File.ReadAllText(path);
            inventory1 = JsonUtility.FromJson<Inventory>(json);
        }
        else
        {
            Debug.Log("no saved data available so intantialted brand new Inventory class");
            inventory1 = new Inventory();
        }
    }
}
