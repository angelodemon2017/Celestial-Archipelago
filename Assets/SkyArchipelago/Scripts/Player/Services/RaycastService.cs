using UnityEngine;
using Zenject;

public class RaycastService : IRaycastService, IInitializable
{
    private Camera _camera;
    private LayerMask _currentLayerMask = ~0;

    public LayerMask CurrentLayerMask => _currentLayerMask;
    public Camera CurrentCamera => _camera;

    public RaycastService()
    {

    }

    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    public void Initialize()
    {

    }

    public bool Raycast(out RaycastHit hit, float maxDistance = Mathf.Infinity)
    {
        return Raycast(out hit, _currentLayerMask, maxDistance);
    }

    public bool Raycast(out RaycastHit hit, LayerMask layerMask, float maxDistance = Mathf.Infinity)
    {
        if (_camera == null)
        {
            hit = default;
            return false;
        }

        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        return Physics.Raycast(ray, out hit, maxDistance, layerMask);
    }

    public Vector3 GetLookPoint(float maxDistance = 100f)
    {
        if (_camera == null) return _camera.transform.position + _camera.transform.forward * maxDistance;

        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, _currentLayerMask))
            return hit.point;

        return ray.origin + ray.direction * maxDistance;
    }

    public void SetCurrentLayerMask(LayerMask layerMask)
    {
        _currentLayerMask = layerMask;
    }
}