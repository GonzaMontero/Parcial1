using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.FSM;
using System;
using AI.Entities;
using AI.Managers;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using static UnityEngine.GraphicsBuffer;

public class MineState : FSMAction
{
    public GridSlot[] map;
    public float currentDeltaTime = 0f;

    public override List<Action> OnEnterBehaviours(FSMParameters onEnterParameters)
    {
        EntityData data = onEnterParameters.Parameters[0] as EntityData;
        PathingAlternatives pathingAlternatives = onEnterParameters.Parameters[2] as PathingAlternatives;
        List<Vector2Int> path = onEnterParameters.Parameters[3] as List<Vector2Int>;

        int timesMined = Convert.ToInt32(onEnterParameters.Parameters[4]);
        int resourcesMined = Convert.ToInt32(onEnterParameters.Parameters[5]);

        map = onEnterParameters.Parameters[6] as GridSlot[];

        MineItem currentMine = data.targetMine;

        List<Action> onExecuteActions = new List<Action>();

        onExecuteActions.Add(() =>
        {
            if (MapManager.Instance.GetItemOnPosition(data.Target) != null)
            {
                data.targetMine = MapManager.Instance.GetItemOnPosition(data.Target);
                data.targetMine.StartWorking();
                onEnterParameters.Parameters[0] = data;
            }
        });    
        
        return onExecuteActions;
    }

    public override List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters)
    {
        EntityData data = onExecuteParameters.Parameters[0] as EntityData;
        PathingAlternatives pathingAlternatives = onExecuteParameters.Parameters[2] as PathingAlternatives;
        List<Vector2Int> path = onExecuteParameters.Parameters[3] as List<Vector2Int>;
        
        int timesMined = Convert.ToInt32(onExecuteParameters.Parameters[4]);
        int resourcesMined = Convert.ToInt32(onExecuteParameters.Parameters[5]);

        map = onExecuteParameters.Parameters[6] as GridSlot[];

        MineItem currentMine = data.targetMine;

        List<Action> onExecuteActions = new List<Action>();
        onExecuteActions.Add(() =>
        {
            

            if (MapManager.Instance.GetItemOnPosition(data.Target) != null)
            {
                data.targetMine = MapManager.Instance.GetItemOnPosition(data.Target);
                onExecuteParameters.Parameters[0] = data;
            }
            else
            {
                data.targetMine = null;
                onExecuteParameters.Parameters[0] = data;
            }

            if(resourcesMined >= 15 || MapManager.Instance.AllMinesOnMap.Count <= 0 || MinerManager.Instance.ReturnToBase)
            {
                SwapFlags((int)MinerFlags.OnInventoryFull);               
                return;
            }
            if (data.targetMine == null)
            {
                data.Target = MinerManager.Instance.PopulationTypes[0].PopulationVoronoiHandler.GetNearestMine(data.Position);
                onExecuteParameters.Parameters[0] = data;

                path = pathingAlternatives.GetPath(map,
                map[GridUtils.PositionToIndex(new Vector2Int((int)data.Position.x, (int)data.Position.y))],
                map[GridUtils.PositionToIndex(data.Target)]);

                onExecuteParameters.Parameters[3] = path;                
            }
            if (timesMined <= 3)
            {
                currentDeltaTime += MinerManager.Instance.DeltaTime;

                if (currentDeltaTime < 3f)
                    return;
                else
                    currentDeltaTime = 0;

                if (data.targetMine == null)
                    return;

                resourcesMined += data.targetMine.TryMine(1);
                timesMined += 1;
                
                onExecuteParameters.Parameters[5] = resourcesMined;
            }
            if (timesMined >= 3)
            {
                bool canEat = data.targetMine.TryEat();

                if (canEat)
                    timesMined = 0;

                onExecuteParameters.Parameters[4] = timesMined;
            }           
        });

        return onExecuteActions;
    }

    public override List<Action> OnExitBehaviours(FSMParameters onExitParameters)
    {
        EntityData data = onExitParameters.Parameters[0] as EntityData;

        List<Action> onExitActions = new List<Action>();

        onExitActions.Add(() =>
        {
            if (MapManager.Instance.GetItemOnPosition(data.Target) != null)
            {
                data.targetMine = MapManager.Instance.GetItemOnPosition(data.Target);
                data.targetMine.StopWorking();
                onExitParameters.Parameters[0] = data;
            }
        });

        return onExitActions;
    }

    public override void SwapFlags(int flags)
    {
        OnSetFlag?.Invoke(flags);
    }
}
