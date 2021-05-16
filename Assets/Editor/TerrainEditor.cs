using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainEditor : Editor
{
   
    public override void OnInspectorGUI()
    {
        TerrainGenerator m = (TerrainGenerator)target;

        DrawDefaultInspector();

        GUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate Terrain"))
            {
                m.GenerateTerrain();
            }

            if (GUILayout.Button("Erode Terrain"))
            {
                m.ErodeTerrain();
            }

        GUILayout.EndHorizontal();
    }
}
