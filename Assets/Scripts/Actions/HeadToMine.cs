using AI.Entities;
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
        EntityData data = onEnterParameters.Parameters[0] as EntityData;
        PathingAlternatives alternatives = onEnterParameters.Parameters[2] as PathingAlternatives;

        List<Action> onEnterActions = new List<Action>();
        onEnterActions.Add(() =>
        {
            MinerManager.Instance.PopulationTypes[0].PopulationVoronoiHandler.UpdateActiveVoronoi(MapManager.Instance.AllMinesOnMap);
            data.Target = MapManager.Instance.Map[MinerManager.Instance.PopulationTypes[0].PopulationVoronoiHandler.GetClosestMine(
                GridUtils.PositionToIndex(data.Position))].position;
            onEnterParameters.Parameters[0] = data;

            List<Vector2Int> pathToMine = new List<Vector2Int>();

            pathToMine = alternatives.GetPath(MapManager.Instance.Map,
                MapManager.Instance.Map[GridUtils.PositionToIndex(data.Position)],
                MapManager.Instance.Map[GridUtils.PositionToIndex(data.Target)], out int totalCost);

            onEnterParameters.Parameters[3] = pathToMine;
        });

        return onEnterActions;
    }

    public override List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters)
    {
        EntityData data = onExecuteParameters.Parameters[0] as EntityData;
        FlockingAlgorithm flocking = onExecuteParameters.Parameters[1] as FlockingAlgorithm;
        PathingAlternatives alternatives = onExecuteParameters.Parameters[2] as PathingAlternatives;
        List<Vector2Int> path = onExecuteParameters.Parameters[3] as List<Vector2Int>;

        List<Action> onExecuteBehaviours = new List<Action>();

        onExecuteBehaviours.Add(() =>
        {
            if (path == null)
            {
                path = alternatives.GetPath(MapManager.Instance.Map,
                MapManager.Instance.Map[GridUtils.PositionToIndex(data.Position)],
                MapManager.Instance.Map[GridUtils.PositionToIndex(data.Target)], out int var);

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
            else if(Vector2.Distance(data.Position, data.Target) < 1f)
            {
                if (data.shouldPathAgain)
                {
                    path = alternatives.GetPath(MapManager.Instance.Map,
                    MapManager.Instance.Map[GridUtils.PositionToIndex(data.Position)],
                    MapManager.Instance.Map[GridUtils.PositionToIndex(data.Target)], out int var);

                    onExecuteParameters.Parameters[3] = path;

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
