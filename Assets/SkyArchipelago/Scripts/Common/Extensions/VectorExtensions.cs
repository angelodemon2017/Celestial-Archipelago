using UnityEngine;

public static class VectorExtensions
{
    public static bool TryGetDownPoint(this Vector3 startPoint, LayerMask layerMask, out Vector3 hitPoint, out RaycastHit hitInfo, float maxDistance = 1000f)
    {
        Ray ray = new Ray(startPoint, Vector3.down);
        if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
        {
            hitPoint = hitInfo.point;
            return true;
        }

        hitPoint = startPoint + Vector3.down * maxDistance;
        return false;
    }
}