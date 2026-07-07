using System;
using UnityEngine;
using Zenject;

public class PlayerInteractionService : ITickable, ISourceHint
{
    private readonly SignalBus _signalBus;
    private readonly IRaycastService _raycastService;
    private readonly FPSCommonModel _fPSCommonModel;
    private readonly UIMBFactory<HitSourceInitModel, HitSource> _hitSourceFactory;

    private InteractHandlerMB _currentHandlerFocus;

    public Vector3 LastHit;
    public string GetHint => _currentHandlerFocus?.InteractionPrompt ?? string.Empty;
    public Action HintUpdated { get; set; }

    [Inject]
    public PlayerInteractionService(
        SignalBus signalBus,
        FPSCommonModel fPSCommonModel,
        IRaycastService raycastService,
        UIMBFactory<HitSourceInitModel, HitSource> hitSourceFactory)
    {
        _signalBus = signalBus;
        _fPSCommonModel = fPSCommonModel;
        _raycastService = raycastService;
        _hitSourceFactory = hitSourceFactory;
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

    public void TryMainAction()
    {
        if (_currentHandlerFocus != null)
        {
            _signalBus.Fire(
                new InteractContext(
                    _fPSCommonModel.LocalPlayerModel,
                    _fPSCommonModel.LocalPlayerModel.CurrentItem,
                    EModeInteract.LCM,
                    _currentHandlerFocus.GetModel));
        }
        else if(_fPSCommonModel.LocalPlayerModel.CurrentItem != null &&
            _fPSCommonModel.LocalPlayerModel.CurrentItem.IsUsable)
        {//??
            //TODO use of currentItem
        }
        else
        {
            var hitSource = _hitSourceFactory.Create(new HitSourceInitModel(
                _fPSCommonModel.LocalPlayerModel,
                _fPSCommonModel.LocalPlayerModel.CurrentItem));
            hitSource.transform.position = LastHit;
        }
    }

    public void TryAltAction()
    {
        if (_currentHandlerFocus != null)
        {
            _signalBus.Fire(
                new InteractContext(
                    _fPSCommonModel.LocalPlayerModel,
                    _fPSCommonModel.LocalPlayerModel.CurrentItem,
                    EModeInteract.RCM,
                    _currentHandlerFocus.GetModel));
        }
        else
        {
            //TODO release alt action/alt use of currentItem
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
}