using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class FsmBaseState<TContext>
{
    protected TContext context;


    public void SetContext(TContext ctx)
    {
        context = ctx;
    }
    public abstract void EnterState(TContext context);
    public abstract void UpdateState(float deltaTime, TContext context);
    public abstract void FixedUpdateState(float fixedDeltaTime, TContext context);
    public abstract void LeaveState(TContext context);
}

