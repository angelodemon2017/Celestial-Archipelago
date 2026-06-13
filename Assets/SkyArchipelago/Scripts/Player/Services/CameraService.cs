using System;
using UnityEngine;

public class CameraService
{
    private readonly RaycastService _raycastService;

    private Camera _camera = null;

    private Transform _currentOwner = null;
    private Vector3 _offset;

    private float _xRotation = 0f;
    private const float MinVerticalAngle = -90f;
    private const float MaxVerticalAngle = 90f;

    public Action CameraUpdated;

    public CameraService(
        RaycastService raycastService)
    {
        _raycastService = raycastService;
    }

    public void SetCamera(Camera nextCamera)
    {
        _camera = nextCamera;
        _raycastService.SetCamera(nextCamera);
        AttachCameraToOwner();
        CameraUpdated?.Invoke();
    }

    public void AttachTo(Transform owner, Vector3 localPosition = default)
    {
        if (_currentOwner == owner) return;

        Detach();

        _currentOwner = owner;
        _offset = localPosition;

        AttachCameraToOwner();
    }

    private void AttachCameraToOwner()
    {
        if (!_camera || !_currentOwner)
            return;

        _camera.transform.SetParent(_currentOwner);
        _camera.transform.localPosition = _offset;
        _camera.transform.localRotation = Quaternion.identity;
    }

    public void Detach()
    {
        if (_currentOwner == null) return;
        _camera.transform.SetParent(null);
        _currentOwner = null;
    }

    public void ProcessLook(Vector2 lookInput, float sensitivity)
    {
        if (!_camera)
            return;

        ProcessLookVertical(lookInput.y * sensitivity * Time.deltaTime);
    }

    public void ProcessLookVertical(float mouseYDelta)
    {
        if (_camera == null) return;

        _xRotation -= mouseYDelta;
        _xRotation = Mathf.Clamp(_xRotation, MinVerticalAngle, MaxVerticalAngle);

        _camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }
}