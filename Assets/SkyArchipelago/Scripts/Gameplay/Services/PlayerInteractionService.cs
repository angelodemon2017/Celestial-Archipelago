using System;
using UnityEngine;
using Zenject;

public class PlayerInteractionService : ITickable, IInitializable, IDisposable, ISourceHint
{
    private readonly SignalBus _signalBus;
    private readonly IRaycastService _raycastService;
    private readonly FPSCommonModel _fPSCommonModel;

    private InteractHandlerMB _currentHandlerFocus;

    public Vector3 LastHit;
    public string GetHint => _currentHandlerFocus?.InteractionPrompt ?? string.Empty;
    public Action HintUpdated { get; set; }

    [Inject]
    public PlayerInteractionService(
        SignalBus signalBus,
        FPSCommonModel fPSCommonModel,
        IRaycastService raycastService)
    {
        _signalBus = signalBus;
        _fPSCommonModel = fPSCommonModel;
        _raycastService = raycastService;
    }

    public void Initialize()
    {
    }

    public void Tick()
    {
        UpdateCheckInteraction();
        if(_raycastService != null && _raycastService.CurrentCamera)
            Debug.DrawLine(_raycastService.CurrentCamera.transform.position, LastHit);
    }

    public void UpdateCheckInteraction()
    {
        bool hitSomething = _raycastService.Raycast(out RaycastHit hit, Mathf.Infinity);

        LastHit = hit.point;

        InteractHandlerMB newFocus = null;

        if (hitSomething)
        {
            newFocus = hit.collider.GetComponentInParent<InteractHandlerMB>();

            if (newFocus != null &&
                !newFocus.IsInRange(_raycastService.CurrentCamera.transform.position)) // или позиции игрока
            {
                newFocus = null;
            }
        }

        if (newFocus != _currentHandlerFocus)
        {
            _currentHandlerFocus?.OnFocusExit();
            newFocus?.OnFocusEnter();
            _currentHandlerFocus = newFocus;
            HintUpdated?.Invoke();
        }
    }

    public void TryInteractWithHandler()
    {
        if (_currentHandlerFocus != null &&
            _currentHandlerFocus.CanInteract)
        {
            _signalBus.Fire(
                new InteractContext(
                    _fPSCommonModel.LocalPlayerModel,
                    _fPSCommonModel.LocalPlayerModel.CurrentItem,
                    EModeInteract.EKB,
                    _currentHandlerFocus.GetModel));
        }
    }

    public void Dispose()
    {
    }
}