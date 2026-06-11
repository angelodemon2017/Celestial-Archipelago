using UnityEngine;

[System.Serializable]
public abstract class ShapeDefinition
{
    public Vector3Int centerOffset = Vector3Int.zero;
    public int seedOffset = 0;
    public float weight = 1f;

    public abstract float Evaluate(Vector3 gridPos, Vector3Int gridSize, int globalSeed);
}