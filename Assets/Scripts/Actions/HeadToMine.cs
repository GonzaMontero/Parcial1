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
            target = MinerManager.Instance.PopulationTypes[0].PopulationVoronoiHandler.GetClosestVoronoi();
            onEnterParameters.Parameters[8] = target;

            List<Vector2Int> pathToMine = new List<Vector2Int>();

            onEnterParameters.Parameters[4] = alternatives.GetPath(MapManager.Instance.Map,
                MapManager.Instance.Map[GridUtils.PositionToIndex(currentPos.Invoke())],
                MapManager.Instance.Map[GridUtils.PositionToIndex(target)]);
        });

        return onEnterActions;
    }

    public override List<Action> OnExecuteBehaviours(FSMParameters onExecuteParameters)
    {
        throw new NotImplementedException();
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
