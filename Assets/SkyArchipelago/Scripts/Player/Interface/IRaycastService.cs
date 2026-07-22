using UnityEngine;

public interface IRaycastService
{
    LayerMask CurrentLayerMask { get; }
    Camera CurrentCamera { get; }

    void SetCamera(Camera camera);
//    void SetCC(CameraController cameraController);
    /// <summary>
    /// Основной рейкаст из камеры текущим слоем
    /// </summary>
    bool Raycast(out RaycastHit hit, float maxDistance = Mathf.Infinity);

    /// <summary>
    /// Рейкаст с конкретной LayerMask
    /// </summary>
    bool Raycast(out RaycastHit hit, LayerMask layerMask, float maxDistance = Mathf.Infinity);

    bool Raycast(Vector3 startPoint, out RaycastHit hit, float maxDistance = Mathf.Infinity);

    /// <summary>
    /// Получить точку взгляда (на максимальной дистанции, если ничего не попало)
    /// </summary>
    Vector3 GetLookPoint(float maxDistance = 100f);

    /// <summary>
    /// Сменить текущий режим рейкаста (LayerMask)
    /// </summary>
    void SetCurrentLayerMask(LayerMask layerMask);
}