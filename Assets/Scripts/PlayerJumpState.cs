using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;


[System.Serializable]
public class PlayerJumpState : FsmBaseState<PlayerFsm>
{
    [SerializeField] private float jumpImpulse = 20f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float airFrictionCoefficient = 0.3f;
    private bool alreadyGoneToMovementState = false;
    private bool isitGoneABoveGround = false;

    public override void EnterState(PlayerFsm context)
    {
        alreadyGoneToMovementState = false;
        isitGoneABoveGround = false;
        context.playerAnimator.Play("Jumping");
        Vector2 inputMoveVector = context.playerInput.actions["Move"].ReadValue<Vector2>();

        //rotating/modifying input values to transform it into move direction
        Vector3 controlDir = context.transform.rotation * new Vector3(inputMoveVector.x, 0, inputMoveVector.y).normalized;
        Vector3 groundNormal = GroundChecker.singleton.GroundNormal;
        Vector3 dirOfJump = (controlDir + groundNormal).normalized;
        var pvc = context.physicsBasedVelocityCalculator;
        Vector3 physicsveloOnDirOfJump = Vector3.Project(pvc.PhysicsVelocity,dirOfJump);
       
            pvc.PhysicsVelocity -= physicsveloOnDirOfJump;
        

        pvc.SetContinuousForces(GetContinuousForces(pvc.PhysicsVelocity));
        pvc.PhysicsVelocity += controlDir * 5f;
        pvc.AddImpulse(groundNormal * jumpImpulse);
      
     
        //context.ChangeState(context.playerMovementState);
    }

    public override void FixedUpdateState(float fixedDeltaTime, PlayerFsm context)
    {
     
            context.cc.Move(context.physicsBasedVelocityCalculator.PhysicsVelocity * fixedDeltaTime);
        context.velocityFieldUi.value = context.transform.InverseTransformDirection(context.RoundVector3(context.physicsBasedVelocityCalculator.PhysicsVelocity));

    }

    public override void LeaveState(PlayerFsm context)
    {
    
    }

    public override void UpdateState(float deltaTime, PlayerFsm context)
    {
        if (GroundChecker.singleton.IsGrounded != true)
        {
            isitGoneABoveGround = true;
        }
      
    
            if (isitGoneABoveGround && GroundChecker.singleton.IsGrounded)
            {

               
            if (!alreadyGoneToMovementState )
            {
                alreadyGoneToMovementState = true;
               
               GoToMovementState(context);
            }
                






            }
        
    }

    private async Task GoToMovementState(PlayerFsm context)
    {

       
       while(context.playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime< 0.99f){
            await Task.Yield();
        }
        context.ChangeState(context.playerMovementState);

    }
   

    private Vector3[] GetContinuousForces(Vector3 currentVelocity)
    {
        Vector3 gravityForce = new Vector3(0f, gravity, 0f);
        Vector3 airFriction = -currentVelocity * airFrictionCoefficient;

        return new[] { gravityForce, airFriction };
    }
}

