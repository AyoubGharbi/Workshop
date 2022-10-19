using System;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/" + nameof(GridDataSO), fileName = nameof(GridDataSO))]
public class GridDataSO : ScriptableObject
{
    [field: Range(1, 5000)]
    [field: SerializeField]
    public int GridRandomizationSeed { get; private set; }

    [field: Range(100, 5000)]
    [field: SerializeField]
    public int GridAnimationDelay { get; private set; }

    [field: Range(1, 10)]
    [field: SerializeField]
    public int GridWidth { get; private set; }

    [field: Range(1, 10)]
    [field: SerializeField]
    public int GridHeight { get; private set; }

    [field: SerializeField] public GameObject CellBlockPrefab { get; private set; }

    public CancellationTokenSource CancellationTokenSource { get; private set; } = new CancellationTokenSource();

    public float CellSize => CellBlockPrefab.transform.localScale.z;
    public float HalfCellSize => CellSize * 0.5f;


}