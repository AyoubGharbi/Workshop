using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [Range(1, 10000)] [SerializeField] private int gridRandomizationSeed;
    [Range(100, 5000)] [SerializeField] private int gridAnimationDelay;
    [Range(1, 10)] [SerializeField] private int gridWidth;
    [Range(1, 10)] [SerializeField] private int gridHeight;
    [SerializeField] private GameObject cellBlockPrefab;

    private CancellationToken _cancellationToken;
    private GridGenerator.NeighborPosition _currentPosition;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private List<Vector3> _gridCellPositions = new List<Vector3>();
    private List<GameObject> _gridCellInstances = new List<GameObject>();

    private float CellSize => cellBlockPrefab.transform.localScale.z;

    private void Awake()
    {
        _cancellationToken = _cancellationTokenSource.Token;
        GridGenerator.OnGridChanged += OnGridChanged;
    }

    private void OnGridChanged(GridGenerator.EventGridData eventGridData)
    {
        _currentPosition = eventGridData.CurrentPosition;
        RenderGrid(eventGridData.GridData);
    }

    private async void Start()
    {
        EditorRefreshGrid();
    }

    public async void EditorRefreshGrid()
    {
        BlockState[,] initialGrid = await DefineGrid(gridAnimationDelay, _cancellationToken, gridRandomizationSeed);
        RenderGrid(initialGrid);
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
    }

    // private void OnValidate()
    // {
    //     if (!Application.isPlaying) return;
    //
    //     RenderGrid();
    // }

    private void CleanUp()
    {
        _gridCellInstances.ForEach(Destroy);
        _gridCellInstances = new List<GameObject>();
        _gridCellPositions = new List<Vector3>();
    }

    private async Task<BlockState[,]> DefineGrid(int animationDelay, CancellationToken token, int randomizationSeed)
    {
        return await GridGenerator.GenerateGrid(gridWidth, gridHeight, animationDelay, token, randomizationSeed);
    }

    private void RenderGrid(BlockState[,] grid)
    {
        CleanUp();

        float halfCellSize = CellSize * 0.5f;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                BlockState gridCellState = grid[x, z];
                Vector3 gridCellCenterPos = new Vector3(x * CellSize, 0, z * CellSize);

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

        Gizmos.color = Color.cyan;
        Vector3 currentCenterPos = new Vector3(_currentPosition.X * CellSize, 0,  _currentPosition.Z * CellSize);
        Gizmos.DrawCube(currentCenterPos, Vector3.one * 0.5f);
    }
}