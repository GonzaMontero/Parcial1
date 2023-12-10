using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.FSM;
using AI.Managers;
using System;

public class ReturnToBase : FSMAction
{
    private int positionOnPath;
    private Vector3 currentDestination;

    public override List<Action> OnEnterBehaviours(FSMParameters onEnterParameters)
    {
        Func<Vector2Int> currentPos = onEnterParameters.Parameters[0] as Func<Vector2Int>;
        Func<Vector2Int> baseHome = onEnterParameters.Parameters[8] as Func<Vector2Int>;
        PathingAlternatives alternatives = new PathingAlternatives();

        List<Action> onEnterActions = new List<Action>();
        onEnterActions.Add(() =>
        {
            onEnterParameters.Parameters[4] = alternatives.GetPath(MapManager.Instance.Map, 
                MapManager.Instance.Map[GridUtils.PositionToIndex(currentPos.Invoke())], 
                MapManager.Instance.Map[GridUtils.PositionToIndex(baseHome.Invoke())]);
        });

        return onEnterActions;
    }

    public override List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters)
    {
        Func<Vector2Int> currentPos = onExecuteParameters.Parameters[0] as Func<Vector2Int>;
        Func<Vector2Int> baseHome = onExecuteParameters.Parameters[1] as Func<Vector2Int>;
        FlockingAlgorithm flocking = onExecuteParameters.Parameters[2] as FlockingAlgorithm;
        PathingAlternatives alternatives = onExecuteParameters.Parameters[3] as PathingAlternatives;
        List<Vector2Int> path = onExecuteParameters.Parameters[4] as List<Vector2Int>;       
        Func<bool> shouldCalculatePathAgain = onExecuteParameters.Parameters[5] as Func<bool>;


        List<Action> onExecuteActions = new List<Action>();
        onExecuteActions.Add(() =>
        {
            if (path == null)
            {
                path = alternatives.GetPath(MapManager.Instance.Map,
                MapManager.Instance.Map[GridUtils.PositionToIndex(currentPos.Invoke())],
                MapManager.Instance.Map[GridUtils.PositionToIndex(baseHome.Invoke())]);

                positionOnPath = 0;

                if (positionOnPath > path.Count - 1)
                {
                    path = null;
                    return;
                }

                currentDestination = new Vector3(path[positionOnPath].x, path[positionOnPath].y, 0);


                flocking.ToggleFlocking(true);
                flocking.UpdateTarget(new Vector2Int((int)currentDestination.x, (int)currentDestination.y);
            }
            else if (Vector2.Distance(currentDestination,currentPos.Invoke()) < 0.1f)
            {
                if (shouldCalculatePathAgain.Invoke())
                {
                    path = alternatives.GetPath(MapManager.Instance.Map,
                    MapManager.Instance.Map[GridUtils.PositionToIndex(currentPos.Invoke())],
                    MapManager.Instance.Map[GridUtils.PositionToIndex(baseHome.Invoke())]);

                    positionOnPath = 0;

                    currentDestination = new Vector3(path[positionOnPath].x, path[positionOnPath].y, 0);


                    flocking.ToggleFlocking(true);
                    flocking.UpdateTarget(new Vector2Int((int)currentDestination.x, (int)currentDestination.y);
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
                    flocking.UpdateTarget(new Vector2Int((int)currentDestination.x, (int)currentDestination.y);
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
