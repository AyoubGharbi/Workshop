using static GridGenerator;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AShortestPathAlgorithm : ScriptableObject
{
    public abstract List<NeighborPosition> FindPath(BlockState[,] grid, NeighborPosition startPos,
        NeighborPosition endPosition);

    public List<NeighborPosition> GetValidNeighbors(GridDataSO gridDataSo, BlockState[,] grid, int currentX, int currentZ)
    {
        List<Neighbor> tempNeighbors = new List<Neighbor>();
        Neighbor tempNeighbor = new Neighbor();

        int width = gridDataSo.GridWidth;
        int height = gridDataSo.GridHeight;

        // RIGHT
        if (currentX < width - 1)
        {
            tempNeighbor.Position.X = currentX + 1;
            tempNeighbor.Position.Z = currentZ;
            tempNeighbor.sharedBlock = BlockState.Right;

            if (!grid[currentX, currentZ].HasFlag(BlockState.Right))
                tempNeighbors.Add(tempNeighbor);
        }

        // LEFT
        if (currentX > 0)
        {
            tempNeighbor.Position.X = currentX - 1;
            tempNeighbor.Position.Z = currentZ;
            tempNeighbor.sharedBlock = BlockState.Left;

            if (!grid[currentX, currentZ].HasFlag(BlockState.Left))
                tempNeighbors.Add(tempNeighbor);
        }

        // UP
        if (currentZ < height - 1)
        {
            tempNeighbor.Position.X = currentX;
            tempNeighbor.Position.Z = currentZ + 1;
            tempNeighbor.sharedBlock = BlockState.Up;

            if (!grid[currentX, currentZ].HasFlag(BlockState.Up))
                tempNeighbors.Add(tempNeighbor);
        }

        // DOWN
        if (currentZ > 0)
        {
            tempNeighbor.Position.X = currentX;
            tempNeighbor.Position.Z = currentZ - 1;
            tempNeighbor.sharedBlock = BlockState.Down;

            if (!grid[currentX, currentZ].HasFlag(BlockState.Down))
                tempNeighbors.Add(tempNeighbor);
        }

        return tempNeighbors.Select(tn => tn.Position).ToList();
    }
}