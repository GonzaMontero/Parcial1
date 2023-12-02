using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridGeneration : MonoBehaviour
{
    public Vector2Int GridSize;
    private GridSlot[] grid;

    private PathingAlternatives pathingManager;

    private void Start()
    {
        pathingManager = new PathingAlternatives();
        GridUtils.GridSize = GridSize;
        grid = new GridSlot[GridSize.x *  GridSize.y];

        int ID = 0;
        for(int y = 0; y < GridSize.y; y++)
        {
            for(int x = 0; x < GridSize.x; x++)
            {
                grid[ID] = new GridSlot(ID, new Vector2Int(x, y));
                ID++;
            }
        }

        grid[GridUtils.PositionToIndex(new Vector2Int(1, 0))].currentState = GridSlot.SlotState.Obstacle;
        grid[GridUtils.PositionToIndex(new Vector2Int(3, 1))].currentState = GridSlot.SlotState.Obstacle;

        grid[GridUtils.PositionToIndex(new Vector2Int(1, 1))].SetWeight(2);
        grid[GridUtils.PositionToIndex(new Vector2Int(1, 2))].SetWeight(2);
        grid[GridUtils.PositionToIndex(new Vector2Int(1, 3))].SetWeight(2);
        grid[GridUtils.PositionToIndex(new Vector2Int(1, 4))].SetWeight(2);
        grid[GridUtils.PositionToIndex(new Vector2Int(1, 5))].SetWeight(2);
        grid[GridUtils.PositionToIndex(new Vector2Int(1, 6))].SetWeight(2);

        List<Vector2Int> path = pathingManager.GetPath(grid, grid[GridUtils.PositionToIndex(new Vector2Int(0, 0))], grid[GridUtils.PositionToIndex(new Vector2Int(8, 3))]);

        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log(path[i]);
        }
    }
}
