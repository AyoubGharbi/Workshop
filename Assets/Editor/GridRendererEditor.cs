using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridRenderer))]
public class GridRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridRenderer gridRenderer = (GridRenderer)target;
        if (GUILayout.Button("Refresh Grid"))
        {
            gridRenderer.EditorRefreshGrid();
        }
    }
}