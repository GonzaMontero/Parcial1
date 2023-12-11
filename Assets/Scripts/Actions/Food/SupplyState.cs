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
}