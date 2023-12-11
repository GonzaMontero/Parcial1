using AI.FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Entities
{
    public class EntityData
    {
        public bool shouldPathAgain;
        public Vector2 Position;
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
        public FSM.FSM fsm;
        protected List<Vector2Int> Path;
        protected FSMParameters parameters;
        public bool updatePos;
        public GridSlot[] map;

        public virtual void Init(Vector2Int position)
        {
            parameters = new FSMParameters();
        }

        public virtual void UpdateMiner()
        {
            fsm.Update();
        }

        public EntityData GetData()
        {
            return data;
        }

        public void InitMap(List<Vector2Int> buildings, List<Vector2Int> mines)
        {
            map = new GridSlot[50 * 50];
            GridUtils.GridSize = new Vector2Int(50, 50);
            int id = 0;

            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    map[id] = new GridSlot(id, new Vector2Int(j, i));
                    map[id].SetWeight(Random.Range(1, 6));

                    for (int k = 0; k < buildings.Count; k++)
                    {
                        if (map[id].position == buildings[k] && !mines.Contains(buildings[k]))
                        {
                            map[id].currentState = SlotStates.Obstacle;
                        }
                    }

                    id++;
                }
            }
        }
    }
}

