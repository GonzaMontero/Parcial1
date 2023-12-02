using System.Collections.Generic;
using UnityEngine;

public class GridSlot
{
    public enum SlotState
    {
        Open,
        Close,
        Ready,
        Obstacle
    }

    private int id;
    public int ID => id;

    public Vector2Int position;
    public List<int> adjacentPositionsID;

    public SlotState currentState;
    public int idOfOpener;

    public int weight = 1;
    private int originalWeight;
    public int totalWeight;

    public GridSlot(int ID, Vector2Int position)
    {
        id = ID;
        this.position = position;
        adjacentPositionsID = GridUtils.GetAdjacentSlotIDs(position);

        originalWeight = weight;
    }

    public void SetWeight(int weight)
    {
        this.weight = weight;
        originalWeight = weight;
    }

    public void Open(int IDOfOpener, int parentWeight)
    {
        currentState = SlotState.Open;
        idOfOpener = IDOfOpener;
        totalWeight = parentWeight + weight;
    }

    public void Reset()
    {
        if (currentState != SlotState.Obstacle)
        {
            currentState = SlotState.Ready;
            idOfOpener = -1;
            weight = originalWeight;
        }
    }
}
