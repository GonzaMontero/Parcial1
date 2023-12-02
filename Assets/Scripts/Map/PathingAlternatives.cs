using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathingAlternatives {

    enum PathingTypes
    {
        BreadthFirst, //Alongside Depthfirst calculates the nodes in a specific direction and only checks for non occupied nodes
        DepthFirst, //These are not usually optimal, but they are light on calculations, and don´t tend to choose the shortest path
        Dijkstra, //This algorithm pick the unvisited slot with the lowest distance and chooses the route with the lowest cost
        AStar //Very similar algorithm to dijkstra, but uses GetManhattanDistance to search exclusively in the direction of the target
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
        while(currentSlot.position != destination.position) //Check if we did not reach the destination yet
        {
            currentSlot = GetNextSlot(grid, currentSlot); // Get next slot from our currentSlot

            if(currentSlot == null) //if currentslot does not exist then the path by extension does not either
            {
                return new List<Vector2Int>();
            }

            for(int i = 0; i < currentSlot.adjacentPositionsID.Count; i++) //for each position adjacent to our cell we check if it is available
            {
                if (currentSlot.adjacentPositionsID[i] != GridUtils.invalidPosition)
                {
                    if (grid[currentSlot.adjacentPositionsID[i]].currentState == SlotStates.Ready) //if available we open it and add it to
                                                                                                           //the list of possible nodes
                    {
                        grid[currentSlot.adjacentPositionsID[i]].Open(currentSlot.ID, currentSlot.totalWeight);
                        openSlotsID.Add(grid[currentSlot.adjacentPositionsID[i]].ID);
                    }
                }
            }

            currentSlot.currentState = SlotStates.Close; //close current slot and advance to the next one
            openSlotsID.Remove(currentSlot.ID);
            closedSlotsID.Add(currentSlot.ID);
        }

        List<Vector2Int> path = GeneratePath(grid, currentSlot); //generate a path with the closed slots and current position

        //reset all slots and return the finished path
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

        while (currentSlot.idOfOpener != -1) //check for currentslots which have the correct traversal id
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
        switch(currentPathingTypes) //depending on which algorithm we use, we get a different position
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
                            //Get lowest weight position slot
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
                            //Get lowest weight position slot which also points torwards the destination
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
