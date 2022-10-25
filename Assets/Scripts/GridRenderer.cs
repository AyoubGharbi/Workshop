using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [SerializeField] private GridDataSO _gridData;
    [SerializeField] private AShortestPathAlgorithm _shortestPathAlgorithm;

    private CancellationToken _cancellationToken;
    private GridGenerator.NeighborPosition _currentPosition;


    private List<Vector3> _gridCellPositions = new List<Vector3>();
    private List<GameObject> _gridCellInstances = new List<GameObject>();

    private void Awake()
    {
        GridGenerator.OnGridChanged += OnGridChanged;
    }

    private void OnGridChanged(GridGenerator.EventGridData eventGridData)
    {
        _currentPosition = eventGridData.CurrentPosition;
        RenderGrid(eventGridData.GridData, _gridData);
    }

    public async void EditorRefreshGrid()
    {
        // CancelProcessedGrid();
        BlockState[,] initialGrid = await DefineGrid(_gridData);
        RenderGrid(initialGrid, _gridData);
    }

    private void OnDestroy() => CancelProcessedGrid();

    private void CancelProcessedGrid()
        => _gridData.CancellationTokenSource?.Cancel();

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        EditorRefreshGrid();
    }

    private void CleanUp()
    {
        _gridCellInstances.ForEach(Destroy);
        _gridCellInstances = new List<GameObject>();
        _gridCellPositions = new List<Vector3>();
    }

    private async Task<BlockState[,]> DefineGrid(GridDataSO gridData)
    {
        return await GridGenerator.GenerateGrid(gridData);
    }

    private void RenderGrid(BlockState[,] grid, GridDataSO gridData)
    {
        CleanUp();

        float cellSize = gridData.CellSize;
        float halfCellSize = gridData.HalfCellSize;

        for (int x = 0; x < gridData.GridWidth; x++)
        {
            for (int z = 0; z < gridData.GridHeight; z++)
            {
                BlockState gridCellState = grid[x, z];
                Vector3 gridCellCenterPos = new Vector3(x * cellSize, 0, z * cellSize);

                _gridCellPositions.Add(gridCellCenterPos);

                if (gridCellState.HasFlag(BlockState.Left))
                {
                    Vector3 cellPos = gridCellCenterPos + new Vector3(-halfCellSize, 0, 0);
                    CreateBlock(gridData.CellBlockPrefab, cellPos, Vector3.zero);
                }

                if (gridCellState.HasFlag(BlockState.Up))
                {
                    Vector3 rotationAngle = new Vector3(0, 90, 0);
                    Vector3 cellPos = gridCellCenterPos + new Vector3(0, 0, halfCellSize);
                    CreateBlock(gridData.CellBlockPrefab, cellPos, rotationAngle);
                }

                if (gridCellState.HasFlag(BlockState.Right))
                {
                    Vector3 rotationAngle = new Vector3(0, 180, 0);
                    Vector3 cellPos = gridCellCenterPos + new Vector3(halfCellSize, 0, 0);
                    CreateBlock(gridData.CellBlockPrefab, cellPos, rotationAngle);
                }

                if (gridCellState.HasFlag(BlockState.Down))
                {
                    Vector3 rotationAngle = new Vector3(0, -90, 0);
                    Vector3 cellPos = gridCellCenterPos + new Vector3(0, 0, -halfCellSize);
                    CreateBlock(gridData.CellBlockPrefab, cellPos, rotationAngle);
                }
            }
        }
    }

    private void CreateBlock(GameObject blockPrefab, Vector3 position, Vector3 angle)
    {
        GameObject cellBlock = Instantiate(blockPrefab, this.transform);
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

        Gizmos.color = Color.cyan;
        Vector3 currentCenterPos = new Vector3(_currentPosition.X * _gridData.CellSize, 0,
            _currentPosition.Z * _gridData.CellSize);
        Gizmos.DrawCube(currentCenterPos, Vector3.one * 0.5f);
    }
}