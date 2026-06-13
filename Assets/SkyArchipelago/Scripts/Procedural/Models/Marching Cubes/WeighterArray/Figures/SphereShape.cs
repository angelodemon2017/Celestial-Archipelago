using UnityEngine;

[System.Serializable]
public class SphereShape : ShapeDefinition
{
    public float radius = 8f;

    public override Vector3 GetSizeBound()
    {
        return new Vector3 (radius * 2, radius * 2, radius * 2);
    }

    public override float Evaluate(Vector3 gridPos, Vector3Int gridSize, int globalSeed)
    {
        Vector3Int center = gridSize / 2 + centerOffset;
        float dist = Vector3.Distance(gridPos, center);
        return (radius - dist) * weight;
    }
}