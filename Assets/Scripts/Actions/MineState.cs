using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.FSM;
using System;
using AI.Entities;
using AI.Managers;

public class MineState : FSMAction
{
    public override List<Action> OnEnterBehaviours(FSMParameters onEnterParameters)
    {
        return new List<Action>();
    }

    public override List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters)
    {
        EntityData data = onExecuteParameters.Parameters[0] as EntityData;
        PathingAlternatives pathingAlternatives = onExecuteParameters.Parameters[2] as PathingAlternatives;
        List<Vector2Int> path = onExecuteParameters.Parameters[3] as List<Vector2Int>;
        
        int timesMined = Convert.ToInt32(onExecuteParameters.Parameters[4]);
        int resourcesMined = Convert.ToInt32(onExecuteParameters.Parameters[5]);

        MineItem currentMine = data.targetMine;

        List<Action> onExecuteActions = new List<Action>();
        onExecuteActions.Add(() =>
        {
            if(resourcesMined >= 15 || MapManager.Instance.AllMinesOnMap.Count <= 0 || MinerManager.Instance.ReturnToBase)
            {
                SwapFlags((int)MinerFlags.OnInventoryFull);
                return;
            }
            if (data.targetMine == null)
            {
                MinerManager.Instance.PopulationTypes[0].PopulationVoronoiHandler.UpdateActiveVoronoi(MapManager.Instance.AllMinesOnMap);
                data.Target = MapManager.Instance.Map[MinerManager.Instance.PopulationTypes[0].PopulationVoronoiHandler.GetClosestMine(
                    GridUtils.PositionToIndex(data.Position))].position;
                onExecuteParameters.Parameters[0] = data;

                path = pathingAlternatives.GetPath(MapManager.Instance.Map, MapManager.Instance.Map[GridUtils.PositionToIndex(data.Position)],
                    MapManager.Instance.Map[GridUtils.PositionToIndex(data.Target)], out int totalCost);
                onExecuteParameters.Parameters[3] = path;
            }
            if(Vector2.Distance(data.Position, data.Target) > 0.6f)
            {
                SwapFlags((int)MinerFlags.OnMineDepleted);
            }
            if(timesMined < 3 && Vector2.Distance(data.Position, data.Target) < 0.5f)
            {
                resourcesMined += data.targetMine.TryMine(1);
                timesMined += 1;

                if (timesMined >= 3)
                {
                    bool canEat = data.targetMine.TryEat();

                    if(canEat)
                        timesMined = 0;
                }

                onExecuteParameters.Parameters[4] = timesMined;
                onExecuteParameters.Parameters[5] = resourcesMined;
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
