using AI.Entities;
using AI.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    public class SupplyState : FSMAction
    {
        public int positionOnPath;
        public Vector3 currentDestination;
        public GridSlot[] map;

        public override List<Action> OnEnterBehaviours(FSMParameters onEnterParameters)
        {
            EntityData data = onEnterParameters.Parameters[0] as EntityData;
            FlockingAlgorithm flocking = onEnterParameters.Parameters[1] as FlockingAlgorithm;
            PathingAlternatives alternatives = onEnterParameters.Parameters[2] as PathingAlternatives;
            List<Vector2Int> path = onEnterParameters.Parameters[3] as List<Vector2Int>;

            map = onEnterParameters.Parameters[5] as GridSlot[];

            List<Action> onEnterBehaviours = new List<Action>();

            onEnterBehaviours.Add(() =>
            {
                data.Target = MinerManager.Instance.PopulationTypes[1].PopulationVoronoiHandler.GetNearestMine(data.Position);
                onEnterParameters.Parameters[0] = data;

                path = alternatives.GetPath(map,
                map[GridUtils.PositionToIndex(new Vector2Int((int)data.Position.x, (int)data.Position.y))],
                map[GridUtils.PositionToIndex(data.Target)]);

                onEnterParameters.Parameters[3] = path;

                positionOnPath = 0;

                if (positionOnPath > path.Count - 1)
                {
                    path = null;
                    return;
                }

                currentDestination = new Vector3(path[positionOnPath].x, path[positionOnPath].y, 0);

                flocking.ToggleFlocking(true);
                flocking.UpdateTarget(new Vector2Int((int)currentDestination.x, (int)currentDestination.y));
            });

            return onEnterBehaviours;
        }

        public override List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters)
        {
            EntityData data = onExecuteParameters.Parameters[0] as EntityData;
            Vector2 currentPos = data.Position;
            Vector2Int baseHome = data.Deposit;
            bool shouldCalculatePathAgain = data.shouldPathAgain;

            FlockingAlgorithm flocking = onExecuteParameters.Parameters[1] as FlockingAlgorithm;
            PathingAlternatives alternatives = onExecuteParameters.Parameters[2] as PathingAlternatives;
            List<Vector2Int> path = onExecuteParameters.Parameters[3] as List<Vector2Int>;

            map = onExecuteParameters.Parameters[5] as GridSlot[];

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {                
                if (path == null)
                {
                    data.Target = MinerManager.Instance.PopulationTypes[1].PopulationVoronoiHandler.GetNearestMine(data.Position);
                    onExecuteParameters.Parameters[0] = data;

                    path = alternatives.GetPath(map,
                    map[GridUtils.PositionToIndex(new Vector2Int((int)currentPos.x, (int)currentPos.y))],
                    map[GridUtils.PositionToIndex(data.Target)]);

                    onExecuteParameters.Parameters[3] = path;

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
                        data.Target = MinerManager.Instance.PopulationTypes[1].PopulationVoronoiHandler.GetNearestMine(data.Position);
                        onExecuteParameters.Parameters[0] = data;

                        path = alternatives.GetPath(map,
                        map[GridUtils.PositionToIndex(new Vector2Int((int)currentPos.x, (int)currentPos.y))],
                        map[GridUtils.PositionToIndex(data.Target)]);

                        onExecuteParameters.Parameters[3] = path;

                        positionOnPath = 0;

                        currentDestination = new Vector3(path[positionOnPath].x, path[positionOnPath].y, 0);


                        flocking.ToggleFlocking(true);
                        flocking.UpdateTarget(new Vector2Int((int)currentDestination.x, (int)currentDestination.y));
                    }
                    else
                    {
                        if (positionOnPath >= path.Count - 1)
                        {
                            if (MapManager.Instance.GetItemOnPosition(data.Target) != null)
                            {
                                data.targetMine = MapManager.Instance.GetItemOnPosition(data.Target);
                                data.targetMine.FoodAmount += 10;
                                SwapFlags((int)FoodFlags.OnReturnToDeposit);
                                onExecuteParameters.Parameters[0] = data;
                                return;
                            }
                            else
                            {
                                data.targetMine = null;
                                onExecuteParameters.Parameters[0] = data;
                            }

                            if (data.targetMine == null)
                            {
                                data.Target = MinerManager.Instance.PopulationTypes[0].PopulationVoronoiHandler.GetNearestMine(data.Position);
                                onExecuteParameters.Parameters[0] = data;

                                path = alternatives.GetPath(map,
                                map[GridUtils.PositionToIndex(new Vector2Int((int)data.Position.x, (int)data.Position.y))],
                                map[GridUtils.PositionToIndex(data.Target)]);

                                onExecuteParameters.Parameters[3] = path;

                                return;
                            }
                        }

                        positionOnPath++;

                        currentDestination = new Vector3(path[positionOnPath].x, path[positionOnPath].y, 0);
                        flocking.ToggleFlocking(true);
                        flocking.UpdateTarget(new Vector2Int((int)currentDestination.x, (int)currentDestination.y));
                    }
                }

            });

            return behaviours;
        }

        public override List<Action> OnExitBehaviours(FSMParameters onExitParameters)
        {
            return new List<Action>();
        }

        public override void SwapFlags(int flags)
        {
            OnSetFlag?.Invoke(flags);
        }
    }
}