using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathingAlternatives {

    enum PathingTypes
    {
        BreadthFirst,
        DepthFirst,
        Dijkstra,
        AStar
    }

    private PathingTypes currentPathingTypes = PathingTypes.AStar;
    private List<int> openSlotsID = new List<int>();
    private List<int> closedSlotsID = new List<int>();
    private Vector2Int destinationPosition;

    public List<Vector2Int> GetPath(GridSlot[] grid, GridSlot origin, GridSlot destination)
    {
        openSlotsID.Add(origin.ID);
        destinationPosition = destination.position;

        GridSlot currentSlot = origin;
        while(currentSlot.position != destination.position)
        {
            currentSlot = GetNextSlot(grid, currentSlot);

            if(currentSlot == null)
            {
                return new List<Vector2Int>();
            }

            for(int i = 0; i < currentSlot.adjacentPositionsID.Count; i++)
            {
                if (currentSlot.adjacentPositionsID[i] != GridUtils.invalidPosition)
                {
                    if (grid[currentSlot.adjacentPositionsID[i]].currentState == GridSlot.SlotState.Ready)
                    {
                        grid[currentSlot.adjacentPositionsID[i]].Open(currentSlot.ID, currentSlot.totalWeight);
                        openSlotsID.Add(grid[currentSlot.adjacentPositionsID[i]].ID);
                    }
                }
            }

            currentSlot.currentState = GridSlot.SlotState.Close;
            openSlotsID.Remove(currentSlot.ID);
            closedSlotsID.Add(currentSlot.ID);
        }

        List<Vector2Int> path = GeneratePath(grid, currentSlot);

        foreach(GridSlot slot in grid)
        {
            slot.Reset();
            openSlotsID.Clear();
            closedSlotsID.Clear();
        }

        return path;
    }

    private List<Vector2Int> GeneratePath(GridSlot[] grid,  GridSlot currentSlot)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        while (currentSlot.idOfOpener != -1)
        {
            path.Add(currentSlot.position);
            currentSlot = grid[currentSlot.idOfOpener];
        }

        path.Add(currentSlot.position);
        path.Reverse();

        return path;
    }

    private GridSlot GetNextSlot(GridSlot[] grid, GridSlot currentSlot)
    {
        switch(currentPathingTypes)
        {
            case PathingTypes.BreadthFirst:
                return grid[openSlotsID[0]];
            case PathingTypes.DepthFirst:
                return grid[openSlotsID[openSlotsID.Count - 1]];
            case PathingTypes.Dijkstra:
                {
                    GridSlot n = null;
                    int currentMaxWeight = int.MaxValue;

                    for (int i = 0; i < openSlotsID.Count; i++)
                    {
                        if (grid[openSlotsID[i]].totalWeight < currentMaxWeight)
                        {
                            n = grid[openSlotsID[i]];
                            currentMaxWeight = grid[openSlotsID[i]].totalWeight;
                        }
                    }

                    return n;
                }
            case PathingTypes.AStar:
                {
                    GridSlot n = null;
                    int currentMaxWeight = int.MaxValue;

                    for (int i = 0; i < openSlotsID.Count; i++)
                    {
                        if (grid[openSlotsID[i]].totalWeight + GetManhattanDistance(grid[openSlotsID[i]].position, destinationPosition) 
                            < currentMaxWeight)
                        {
                            n = grid[openSlotsID[i]];
                            currentMaxWeight = grid[openSlotsID[i]].totalWeight;
                        }
                    }

                    return n;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private int GetManhattanDistance(Vector2Int origin, Vector2Int destination)
    {
        int disX = Mathf.Abs(origin.x - destination.x);
        int disY = Mathf.Abs(origin.y - destination.y);

        return disX + disY;
    }
}
