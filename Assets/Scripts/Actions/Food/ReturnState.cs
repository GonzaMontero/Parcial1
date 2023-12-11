using AI.Entities;
using AI.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    public class ReturnState : FSMAction
    {
        public int positionOnPath;
        public Vector3 currentDestination;


        public override List<Action> OnEnterBehaviours(FSMParameters onEnterParameters)
        {
            EntityData data = onEnterParameters.Parameters[0] as EntityData;
            PathingAlternatives alternatives = onEnterParameters.Parameters[2] as PathingAlternatives;
            List<Vector2Int> path = onEnterParameters.Parameters[3] as List<Vector2Int>;

            List<Action> onEnterBehaviors = new List<Action>();

            onEnterBehaviors.Add(() =>
            {
                onEnterParameters.Parameters[3] = alternatives.GetPath(MapManager.Instance.Map,
                MapManager.Instance.Map[GridUtils.PositionToIndex(new Vector2Int((int)data.Position.x, (int)data.Position.y))],
                MapManager.Instance.Map[GridUtils.PositionToIndex(data.Deposit)], out int var);
            });

            return onEnterBehaviors;
        }

        public override List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters)
        {
            EntityData currentData = onExecuteParameters.Parameters[0] as EntityData;
            Vector2 currentPos = currentData.Position;
            Vector2Int baseHome = currentData.Deposit;
            bool shouldCalculatePathAgain = currentData.shouldPathAgain;

            FlockingAlgorithm flocking = onExecuteParameters.Parameters[1] as FlockingAlgorithm;
            PathingAlternatives alternatives = onExecuteParameters.Parameters[2] as PathingAlternatives;
            List<Vector2Int> path = onExecuteParameters.Parameters[3] as List<Vector2Int>;


            List<Action> onExecuteActions = new List<Action>();
            onExecuteActions.Add(() =>
            {
                if (path == null)
                {
                    path = alternatives.GetPath(MapManager.Instance.Map,
                    MapManager.Instance.Map[GridUtils.PositionToIndex(new Vector2Int((int)currentPos.x, (int)currentPos.y))],
                    MapManager.Instance.Map[GridUtils.PositionToIndex(baseHome)], out int var);

                    positionOnPath = 0;

                    if (positionOnPath > path.Count - 1)
                    {
                        path = null;
                        return;
                    }

                    currentDestination = new Vector3(path[positionOnPath].x, path[positionOnPath].y, 0);

                    flocking.ToggleFlocking(true);
                    flocking.UpdateTarget(new Vector2Int((int)currentDestination.x, (int)currentDestination.y));
                }
                else if (Vector2.Distance(currentDestination, currentPos) < 0.1f)
                {
                    if (shouldCalculatePathAgain)
                    {
                        path = alternatives.GetPath(MapManager.Instance.Map,
                        MapManager.Instance.Map[GridUtils.PositionToIndex(new Vector2Int((int)currentPos.x, (int)currentPos.y))],
                        MapManager.Instance.Map[GridUtils.PositionToIndex(baseHome)], out int var);

                        positionOnPath = 0;

                        currentDestination = new Vector3(path[positionOnPath].x, path[positionOnPath].y, 0);


                        flocking.ToggleFlocking(true);
                        flocking.UpdateTarget(new Vector2Int((int)currentDestination.x, (int)currentDestination.y));
                    }
                    else
                    {
                        positionOnPath++;

                        if (positionOnPath >= path.Count - 1 && !MinerManager.Instance.ReturnToBase)
                        {
                            path = null;
                            onExecuteParameters.Parameters[4] = 10;
                            SwapFlags((int)FoodFlags.OnSupply);
                            return;
                        }
                        else if (MinerManager.Instance.ReturnToBase)
                        {
                            return;
                        }

                        currentDestination = new Vector3(path[positionOnPath].x, path[positionOnPath].y, 0);


                        flocking.ToggleFlocking(true);
                        flocking.UpdateTarget(new Vector2Int((int)currentDestination.x, (int)currentDestination.y));
                    }
                }
            });

            return onExecuteActions;
        }

        public override List<Action> OnExitBehaviours(FSMParameters onExitParameters)
        {
            return new List<Action>();
        }

        public override void SwapFlags(int flags)
        {
            OnSetFlag(flags);
        }
    }
}