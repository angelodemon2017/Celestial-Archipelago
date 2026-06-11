using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Marching Cubes/Floating Island Config")]
public class FloatingIslandConfig : ScriptableObject
{
    [Header("Grid")]
    public Vector3Int gridSize = new Vector3Int(96, 52, 96);
    public float cellSize = 1f;
    public float bottomThickness = 4;

    [Header("Multiple Shapes")]
    public float smoothRadius = 3.0f;
    public List<Vector3Int> coneCenters = new List<Vector3Int>()
    {
        new Vector3Int(0, 0, 0),
    };
    public int numCones = 3;
    public Vector3Int minOffset = new Vector3Int(-12, -2, -12);
    public Vector3Int maxOffset = new Vector3Int(12, 4, 12);
    public float minDistanceBetweenCones = 6f;

    [Header("Base Shape")]
    public float baseRadius = 32f;
    public float maxHeight = 18f;
    public float centerYOffset = -4f;

    [Header("Noise")]
    public int seed = 42;
    public float perimeterNoiseScale = 0.085f;
    public float mainNoiseScale = 0.06f;
    public float detailNoiseScale = 0.25f;
    public float caveNoiseScale = 0.15f;

    [Header("Strength")]
    public float noiseStrength = 7f;
    public float stalactiteStrength = 12f;
    public float caveStrength = 6f;
    public float edgeFalloff = 0.73f;

    [Header("Visual")]
    public Material material;
}