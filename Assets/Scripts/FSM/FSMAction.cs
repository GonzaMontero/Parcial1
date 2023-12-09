using System;
using System.Collections.Generic;

namespace AI.FSM
{
    public abstract class FSMAction
    {
        public Action<int> OnSetFlag;

        public abstract List<Action> OnEnterBehaviours(FSMParameters onEnterParameters);
        public abstract List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters);
        public abstract List<Action> OnExitBehaviours(FSMParameters onExitParameters);

        public abstract void SwapFlags(int flags);
    }
}