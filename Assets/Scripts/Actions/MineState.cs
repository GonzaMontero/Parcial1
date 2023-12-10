using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.FSM;
using System;

public class MineState : FSMAction
{
    public override List<Action> OnEnterBehaviours(FSMParameters onEnterParameters)
    {
        return new List<Action>();
    }

    public override List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters)
    {
        throw new NotImplementedException();
    }

    public override List<Action> OnExitBehaviours(FSMParameters onExitParameters)
    {
        return new List<Action>();
    }

    public override void SwapFlags(int flags)
    {
        OnSetFlag?.Invoke(flags);
    }
}
