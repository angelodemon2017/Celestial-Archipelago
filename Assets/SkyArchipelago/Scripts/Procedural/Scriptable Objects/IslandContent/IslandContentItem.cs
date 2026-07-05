using UnityEngine;

public abstract class IslandContentItem : ScriptableObject
{
    public bool IsCuttingWeight = false;
    [Tooltip("Насколько сильно сглаживать края вырезания")]
    public float smoothK = 4f;

    [Header("Common Settings")]
    public Vector3 rotationOffset = Vector3.zero;
    public ModelConfig ModelConfig;

//    public abstract void Process(MarchingCubesGeneratorController controller, DiContainer diContainer);

    public virtual bool TryGetDensityInfluence(Vector3 pos, Vector3Int gridSize, Vector3Int positionOffset, int seed, out float densityValue)
    {
        densityValue = 0f;
        return false;
    }

    public virtual Vector3 GetSizeBound() => Vector3.one;
}