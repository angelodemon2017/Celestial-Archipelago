using UnityEngine;

[System.Serializable]
public class ShapeInstance
{
    public BaseShapeConfigSO shape;
    public Vector3Int shapeOffset;
    public ShapeOperation operation = ShapeOperation.SmoothUnion;
    public float smoothK = 3f;

    public Vector3 GizmoCentr()
    {
        return -shapeOffset + shape.GetShape.OffSetBound();
    }
}