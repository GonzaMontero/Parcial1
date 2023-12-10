using AI.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Entities
{
    public class EntityData
    {
        public bool shouldPathAgain;
        public Vector2Int Position;
        public Vector2Int Target;
        public Vector2Int Deposit;
        public MineItem targetMine;
    }

    public abstract class AIEntity : MonoBehaviour
    {
        protected EntityData data;
        protected FSM.FSM fsm;
        protected List<Vector2Int> Path;
        protected FSMParameters parameters;

        protected virtual void Init(int stateLength, int flagLength)
        {
            parameters = new FSMParameters();
            fsm = new FSM.FSM(stateLength, flagLength);
        }

        public virtual void UpdateMiner()
        {
            fsm.Update();
        }
    }
}

