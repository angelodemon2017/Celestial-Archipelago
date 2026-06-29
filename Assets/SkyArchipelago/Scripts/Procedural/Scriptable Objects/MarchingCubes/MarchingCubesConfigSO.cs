using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Marching Cubes/Marching Cubes Config")]
public class MarchingCubesConfigSO : ScriptableObject
{
    public int IdConfig;

    [Header("Grid")]
    public Vector3Int gridSize = new Vector3Int(64, 32, 64);
    public float cellSize = 1f;

    [Header("Generation")]
    public int globalSeed = 42;
    public List<ShapeInstance> shapes = new List<ShapeInstance>();

    [Header("Content")]
    public List<IslandContentItem> contentItems;

    [Header("Marching Cubes")]
    public float surfaceLevel = 0f;

    public List<ColorLayer> layers;

    public Color GetVertexColor(Vector3 worldPos)
    {
        float height = worldPos.y;

        foreach (var layer in layers)
        {
            if(height > layer.Height)
                return layer.Color;
        }

        return Color.black;
    }

    [Serializable]
    public class ColorLayer
    {
        public Color Color;
        public float Height;
    }
}