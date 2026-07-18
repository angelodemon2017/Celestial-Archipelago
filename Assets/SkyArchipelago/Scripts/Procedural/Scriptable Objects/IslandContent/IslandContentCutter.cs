using UnityEngine;

public abstract class IslandContentCutter : ScriptableObject
{
    public virtual bool TryGetDensityInfluence(Vector3 pos, Vector3Int gridSize, Vector3Int positionOffset, int seed, out float densityValue)
    {
        densityValue = 0f;
        return false;
    }

    public virtual Vector3 GetSizeBound() => Vector3.one;
}