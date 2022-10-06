using System;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private GameObject cellBlockPrefab;

    private void Start()
    {
        RenderGrid();
    }

    private void RenderGrid()
    {
        BlockState[,] grid = GridGenerator.GenerateGrid(gridWidth, gridHeight);

        float cellSize = cellBlockPrefab.transform.localScale.z;
        float halfCellSize = cellSize * 0.5f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                BlockState gridCellState = grid[x, z];
                Vector3 gridCellCenterPos = new Vector3(x * cellSize, 0, z * cellSize);

                if (gridCellState.HasFlag(BlockState.Left))
                {
                    GameObject cellBlock = Instantiate(cellBlockPrefab, this.transform);
                    Vector3 cellPos = gridCellCenterPos + new Vector3(-halfCellSize, 0, 0);
                    cellBlock.transform.position = cellPos;
                }

                if (gridCellState.HasFlag(BlockState.Up))
                {
                    GameObject cellBlock = Instantiate(cellBlockPrefab, this.transform);
                    Vector3 rotationAngle = new Vector3(0, 90, 0);
                    cellBlock.transform.eulerAngles = rotationAngle;
                    Vector3 cellPos =  gridCellCenterPos + new Vector3(0, 0, halfCellSize);
                    cellBlock.transform.position = cellPos;
                }

                if (gridCellState.HasFlag(BlockState.Right))
                {
                    GameObject cellBlock = Instantiate(cellBlockPrefab, this.transform);
                    Vector3 rotationAngle = new Vector3(0, 180, 0);
                    cellBlock.transform.eulerAngles = rotationAngle;
                    Vector3 cellPos =  gridCellCenterPos + new Vector3(halfCellSize, 0,0 );
                    cellBlock.transform.position = cellPos;
                }

                if (gridCellState.HasFlag(BlockState.Down))
                {
                    GameObject cellBlock = Instantiate(cellBlockPrefab, this.transform);
                    Vector3 rotationAngle = new Vector3(0, -90, 0);
                    cellBlock.transform.eulerAngles = rotationAngle;
                    Vector3 cellPos =  gridCellCenterPos + new Vector3(0, 0,-halfCellSize);
                    cellBlock.transform.position = cellPos;
                }
            }
        }
    }
}