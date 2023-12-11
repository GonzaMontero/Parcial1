using AI.FSM;
using AI.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Entities
{
    public class FoodCarriage : AIEntity
    {
        private float food = 0;

        private FlockingAlgorithm flockingMiners;
        private PathingAlternatives pathingAlternatives;

        public Vector2 CurrentPos;
        public Vector2Int CurrentMine;

        int previousStateIndex;

        public override void Init(Vector2Int position)
        {
            MinerManager.OnReturnToBaseCalled += (bool shouldReturn) =>
            {
                if(!shouldReturn)
                    fsm.SetFlag((int)FoodFlags.OnEmergency);
                else
                {
                    if (MapManager.Instance.AllWorkedMines.Count > 0)
                        fsm.SetFlag((int)FoodFlags.OnSupply);
                }
            };

            flockingMiners = GetComponent<FlockingAlgorithm>();
            flockingMiners.Init(UpdatePos, GetPos);

            pathingAlternatives = new PathingAlternatives();
            parameters = new FSM.FSMParameters();

            data = new EntityData();

            data.Position = position;
            data.Deposit = new Vector2Int(MapManager.Instance.MinerSpawnPosition.x, MapManager.Instance.MinerSpawnPosition.y);

            fsm = new FSM.FSM(Enum.GetValues(typeof(FoodStates)).Length, Enum.GetValues(typeof(FoodFlags)).Length);
            fsm.SetRelation((int)FoodStates.Supply, (int)FoodFlags.OnReturnToDeposit, (int)FoodStates.Return);
            fsm.SetRelation((int)FoodStates.Return, (int)FoodFlags.OnSupply, (int)FoodStates.Supply);
            fsm.SetRelation((int)FoodStates.Idle, (int)FoodFlags.OnSupply, (int)FoodStates.Supply);
            fsm.SetRelation((int)FoodStates.Supply, (int)FoodFlags.OnEmergency, (int)FoodStates.Return);
            fsm.SetRelation((int)FoodStates.Return, (int)FoodFlags.OnEmergency, (int)FoodStates.Idle);

            parameters.Parameters = new object[6]
            {
                data,
                flockingMiners,
                pathingAlternatives,
                Path,
                food,
                map
            };

            fsm.SetAction<ReturnState>((int)FoodStates.Return, parameters, parameters);
            fsm.SetAction<SupplyState>((int)FoodStates.Supply, parameters, parameters);
            fsm.SetAction<Idle>((int)FoodStates.Idle, parameters, parameters);

            fsm.ForceCurrentState((int)FoodStates.Idle);
        }

        private void UpdatePos(Vector2 newPos)
        {
            updatePos = true;
            data.Position = newPos;
        }

        private Vector2 GetPos()
        {
            return data.Position;
        }
    }
}
