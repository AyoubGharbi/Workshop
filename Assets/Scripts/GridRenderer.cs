using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [Range(1, 10)] [SerializeField] private int gridWidth;
    [Range(1, 10)] [SerializeField] private int gridHeight;
    [SerializeField] private GameObject cellBlockPrefab;

    private BlockState[,] _grid;
    private List<Vector3> _gridCellPositions = new List<Vector3>();
    private List<GameObject> _gridCellInstances = new List<GameObject>();

    private void Start()
    {
        RenderGrid();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;

        RenderGrid();
    }

    private void CleanUp()
    {
        _gridCellInstances.ForEach(Destroy);
        _gridCellInstances = new List<GameObject>();
        _gridCellPositions = new List<Vector3>();
    }

    private void RenderGrid()
    {
        CleanUp();

        _grid = GridGenerator.GenerateGrid(gridWidth, gridHeight);

        float cellSize = cellBlockPrefab.transform.localScale.z;
        float halfCellSize = cellSize * 0.5f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                BlockState gridCellState = _grid[x, z];
                Vector3 gridCellCenterPos = new Vector3(x * cellSize, 0, z * cellSize);

                _gridCellPositions.Add(gridCellCenterPos);

                if (gridCellState.HasFlag(BlockState.Left))
                {
                    Vector3 cellPos = gridCellCenterPos + new Vector3(-halfCellSize, 0, 0);
                    CreateBlock(cellPos, Vector3.zero);
                }

                if (gridCellState.HasFlag(BlockState.Up))
                {
                    Vector3 rotationAngle = new Vector3(0, 90, 0);
                    Vector3 cellPos = gridCellCenterPos + new Vector3(0, 0, halfCellSize);
                    CreateBlock(cellPos, rotationAngle);
                }

                if (gridCellState.HasFlag(BlockState.Right))
                {
                    Vector3 rotationAngle = new Vector3(0, 180, 0);
                    Vector3 cellPos = gridCellCenterPos + new Vector3(halfCellSize, 0, 0);
                    CreateBlock(cellPos, rotationAngle);
                }

                if (gridCellState.HasFlag(BlockState.Down))
                {
                    Vector3 rotationAngle = new Vector3(0, -90, 0);
                    Vector3 cellPos = gridCellCenterPos + new Vector3(0, 0, -halfCellSize);
                    CreateBlock(cellPos, rotationAngle);
                }
            }
        }
    }

    private void CreateBlock(Vector3 position, Vector3 angle)
    {
        GameObject cellBlock = Instantiate(cellBlockPrefab, this.transform);
        cellBlock.transform.eulerAngles = angle;
        cellBlock.transform.position = position;

        _gridCellInstances.Add(cellBlock);
    }

    private void OnDrawGizmos()
    {
        if (_gridCellPositions is not { Count: > 0 }) return;

        Gizmos.color = Color.magenta;
        foreach (Vector3 gridCellPos in _gridCellPositions)
            Gizmos.DrawWireSphere(gridCellPos, 0.25f);
    }
}