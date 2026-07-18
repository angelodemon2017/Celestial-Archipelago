using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Island Content/Entity Spawner")]
public class SphereCutter : IslandContentCutter
{
    [Header("Cutting Sphere")]
    [Tooltip("Радиус сферы вырезания")]
    public float cutRadius = 6f;
    public float smoothSphere;

    public override Vector3 GetSizeBound()
    {
        return Vector3.one * (cutRadius * 1f);
    }

    public override bool TryGetDensityInfluence(Vector3 pos, Vector3Int gridSize, Vector3Int positionOffset, int seed, out float densityValue)
    {
        Vector3Int center = gridSize / 2 + positionOffset;
        Vector3 localPos = pos - center;
        float distance = localPos.magnitude;

        if (distance > cutRadius + smoothSphere * 2f)
        {
            densityValue = 0f;
            return false;
        }

        float sphereSDF = distance - cutRadius;
        densityValue = -sphereSDF;

        return true;
    }
}