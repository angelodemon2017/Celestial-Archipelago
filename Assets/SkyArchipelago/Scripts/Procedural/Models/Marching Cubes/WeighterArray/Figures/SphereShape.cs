using UnityEngine;

[System.Serializable]
public class SphereShape : ShapeDefinition
{
    public float radius = 8f;

    public override float Evaluate(Vector3 gridPos, Vector3Int gridSize, int globalSeed)
    {
        Vector3Int center = gridSize / 2 + centerOffset;
        float dist = Vector3.Distance(gridPos, center);
        return (radius - dist) * weight;
    }
}