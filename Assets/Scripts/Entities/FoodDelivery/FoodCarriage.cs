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

        public override void Init(Vector2Int position)
        {
            MineItem.OnMineDrained += (bool areMines, bool areWorkedMines) =>
            {
                if (!areWorkedMines)
                    fsm.ForceCurrentState((int)FoodStates.Return);
            };

            MinerManager.OnReturnToBaseCalled += () => fsm.ForceCurrentState((int)FoodStates.Return);

            flockingMiners = GetComponent<FlockingAlgorithm>();
            parameters = new FSM.FSMParameters();

            data = new EntityData();
            data.Position = position;
            data.Deposit = MapManager.Instance.DepositPos;

            fsm = new FSM.FSM(Enum.GetValues(typeof(FoodStates)).Length, Enum.GetValues(typeof(FoodFlags)).Length);

            fsm.SetRelation((int)FoodStates.Supply, (int)FoodFlags.OnReturnToDeposit, (int)FoodStates.Return);
            fsm.SetRelation((int)FoodStates.Return, (int)FoodFlags.OnSupply, (int)FoodStates.Supply);

            parameters.Parameters = new object[5]
            {
                data,
                flockingMiners,
                pathingAlternatives,
                Path,
                food
            };

            fsm.SetAction<ReturnState>((int)FoodStates.Return, parameters, parameters);
        }
    }
}
