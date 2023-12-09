using AI.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Entities
{
    public abstract class AIEntity : MonoBehaviour
    {
        protected GameObject Target;
        protected GameObject TownHall;
        protected FSM.FSM fsm;
        protected List<Vector2Int> Path;
        protected FSMParameters parameters;

        protected virtual void Init(int stateLength, int flagLength)
        {
            parameters = new FSMParameters();
            fsm = new FSM.FSM(stateLength, flagLength);
        }

        protected virtual void Update()
        {
            fsm.Update();
        }
    }
}

