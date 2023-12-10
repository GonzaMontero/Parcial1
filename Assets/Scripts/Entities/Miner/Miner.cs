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
        public bool updatePath = false;

        protected override void Init(int stateLength, int flagLength)
        {
            MinerManager.OnReturnToBaseCalled += () => fsm.ForceCurrentState((int)MinerStates.Return);

            fsm = new FSM.FSM(stateLength, flagLength);

            fsm.SetRelation((int)MinerStates.Collect, (int)MinerFlags.OnNearTarget, (int)MinerStates.Mine);
            fsm.SetRelation((int)MinerStates.Mine, (int)MinerFlags.OnInventoryFull, (int)MinerStates.Return);
            fsm.SetRelation((int)MinerStates.Return, (int)MinerFlags.OnFindTarget, (int)MinerStates.Collect);
            fsm.SetRelation((int)MinerStates.Mine, (int)MinerFlags.OnMineDepleted, (int)MinerStates.Collect);
            fsm.SetRelation((int)MinerStates.Collect, (int)MinerFlags.OnEmergency, (int)MinerStates.Return);

            parameters = new FSM.FSMParameters();

            parameters.Parameters = new object[8]
            {
                GetPos(),
                GetTownHall(),
                flockingMiners,
                pathingAlternatives,
                Path,
                RePath(),
                GetTarget(),
                Target
            };

            fsm.SetAction<ReturnToBase>((int)MinerStates.Return, parameters, parameters);

            fsm.SetAction<HeadToMine>((int)MinerStates.Collect, parameters, parameters);
        }

        public void UpdateMiner()
        {
            fsm.Update();
        }

        private void UpdatePos(Vector2 newPos)
        {
            updatePos = true;
            CurrentPos = newPos;
        }

        private void UpdateTarget(Vector2Int newTarget)
        {
            flockingMiners.ToggleFlocking(true);
            flockingMiners.UpdateTarget(newTarget);
        }

        private void StopMovement()
        {
            flockingMiners.ToggleFlocking(false);
        }

        private Vector2 GetPos()
        {
            return CurrentPos;
        }

        private Vector2Int GetTownHall()
        {
            return TownHall;
        }

        private Vector2Int GetTarget()
        {
            return Target;
        }

        public void UpdateMine(Vector2Int minePos)
        {
            CurrentMine = minePos;
            updatePath = true;
        }

        private void OnEmptyMine()
        {
            updatePath = true;
            onEmptyMine?.Invoke(CurrentMine);
        }

        private void OnUpdateWeight()
        {
            updatePath = true;
        }

        private bool RePath()
        {
            return updatePath;
        }
    }
}
