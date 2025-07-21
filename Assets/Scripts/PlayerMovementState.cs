using UnityEngine;
using Unity.Cinemachine;

[System.Serializable]
public class PlayerMovementState : FsmBaseState<PlayerFsm>
{
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private Vector3 lookAtLocalPos;
    [SerializeField] private float controlvelo = 20f;
    [SerializeField] private float dynamicFrictionCoeff=0.3f;
    [SerializeField] private float staticFrictionCoeff = 0.9f;
    [SerializeField] private float dragCoeff = 0.9f;
    
    private bool isControlledVelocityexisting = false;
    [SerializeField] private float maxSlopePlayerCanWalk = 20;
    [SerializeField] private AudioClip landMp3;
    private bool isLandMp3Played = false;



    public override void EnterState(PlayerFsm context)
    {
        if (isLandMp3Played == false )
        {
            CinemachineImpulseSource impulseSrc = context.transform.GetComponent<CinemachineImpulseSource>();
            if (impulseSrc)
            {
                impulseSrc.GenerateImpulse();
            }
          
            if (!landMp3)
            {
                
                return;
            }
            isLandMp3Played = true;
            context.audioSrc.loop = false;
            context.audioSrc.clip = landMp3;
            context.audioSrc.Play();
            context.inventory1.Increase();
            
            context.SaveInventoryData();
        }
        context.jumpEvent += GoToJumpState;

        //playing animation for this state
        if (!context.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Movemnt"))
        {
            context.playerAnimator.Play("Movemnt");
        }
      

        //switching suitable cinemachine camra
        CmCameraSwitch.singleton.SwitchCmCam(
           CmCameraSwitch.singleton.PlayerThirdPersonCam,
           context.lookAtObjectOfThirdPersonCamera
       );
        CmCameraSwitch.singleton.PlayerThirdPersonCam.Lens.FarClipPlane = 1000f;

        

        
        
    }

    public override void FixedUpdateState(float fixedDeltaTime, PlayerFsm context)
    {
        //getting reference of physiscs simulator
        var pvc = context.physicsBasedVelocityCalculator;
        //setting continuous forces 
        pvc.SetContinuousForces(GetContinuousForces(pvc.PhysicsVelocity));
   
        //checking if player is on ground
        if (GroundChecker.singleton.IsGrounded)
        {

           //getting input value
            Vector2 inputMoveVector = context.playerInput.actions["Move"].ReadValue<Vector2>();

            //rotating/modifying input values to transform it into move direction
            Vector3 controlDir = context.transform.rotation * new Vector3(inputMoveVector.x, 0, inputMoveVector.y).normalized;
          
            //checking if player trying to control the character
            if (controlDir.magnitude > 0)
            {
                isControlledVelocityexisting = true;
            }
            else
            {
                isControlledVelocityexisting = false;
            }

            //projecting movedir on slope of the terrain
            Vector3 controlDirProjectedOnGround = Vector3.ProjectOnPlane(controlDir, GroundChecker.singleton.GroundNormal);
            


            if (controlDirProjectedOnGround.y > 0) 
            {
                //going uphill;
              
                if(Vector3.Angle(controlDir, controlDirProjectedOnGround) > maxSlopePlayerCanWalk)
                {
                    //too much slope for player to move
                    
                    //pushing the player down along the slope
                    float pushingDownMag = 10f;

                    Vector3 physicsVeloProjectedOnGround = Vector3.Project(pvc.PhysicsVelocity, controlDirProjectedOnGround.normalized*-1);
                    if(Vector3.Dot(physicsVeloProjectedOnGround.normalized, controlDirProjectedOnGround.normalized * -1) > 0.99 && physicsVeloProjectedOnGround.magnitude>=pushingDownMag)
                    {
                       //it has already enough downward velocity;
                    }
                    else
                    {
                        
                        pvc.PhysicsVelocity += controlDirProjectedOnGround.normalized * -(pushingDownMag-physicsVeloProjectedOnGround.magnitude);
                    }
                   
                    

                    
                    controlDirProjectedOnGround = Vector3.zero;
                   
                }
                
            }

            //calculating net velocity
            Vector3 netvelocity = pvc.PhysicsVelocity + controlDirProjectedOnGround * controlvelo;
            
            //moving character controller
            context.cc.Move(netvelocity * fixedDeltaTime);


            //chnaging world velocty to local to control animator blend tree
            Vector3 localVelocity = context.transform.InverseTransformDirection(netvelocity);
            //changing local velocity in a 2d form
            
            Vector2 finalAnimationVector = new Vector2(localVelocity.x, localVelocity.z);
            Vector2 finalAnimationDir;

            float velocityAboveWhichRunningAnimationWillBePlayed = 3f;
            if (finalAnimationVector.magnitude >= velocityAboveWhichRunningAnimationWillBePlayed)
            {
                //running animation
               finalAnimationDir = finalAnimationVector.normalized;
            }
            else
            {
                //walking animation
                finalAnimationDir = finalAnimationVector / velocityAboveWhichRunningAnimationWillBePlayed;
            }

           
         

            
           
                context.playerAnimator.SetFloat("veloX", finalAnimationDir.x);
                context.playerAnimator.SetFloat("veloY", finalAnimationDir.y);
          
            

            context.velocityFieldUi.value = context.transform.InverseTransformDirection( context.RoundVector3(netvelocity));

            

        }
        else
        {
            
            //if pl;ayer not on ground only physics based velocty will be applied not control velocity generated from player input
            context.cc.Move(pvc.PhysicsVelocity * fixedDeltaTime);
            context.velocityFieldUi.value = pvc.PhysicsVelocity;
            
        }

        
    }

    public override void LeaveState(PlayerFsm context)
    {
        context.jumpEvent -= GoToJumpState;
    }

    public override void UpdateState(float deltaTime, PlayerFsm context)
    {
        context.lookAtObjectOfThirdPersonCamera.localPosition = Vector3.Lerp(context.lookAtObjectOfThirdPersonCamera.localPosition, lookAtLocalPos, deltaTime * 10f);
    }

    private Vector3[] GetContinuousForces(Vector3 currentVelocity)
    {
        Vector3 gravityForce = new Vector3(0f, gravity, 0f);

        //normal force is component of gravity along normal of ground.
        Vector3 normalForce = Vector3.zero;

        if (GroundChecker.singleton.IsGrounded)
        {
            normalForce = -Vector3.Project(gravityForce, GroundChecker.singleton.GroundNormal);
           

        }
        //air drag force to reduce valocity,it help player to walk on slope 
        Vector3 dragForce = currentVelocity * -dragCoeff;

        Vector3 frictionForce = Vector3.zero;

        //calculating frictionforce from below line, it also help like air drag
        Vector3 slidingFrc = Vector3.ProjectOnPlane(gravityForce, GroundChecker.singleton.GroundNormal); // component of gravity along slope of terrain




        float magOfFriction = 0;
        Vector3 dirOfFriction = Vector3.zero;
        if (currentVelocity.magnitude > 0.1f || isControlledVelocityexisting)
        {

           //calculating dynamic friction

            magOfFriction = normalForce.magnitude * dynamicFrictionCoeff;
            Vector3 projection = Vector3.Project(currentVelocity, slidingFrc);
            if (projection.magnitude > 0.00001f)
                dirOfFriction = -projection.normalized;



        }
        else
        {


           //calculating static friction

            
            magOfFriction = Mathf.Min(staticFrictionCoeff * normalForce.magnitude, slidingFrc.magnitude);
            
                dirOfFriction = -slidingFrc.normalized;



        }
       
        
        
        frictionForce = magOfFriction * dirOfFriction;
      
        
        
        return new[] { gravityForce,normalForce,dragForce,frictionForce};
    }

    private void GoToJumpState()
    {
        
    
       
        if (GroundChecker.singleton.IsGrounded)
        {
            
            PlayerFsm.singleton.ChangeState(PlayerFsm.singleton.playerJumpState);
        }
        
    }
}
