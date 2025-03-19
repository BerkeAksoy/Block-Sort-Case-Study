using UnityEngine;

public static class Helpers
{
    public static int GetComplementary(int value)
    {
        switch (value)
        {
            case 1:
                return 0;
            case 4:
                return 2;
            case 5:
                return 3;
            case 2:
                return 4;
            case 3:
                return 5;
            default:
                return 1;
        }
    }

    public static readonly Vector2Int[] NeighborDirections =
    {
        new Vector2Int(0, 1),  // Up
        new Vector2Int(0, -1), // Down
        new Vector2Int(1, 0),  // Right
        new Vector2Int(-1, 0)  // Left
    };
}
