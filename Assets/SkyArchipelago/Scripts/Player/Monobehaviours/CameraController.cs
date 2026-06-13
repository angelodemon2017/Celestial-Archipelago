using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;

/*    private SignalBus _signalBus;
    private RaycastService _raycastService;

    public Camera GetCamera => _camera;

    [Inject]
    private void Init(
        SignalBus signalBus,
        RaycastService raycastService)
    {
        _signalBus = signalBus;
        _raycastService = raycastService;

        Subs();
    }/**/

    private void Subs()
    {
//        _raycastService.SetCC(this);
    }

    private void OnDestroy()
    {
    }
}