using AI.FSM;
using AI.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HeadToMine : FSMAction
{
    public int positionOnPath;
    public Vector3 currentDestination;

    public override List<Action> OnEnterBehaviours(FSMParameters onEnterParameters)
    {
        Func<Vector2Int> currentPos = onEnterParameters.Parameters[0] as Func<Vector2Int>;
        Func<Vector2Int> target = onEnterParameters.Parameters[8] as Func<Vector2Int>;
        PathingAlternatives alternatives = new PathingAlternatives();

        List<Action> onEnterActions = new List<Action>();
        onEnterActions.Add(() =>
        {

            MinerManager.Instance.PopulationTypes[0].PopulationVoronoiHandler.UpdateVillagerVoronoi();
            //target = MinerManager.Instance.PopulationTypes[0].PopulationVoronoiHandler.GetClosestVoronoi();
            onEnterParameters.Parameters[8] = target;

            List<Vector2Int> pathToMine = new List<Vector2Int>();

            //onEnterParameters.Parameters[4] = alternatives.GetPath(MapManager.Instance.Map,
            //    MapManager.Instance.Map[GridUtils.PositionToIndex(currentPos.Invoke())],
            //    MapManager.Instance.Map[GridUtils.PositionToIndex(target)]);
        });

        return onEnterActions;
    }

    public override List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters)
    {
        Func<Vector2Int> currentPos = onExecuteParameters.Parameters[0] as Func<Vector2Int>;
        Func<Vector2Int> target = onExecuteParameters.Parameters[8] as Func<Vector2Int>;
        FlockingAlgorithm flocking = onExecuteParameters.Parameters[2] as FlockingAlgorithm;
        PathingAlternatives alternatives = onExecuteParameters.Parameters[3] as PathingAlternatives;
        List<Vector2Int> path = onExecuteParameters.Parameters[4] as List<Vector2Int>;
        Func<bool> shouldCalculatePathAgain = onExecuteParameters.Parameters[5] as Func<bool>;

        List<Action> onExecuteBehaviours = new List<Action>();

        onExecuteBehaviours.Add(() =>
        {
            if (path == null)
            {
                path = alternatives.GetPath(MapManager.Instance.Map,
                MapManager.Instance.Map[GridUtils.PositionToIndex(currentPos.Invoke())],
                MapManager.Instance.Map[GridUtils.PositionToIndex(target.Invoke())]);

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
            else if(Vector2.Distance(currentDestination, currentPos.Invoke()) < 0.1f)
            {
                if (shouldCalculatePathAgain.Invoke())
                {
                    path = alternatives.GetPath(MapManager.Instance.Map,
                    MapManager.Instance.Map[GridUtils.PositionToIndex(currentPos.Invoke())],
                    MapManager.Instance.Map[GridUtils.PositionToIndex(target.Invoke())]);

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
                        SwapFlags((int)MinerFlags.OnNearTarget);
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

        return onExecuteBehaviours;
    }

    public override List<Action> OnExitBehaviours(FSMParameters onExitParameters)
    {
        throw new NotImplementedException();
    }

    public override void SwapFlags(int flags)
    {
        throw new NotImplementedException();
    }
}
