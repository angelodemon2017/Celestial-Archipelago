using UnityEngine;

public class BaseShapeConfigSO : ScriptableObject
{
    public virtual ShapeDefinition GetShape { get; }
}