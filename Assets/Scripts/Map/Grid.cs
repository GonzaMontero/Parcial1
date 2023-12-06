using System.Collections.Generic;
using UnityEngine;

public class GridSlot
{
    private int id;
    public int ID => id;

    public Vector2Int position;
    public List<int> adjacentPositionsID;

    public SlotStates currentState;
    public int idOfOpener;

    public int weight = 1;
    private int originalWeight;
    public int totalWeight;

    public GridSlot(int ID, Vector2Int position)
    {
        id = ID;
        this.position = position;
        adjacentPositionsID = GridUtils.GetAdjacentSlotIDs(position);

        this.currentState = SlotStates.Ready;
        idOfOpener = -1;

        originalWeight = weight;
    }

    public void SetWeight(int weight)
    {
        this.weight = weight;
        originalWeight = weight;
    }

    public void Open(int IDOfOpener, int parentWeight)
    {
        currentState = SlotStates.Open;
        idOfOpener = IDOfOpener;
        totalWeight = parentWeight + weight;
    }

    public void Reset()
    {
        if (currentState != SlotStates.Obstacle)
        {
            currentState = SlotStates.Ready;
            idOfOpener = -1;
            weight = originalWeight;
        }
    }
}
