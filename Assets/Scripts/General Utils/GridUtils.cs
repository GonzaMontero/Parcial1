using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUtils
{
    public static Vector2Int GridSize;
    public const int invalidPosition = -1;

    public static List<int> GetAdjacentSlotIDs(Vector2Int position)
    {
        List<int> IDs = new List<int>();
        IDs.Add(PositionToIndex(new Vector2Int(position.x + 1, position.y)));
        IDs.Add(PositionToIndex(new Vector2Int(position.x, position.y - 1)));
        IDs.Add(PositionToIndex(new Vector2Int(position.x - 1, position.y)));
        IDs.Add(PositionToIndex(new Vector2Int(position.x, position.y + 1)));
        return IDs;
    }

    public static int PositionToIndex(Vector2Int position)
    {
        if (position.x < 0 || position.x >= GridSize.x ||
            position.y < 0 || position.y >= GridSize.y)
            return -1;
        return position.y * GridSize.x + position.x;
    }
}
