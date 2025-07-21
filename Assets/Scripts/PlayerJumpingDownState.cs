using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class PlayerJumpingDownState : FsmBaseState<PlayerFsm>
{
    [SerializeField] private Vector3 lookAtLocalPos;
    [SerializeField] private float skyDiveVelocity = 50f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float airFrictionCoefficient = 0.3f;
    [SerializeField] private AudioClip skyDiveMp3;
    [SerializeField] private float minimumHeightToOpenParachute = 150;

    public override void EnterState(PlayerFsm context)
    {
        context.audioSrc.loop = true;
        context.audioSrc.clip = skyDiveMp3;
        context.audioSrc.Play();
        context.parachuteEvent += OpenParachute;
        // Set camera target's local position
        context.lookAtObjectOfThirdPersonCamera.localPosition = lookAtLocalPos;

        // Switch to third person camera with correct LookAt
        CmCameraSwitch.singleton.SwitchCmCam(
            CmCameraSwitch.singleton.PlayerThirdPersonCam,
            context.lookAtObjectOfThirdPersonCamera
        );
        CmCameraSwitch.singleton.PlayerThirdPersonCam.Lens.FarClipPlane = 7000f;

        // Play flying animation
        context.playerAnimator.Play("Flying");
    }

    public override void FixedUpdateState(float fixedDeltaTime, PlayerFsm context)
    {
        var pvc = context.physicsBasedVelocityCalculator;

        // Set continuous forces (gravity + air friction)
        pvc.SetContinuousForces(GetContinuousForces(pvc.PhysicsVelocity));

        // Move the character controller using physics-based + dive velocity
        Vector3 totalVelocity = pvc.PhysicsVelocity + context.transform.forward * skyDiveVelocity;
        context.velocityFieldUi.value = context.transform.InverseTransformDirection( context.RoundVector3(totalVelocity));
        
        
        context.cc.Move(totalVelocity * fixedDeltaTime);
      
    }

    public override void LeaveState(PlayerFsm context)
    {
        context.parachuteEvent -= OpenParachute;
        context.physicsBasedVelocityCalculator.PhysicsVelocity += context.transform.forward * skyDiveVelocity;
    }

    public override void UpdateState(float deltaTime, PlayerFsm context)
    {
        //Debug.Log(GroundChecker.singleton.)
        if ( GroundChecker.singleton.isGroundDetected &&  GroundChecker.singleton.distanceOfPlayerFromGroundHitPoint < minimumHeightToOpenParachute)
        {
            OpenParachute();
        }
    }

    private Vector3[] GetContinuousForces(Vector3 currentVelocity)
    {
        Vector3 gravityForce = new Vector3(0f, gravity, 0f);
        Vector3 airFriction = -currentVelocity * airFrictionCoefficient;
       
        return new[] { gravityForce, airFriction };
    }

    private void OpenParachute()
    {
        PlayerFsm.singleton.ChangeState(PlayerFsm.singleton.playerParachuteState);
    }
}

