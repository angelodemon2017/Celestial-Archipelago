using UnityEngine;
using Zenject;

public abstract class IslandContentItem : ScriptableObject
{
    public EEntityType eEntityType;
    public string ContentName = "New Content";
    public bool IsCuttingWeight = false;
    [Tooltip("Насколько сильно сглаживать края вырезания")]
    public float smoothK = 4f;

    [Header("Common Settings")]
    public Vector3Int positionOffset = Vector3Int.zero;
    public Vector3 rotationOffset = Vector3.zero;
    public GameObject ViewModelPrefab;

    public abstract void Process(MarchingCubesGeneratorController controller, DiContainer diContainer);

    public virtual bool TryGetDensityInfluence(Vector3 pos, Vector3Int gridSize, int seed, out float densityValue)
    {
        densityValue = 0f;
        return false;
    }

    public virtual Vector3 GetOffsetBound() => positionOffset;
    public virtual Vector3 GetSizeBound() => Vector3.one;
}