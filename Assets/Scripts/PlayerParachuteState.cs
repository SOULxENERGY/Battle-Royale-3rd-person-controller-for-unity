using UnityEngine;
using UnityEngine.Animations.Rigging;

[System.Serializable]
public class PlayerParachuteState : FsmBaseState<PlayerFsm>
{
    [SerializeField] private Vector3 lookAtLocalPos;
    [SerializeField] private GameObject parachute;
  
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float airFrictionCoefficient = 0.9f;
    [SerializeField] private float controlvelo = 20f;
    [SerializeField] private Rig parachuteRig;
    [SerializeField] private AudioClip parachuteMp3;
    
    public override void EnterState(PlayerFsm context)
    {
        context.audioSrc.loop = false;
        context.audioSrc.clip = parachuteMp3;
        context.audioSrc.Play();
        
       
        parachute.SetActive(true);

        // Switch to third person camera with correct LookAt
        
        CmCameraSwitch.singleton.SwitchCmCam(
            CmCameraSwitch.singleton.PlayerThirdPersonCam,
            context.lookAtObjectOfThirdPersonCamera
        );
        CmCameraSwitch.singleton.PlayerThirdPersonCam.Lens.FarClipPlane = 7000f;
        context.playerAnimator.Play("Hanging Idle");
        parachuteRig.weight = 1;
        var pvc = context.physicsBasedVelocityCalculator;
        //sudden upward drag force at the time of opening the parachute
        pvc.AddImpulse(pvc.PhysicsVelocity.normalized * -100);
    }

    public override void FixedUpdateState(float fixedDeltaTime, PlayerFsm context)
    {
        var pvc = context.physicsBasedVelocityCalculator;

        // Set continuous forces (gravity + air friction)
        pvc.SetContinuousForces(GetContinuousForces(pvc.PhysicsVelocity));
        

        Vector2 inputMoveVector= context.playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 controlDir =context.transform.rotation* new Vector3(inputMoveVector.x, 0, inputMoveVector.y).normalized;


        Vector3 netvelocity = pvc.PhysicsVelocity + controlDir * controlvelo;

        context.velocityFieldUi.value = context.transform.InverseTransformDirection(context.RoundVector3(netvelocity));

        context.cc.Move(netvelocity * fixedDeltaTime);


        //DID NOT FIND ENOUGH PARACHUTE ANUMATIONS ONLINE SO THIS CODES ARE TO ANIMATE PARACHUTE DIRECTION CONTROL.
        float signX = 0;
        float signZ = 0;
        if (inputMoveVector.x != 0)
        {
            signX = -inputMoveVector.x / Mathf.Abs(inputMoveVector.x);

        }
        if (inputMoveVector.y != 0)
        {
            signZ= inputMoveVector.y/ Mathf.Abs(inputMoveVector.y);
        }
        Quaternion targetRot = Quaternion.Euler(6f * signZ, 0, 6f * signX);
        ;
        context.actual3dModelOfPlayer.localRotation = Quaternion.Slerp(
            context.actual3dModelOfPlayer.localRotation,
            targetRot,
            fixedDeltaTime * 10f // the higher the multiplier, the faster it rotates
        );

     





        //............................................................................

    }

    public override void LeaveState(PlayerFsm context)
    {
        parachuteRig.weight = 0;
        parachute.SetActive(false);
    }

    public override void UpdateState(float deltaTime, PlayerFsm context)
    {
        context.lookAtObjectOfThirdPersonCamera.localPosition = Vector3.Lerp(context.lookAtObjectOfThirdPersonCamera.localPosition, lookAtLocalPos, deltaTime * 10f);
        if (GroundChecker.singleton.IsGrounded)
        {
            context.ChangeState(context.playerMovementState);
        };
    }

    private Vector3[] GetContinuousForces(Vector3 currentVelocity)
    {
        Vector3 gravityForce = new Vector3(0f, gravity, 0f);
        Vector3 airFriction = -currentVelocity * airFrictionCoefficient;
        return new[] { gravityForce, airFriction };
    }
}
