using AI.FSM;
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

        int previousStateIndex;
        
        public override void Init(Vector2Int position)
        {
            MinerManager.OnReturnToBaseCalled += (bool shouldReturn) =>
            {
               fsm.SetFlag((int)MinerFlags.OnEmergency);
            };

            flockingMiners = GetComponent<FlockingAlgorithm>();
            pathingAlternatives = new PathingAlternatives();

            data = new EntityData();

            data.Position = position;
            data.Deposit = new Vector2Int(MapManager.Instance.MinerSpawnPosition.x, MapManager.Instance.MinerSpawnPosition.y);

            fsm = new FSM.FSM(Enum.GetValues(typeof(MinerStates)).Length, Enum.GetValues(typeof(MinerFlags)).Length);

            fsm.SetRelation((int)MinerStates.Collect, (int)MinerFlags.OnNearTarget, (int)MinerStates.Mine);
            fsm.SetRelation((int)MinerStates.Mine, (int)MinerFlags.OnInventoryFull, (int)MinerStates.Return);
            fsm.SetRelation((int)MinerStates.Return, (int)MinerFlags.OnFindTarget, (int)MinerStates.Collect);
            fsm.SetRelation((int)MinerStates.Mine, (int)MinerFlags.OnMineDepleted, (int)MinerStates.Collect);
            fsm.SetRelation((int)MinerStates.Collect, (int)MinerFlags.OnEmergency, (int)MinerStates.Return);
            fsm.SetRelation((int)MinerStates.Mine, (int)MinerFlags.OnEmergency, (int)MinerStates.Return);
            fsm.SetRelation((int)MinerStates.Idle, (int)MinerFlags.OnEmergency, (int)MinerStates.Collect);
            fsm.SetRelation((int)MinerStates.Return, (int)MinerFlags.OnEmergency, (int)MinerStates.Collect);

            parameters = new FSM.FSMParameters();

            flockingMiners.Init(UpdatePos, GetPos);

            parameters.Parameters = new object[7]
            {
                data,
                flockingMiners,
                pathingAlternatives,
                Path,
                timesMined,
                resourcesMined,
                map
            };

            fsm.SetAction<ReturnToBase>((int)MinerStates.Return, parameters, parameters);

            fsm.SetAction<HeadToMine>((int)MinerStates.Collect, parameters, parameters);

            fsm.SetAction<MineState>((int)MinerStates.Mine, parameters, parameters);

            fsm.SetAction<Idle>((int)MinerStates.Idle, parameters, parameters);

            fsm.ForceCurrentState((int)MinerStates.Collect);
        }

        public override void UpdateMiner()
        {
            fsm.Update();
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

        private void OnUpdateWeight()
        {
            data.shouldPathAgain = true;
        }        
    }
}
