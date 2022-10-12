using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Flags]
public enum BlockState
{
    Left = 1, // 0000 0001
    Right = 2, // 0000 0010
    Down = 4, // 0000 0100
    Up = 8, // 0000 1000
    Explored = 16 // 0001 1111
}

public static class GridGenerator
{
    public static BlockState[,] GenerateGrid(int width, int height)
    {
        BlockState[,] grid = new BlockState[width, height];

        BlockState initialState = BlockState.Left | BlockState.Right | BlockState.Down | BlockState.Up;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                grid[x, z] = initialState;
            }
        }

        return GenerateMazeWithBacktracking(grid, width, height);
    }

    public static BlockState[,] GenerateMazeWithBacktracking(BlockState[,] grid, int width, int height)
    {
        Stack<NeighborPosition> neighborsStack = new Stack<NeighborPosition>();
        NeighborPosition initPosition = new NeighborPosition(0, 0);
        neighborsStack.Push(initPosition);

        while (neighborsStack.Count > 0)
        {
            NeighborPosition currentPosition = neighborsStack.Pop();
            grid[currentPosition.X, currentPosition.Z] |= BlockState.Explored;

            List<Neighbor> unExploredNeighbors =
                GetUnExploredNeighbors(grid, width, height, currentPosition.X, currentPosition.Z);

            if (unExploredNeighbors.Count > 0)
            {
                if (unExploredNeighbors.Count > 1)
                    neighborsStack.Push(currentPosition);

                int randomNeighborIndex = Random.Range(0, unExploredNeighbors.Count);
                Neighbor randomNeighbor = unExploredNeighbors[randomNeighborIndex];

                grid[currentPosition.X, currentPosition.Z] &= ~randomNeighbor.sharedBlock;
                grid[randomNeighbor.Position.X, randomNeighbor.Position.Z] &=
                    ~GetOppositeBlock(randomNeighbor.sharedBlock); // ~ => complement of the bits

                grid[randomNeighbor.Position.X, randomNeighbor.Position.Z] |= BlockState.Explored;

                neighborsStack.Push(randomNeighbor.Position);
            }
        }

        return grid;
    }

    public static BlockState GetOppositeBlock(BlockState blockState)
    {
        switch (blockState)
        {
            case BlockState.Left:
                return BlockState.Right;
            case BlockState.Right:
                return BlockState.Left;
            case BlockState.Down:
                return BlockState.Up;
            case BlockState.Up:
                return BlockState.Down;
            default:
                throw new ArgumentOutOfRangeException(nameof(blockState), blockState, null);
        }
    }


    public struct NeighborPosition
    {
        public int X;
        public int Z;

        public NeighborPosition(int x, int z)
        {
            X = x;
            Z = z;
        }
    }

    public struct Neighbor
    {
        public NeighborPosition Position;
        public BlockState sharedBlock;
    }

    public static List<Neighbor> GetUnExploredNeighbors(BlockState[,] grid, int width, int height, int currentX,
        int currentZ)
    {
        List<Neighbor> tempNeighbors = new List<Neighbor>();
        Neighbor tempNeighbor = new Neighbor();

        // RIGHT
        if (currentX < width - 1)
        {
            tempNeighbor.Position.X = currentX + 1;
            tempNeighbor.Position.Z = currentZ;
            tempNeighbor.sharedBlock = BlockState.Right;

            BlockState neighborState = grid[tempNeighbor.Position.X, tempNeighbor.Position.Z];
            if (!neighborState.HasFlag(BlockState.Explored))
                tempNeighbors.Add(tempNeighbor);
        }

        // LEFT
        if (currentX > 0)
        {
            tempNeighbor.Position.X = currentX - 1;
            tempNeighbor.Position.Z = currentZ;
            tempNeighbor.sharedBlock = BlockState.Left;

            BlockState neighborState = grid[tempNeighbor.Position.X, tempNeighbor.Position.Z];
            if (!neighborState.HasFlag(BlockState.Explored))
                tempNeighbors.Add(tempNeighbor);
        }

        // UP
        if (currentZ < height - 1)
        {
            tempNeighbor.Position.X = currentX;
            tempNeighbor.Position.Z = currentZ + 1;
            tempNeighbor.sharedBlock = BlockState.Up;

            BlockState neighborState = grid[tempNeighbor.Position.X, tempNeighbor.Position.Z];
            if (!neighborState.HasFlag(BlockState.Explored))
                tempNeighbors.Add(tempNeighbor);
        }

        // DOWN
        if (currentZ > 0)
        {
            tempNeighbor.Position.X = currentX;
            tempNeighbor.Position.Z = currentZ - 1;
            tempNeighbor.sharedBlock = BlockState.Down;

            BlockState neighborState = grid[tempNeighbor.Position.X, tempNeighbor.Position.Z];
            if (!neighborState.HasFlag(BlockState.Explored))
                tempNeighbors.Add(tempNeighbor);
        }

        return tempNeighbors;
    }
}