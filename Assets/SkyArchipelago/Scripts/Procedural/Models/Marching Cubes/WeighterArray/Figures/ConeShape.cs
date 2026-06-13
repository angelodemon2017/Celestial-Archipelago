using UnityEngine;

[System.Serializable]
public class ConeShape : ShapeDefinition
{
    public float height = 12f;
    public float radius = 8f;
    public float hatHeight = 2f;
    public float noiseStrength = 2f;
    public float noiseScale = 1.5f;
    public float perlinMultSeed = 1.2f;
    public float airWeight = -10f;

    public override Vector3 OffSetBound()
    {
        return new Vector3(0f, hatHeight / 2, 0f);
    }

    public override Vector3 GetSizeBound()
    {
        return new Vector3(radius * 2, height + hatHeight, radius * 2);
    }

    public override float Evaluate(Vector3 gridPos, Vector3Int gridSize, int globalSeed)
    {
        return GetTestCone(gridPos, gridSize, globalSeed);
    }

    private float GetTestCone(Vector3 wp, Vector3Int gridSize, int globalSeed)
    {
        Vector3Int center = gridSize / 2 + centerOffset;
        Vector3 localPos = wp - center;

        float coneHeight = height;
        float maxRadius = radius;

        localPos.y += coneHeight * 0.55f;

        float normalizedFromTop = Mathf.Clamp01(localPos.y / coneHeight);
        float radiusAtThisLevel = maxRadius * normalizedFromTop;

        float distToAxis = new Vector2(localPos.x, localPos.z).magnitude;

        float density = radiusAtThisLevel - distToAxis;

        if (localPos.y < 0f)
        {
            density -= Mathf.Abs(localPos.y) * 5f;
        }

        if (localPos.y > coneHeight + hatHeight)
        {
            density = airWeight;
        }

        float noise = Mathf.PerlinNoise(wp.x * 1.5f + globalSeed, wp.z * 1.5f + globalSeed * 1.2f);
        density += (noise - 0.5f) * noiseStrength;

        return density;
    }
}