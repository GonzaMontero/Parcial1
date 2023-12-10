using AI.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using static UnityEditor.VersionControl.Asset;
using Random = UnityEngine.Random;

namespace AI.Entities
{
    public class Miner : AIEntity
    {
        private int timesMined = 0;
        private int resourcesMined = 0;

        private Action<Vector2Int> onEmptyMine;
        private FlockingAlgorithm flockingMiners;
        private PathingAlternatives pathingAlternatives;

        public Vector2 CurrentPos;
        public Vector2Int CurrentMine;
        public bool updatePos;

        public override void Init(Vector2Int position)
        {
            MinerManager.OnReturnToBaseCalled += () => fsm.ForceCurrentState((int)MinerStates.Return);

            flockingMiners = GetComponent<FlockingAlgorithm>();
            pathingAlternatives = new PathingAlternatives();

            data = new EntityData();

            data.Position = position;

            fsm = new FSM.FSM(Enum.GetValues(typeof(MinerStates)).Length, Enum.GetValues(typeof(MinerFlags)).Length);

            fsm.SetRelation((int)MinerStates.Collect, (int)MinerFlags.OnNearTarget, (int)MinerStates.Mine);
            fsm.SetRelation((int)MinerStates.Mine, (int)MinerFlags.OnInventoryFull, (int)MinerStates.Return);
            fsm.SetRelation((int)MinerStates.Return, (int)MinerFlags.OnFindTarget, (int)MinerStates.Collect);
            fsm.SetRelation((int)MinerStates.Mine, (int)MinerFlags.OnMineDepleted, (int)MinerStates.Collect);
            fsm.SetRelation((int)MinerStates.Collect, (int)MinerFlags.OnEmergency, (int)MinerStates.Return);

            parameters = new FSM.FSMParameters();

            parameters.Parameters = new object[6]
            {
                data,
                flockingMiners,
                pathingAlternatives,
                Path,
                timesMined,
                resourcesMined
            };

            fsm.SetAction<ReturnToBase>((int)MinerStates.Return, parameters, parameters);

            fsm.SetAction<HeadToMine>((int)MinerStates.Collect, parameters, parameters);

            fsm.SetAction<MineState>((int)MinerStates.Mine, parameters, parameters);

            fsm.ForceCurrentState((int)MinerStates.Collect);
        }

        public override void UpdateMiner()
        {
            fsm.Update();
        }

        //private void UpdatePos(Vector2 newPos)
        //{
        //    updatePos = true;
        //    CurrentPos = newPos;
        //}

        private void OnUpdateWeight()
        {
            data.shouldPathAgain = true;
        }
    }
}
