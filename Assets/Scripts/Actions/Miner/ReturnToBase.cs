using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.FSM;
using AI.Managers;
using System;
using AI.Entities;

public class ReturnToBase : FSMAction
{
    private int positionOnPath;
    private Vector3 currentDestination;

    public override List<Action> OnEnterBehaviours(FSMParameters onEnterParameters)
    {
        EntityData currentData = onEnterParameters.Parameters[0] as EntityData;
        Vector2 currentPos = currentData.Position;
        Vector2Int baseHome = currentData.Deposit;

        PathingAlternatives alternatives = onEnterParameters.Parameters[2] as PathingAlternatives;

        List<Action> onEnterActions = new List<Action>();
        onEnterActions.Add(() =>
        {
            onEnterParameters.Parameters[3] = alternatives.GetPath(MapManager.Instance.Map, 
                MapManager.Instance.Map[GridUtils.PositionToIndex(new Vector2Int((int)currentPos.x, (int)currentPos.y))], 
                MapManager.Instance.Map[GridUtils.PositionToIndex(baseHome)], out int var);
        });

        return onEnterActions;
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
                MapManager.Instance.Map[GridUtils.PositionToIndex(new Vector2Int((int)currentPos.x,(int)currentPos.y))],
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
            else if (Vector2.Distance(currentDestination,currentPos) < 0.1f)
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

                    if(positionOnPath >= path.Count - 1 && !MinerManager.Instance.ReturnToBase)
                    {
                        path = null;
                        SwapFlags((int)MinerFlags.OnReachDeposit);
                        return;
                    }
                    else if(MinerManager.Instance.ReturnToBase)
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
        OnSetFlag?.Invoke(flags);
    }
}
