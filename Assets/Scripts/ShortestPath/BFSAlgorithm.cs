using System.Collections.Generic;
using UnityEngine;
using static GridGenerator;

[CreateAssetMenu(menuName = "Scriptables/ShortestPaths/" + nameof(BFSAlgorithm), fileName = nameof(BFSAlgorithm))]
public class BFSAlgorithm : AShortestPathAlgorithm
{
    public override List<NeighborPosition> FindPath(GridDataSO gridDataSO, BlockState[,] grid,
        GridGenerator.NeighborPosition startPos, NeighborPosition endPosition)
    {
        // BFS = Queue FIFO
        // DFS = Stack LIFO
        List<NeighborPosition> finalPath = new List<NeighborPosition>();
        Dictionary<NeighborPosition, NeighborPosition> neighborsTrace =
            new Dictionary<NeighborPosition, NeighborPosition>();

        Queue<NeighborPosition> npQueue = new Queue<NeighborPosition>();;
        List<NeighborPosition> exploredNeighbors = new List<NeighborPosition>();

        npQueue.Enqueue(startPos);

        while (npQueue.Count != 0)
        {
            NeighborPosition current = npQueue.Dequeue();

            List<NeighborPosition> validNeighbors =
                GetValidNeighbors(gridDataSO, grid, current.X, current.Z);

            foreach (NeighborPosition neighbor in validNeighbors)
            {
                if (!exploredNeighbors.Contains(neighbor))
                {
                    exploredNeighbors.Add(neighbor);
                    neighborsTrace.Add(neighbor, current);
                    npQueue.Enqueue(neighbor);
                }
            }
        }



    }
}