using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class FSM<TContext>
{
    public FsmBaseState<TContext> CurrentState { get; private set; }

    public void ChangeState(FsmBaseState<TContext> newState, TContext context)
    {
        CurrentState?.LeaveState(context);
        CurrentState = newState;
        CurrentState?.EnterState(context);
    }

    public void Update(float deltaTime, TContext context)
    {
        CurrentState?.UpdateState(deltaTime, context);
    }

    public void FixedUpdate(float fixedDeltaTime, TContext context)
    {
        CurrentState?.FixedUpdateState(fixedDeltaTime, context);
    }
}
