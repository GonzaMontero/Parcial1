using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using static UnityEditor.VersionControl.Asset;
using Random = UnityEngine.Random;

public class Miner : MonoBehaviour
{
    private Action<Vector2Int> onEmptyMine;
    private FlockingAlgorithm flockingMiners;
    private PathingAlternatives pathingAlternatives;
    private FSM fsm;
    private GridSlot[] map;

    public Vector2 CurrentPos;
    public Vector2Int CurrentMine;
    public bool updatePos;
    public bool updatePath = false;

    public void Init(Vector2Int deposit, Vector2Int rest, Vector2 currentPos, Func<float> onGetDeltaTime, Func<Vector2, Vector2Int> onGetMine,
        Action<Vector2Int> onEmptyMine, ref Action onUpdateWeight, List<Vector2Int> buildings, List<Vector2Int> mines)
    {
        InitMap(buildings, mines);

        pathingAlternatives = new PathingAlternatives();
        CurrentPos = currentPos;

        fsm = new FSM((int)MinerStates._Count, (int)MinerFlags._Count);

        //Base States
        fsm.SetRelation((int)MinerStates.GoToMine, (int)MinerFlags.OnReachMine, (int)MinerStates.Mining);
        fsm.SetRelation((int)MinerStates.Mining, (int)MinerFlags.OnFullInventory, (int)MinerStates.GoToDeposit);
        fsm.SetRelation((int)MinerStates.GoToDeposit, (int)MinerFlags.OnReachDeposit, (int)MinerStates.GoToMine);
        fsm.SetRelation((int)MinerStates.GoToRest, (int)MinerFlags.OnIdle, (int)MinerStates.Idle);

        //Early exit states
        fsm.SetRelation((int)MinerStates.Mining, (int)MinerFlags.OnAbruptReturn, (int)MinerStates.GoToRest);
        fsm.SetRelation((int)MinerStates.GoToMine, (int)MinerFlags.OnAbruptReturn, (int)MinerStates.GoToRest);
        fsm.SetRelation((int)MinerStates.Idle, (int)MinerFlags.OnGoBackToWork, (int)MinerStates.GoToMine);
        fsm.SetRelation((int)MinerStates.GoToRest, (int)MinerFlags.OnGoBackToWork, (int)MinerStates.GoToMine);

        //Behaviours
        fsm.AddBehaviour((int)MinerStates.Idle, new Idle(fsm.SetFlag), () => { fsm.SetFlag((int)MinerFlags.OnGoBackToWork); });
        fsm.AddBehaviour((int)MinerStates.Mining, new Mine(fsm.SetFlag, onGetDeltaTime, OnEmptyMine, StopMovement), () => { fsm.SetFlag((int)MinerFlags.OnAbruptReturn); });
        fsm.AddBehaviour((int)MinerStates.GoToMine, new GoToMine(fsm.SetFlag, GetPos, GetPath, UpdateTarget, onGetMine, UpdateMine, RePath), () => { fsm.SetFlag((int)MinerFlags.OnAbruptReturn); });
        fsm.AddBehaviour((int)MinerStates.GoToDeposit, new GoToDeposit(fsm.SetFlag, GetPos, GetPath, UpdateTarget, deposit, RePath));
        fsm.AddBehaviour((int)MinerStates.GoToRest, new AbruptReturn(fsm.SetFlag, GetPos, GetPath, UpdateTarget, StopMovement, rest), () => { fsm.SetFlag((int)MinerFlags.OnGoBackToWork); });

        fsm.ForceCurrentState((int)MinerStates.GoToMine);

        flockingMiners = GetComponent<FlockingAlgorithm>();
        flockingMiners.Init(UpdatePos, GetPos);

        this.onEmptyMine = onEmptyMine;
        onUpdateWeight = OnUpdateWeight;
    }

    public void UpdateMiner()
    {
        fsm.Update();
    }

    public void ExitMiner()
    {
        fsm.Exit();
    }

    public void UpdateWeight(Vector2Int slotPos, int slotWeight)
    {
        for (int i = 0; i < map.Length; i++)
        {
            if (map[i].position == slotPos)
            {
                map[i].SetWeight(slotWeight);
                updatePath = true;
                return;
            }
        }
    }

    private void UpdatePos(Vector2 newPos)
    {
        updatePos = true;
        CurrentPos = newPos;
    }

    private void UpdateTarget(Vector2Int newTarget)
    {
        flockingMiners.ToggleFlocking(true);
        flockingMiners.UpdateTarget(newTarget);
    }

    private void StopMovement()
    {
        flockingMiners.ToggleFlocking(false);
    }

    private Vector2 GetPos()
    {
        return CurrentPos;
    }

    private List<Vector2Int> GetPath(Vector2Int origin, Vector2Int destination)
    {
        updatePath = false;
        return pathingAlternatives.GetPath(map, map[GridUtils.PositionToIndex(origin)], map[GridUtils.PositionToIndex(destination)]);
    }

    public void UpdateMine(Vector2Int minePos)
    {
        CurrentMine = minePos;
        updatePath = true;
    }

    private void OnEmptyMine()
    {
        updatePath = true;
        onEmptyMine?.Invoke(CurrentMine);
    }

    private void OnUpdateWeight()
    {
        updatePath = true;
    }

    private bool RePath()
    {
        return updatePath;
    }

    private void InitMap(List<Vector2Int> buildings, List<Vector2Int> mines)
    {
        map = new GridSlot[50 * 50];
        GridUtils.GridSize = new Vector2Int(50, 50);
        int id = 0;

        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                map[id] = new GridSlot(id, new Vector2Int(j, i));
                map[id].SetWeight(Random.Range(1, 6));

                for (int k = 0; k < buildings.Count; k++)
                {
                    if (map[id].position == buildings[k] && !mines.Contains(buildings[k]))
                    {
                        map[id].currentState = GridSlot.SlotState.Obstacle;
                    }
                }

                id++;
            }
        }
    }
}
