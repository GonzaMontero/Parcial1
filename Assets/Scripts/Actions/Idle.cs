using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    public class Idle : FSMAction
    {
        public override List<Action> OnEnterBehaviours(FSMParameters onEnterParameters)
        {
            return new List<Action>();
        }

        public override List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters)
        {
            return new List<Action>();
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
}

