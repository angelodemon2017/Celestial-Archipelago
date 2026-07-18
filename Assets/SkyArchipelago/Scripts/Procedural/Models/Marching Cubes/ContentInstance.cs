using UnityEngine;

[System.Serializable]
public class ContentInstance
{
    public IslandContentCutter CutterConfig;
    [Tooltip("Насколько сильно сглаживать края вырезания")]
    public float smoothK = 4f;

    [Header("Common Settings")]
    public Vector3 rotationOffset = Vector3.zero;
    public Vector3Int positionOffset;
    public ModelConfig ModelConfig;

    public Vector3 GetSizeBound() => CutterConfig?.GetSizeBound() ?? Vector3.one;
}