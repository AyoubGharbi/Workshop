using System;

[Flags]
public enum BlockState
{
    Left = 1, // 0001
    Right = 2, // 0010
    Down = 4, // 0100
    Up = 8 // 1000
}

public static class GridGenerator
{
    public static BlockState[,] GenerateGrid(int width, int height)
    {
        BlockState[,] grid = new BlockState[width, height];

        BlockState initialState = BlockState.Left | BlockState.Right | BlockState.Down | BlockState.Up;

        for (int x = 0; x < height; x++)
        {
            for (int z = 0; z < width; z++)
            {
                grid[x, z] = initialState;
            }
        }

        return grid;
    }
}