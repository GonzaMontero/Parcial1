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

        public EntityData()
        {
            shouldPathAgain = false;
            Position = new Vector2Int();
            Target = new Vector2Int();
            Deposit = new Vector2Int();
        }
    }

    public abstract class AIEntity : MonoBehaviour
    {
        protected EntityData data;
        protected FSM.FSM fsm;
        protected List<Vector2Int> Path;
        protected FSMParameters parameters;

        public virtual void Init(Vector2Int position)
        {
            parameters = new FSMParameters();
        }

        public virtual void UpdateMiner()
        {
            fsm.Update();
        }
    }
}

