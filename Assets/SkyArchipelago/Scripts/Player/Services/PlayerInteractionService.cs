using System;
using UnityEngine;
using Zenject;

public class PlayerInteractionService : ITickable, IInitializable, IDisposable, ISourceHint
{
    private readonly IRaycastService _raycastService;

    private IInteractable _currentFocus;

    public IInteractable CurrentFocus => _currentFocus;
    public string GetHint => _currentFocus == null ? string.Empty : _currentFocus.InteractionPrompt;
    public Action HintUpdated { get; set; }

    [Inject]
    public PlayerInteractionService(
        IRaycastService raycastService)
    {
        _raycastService = raycastService;
    }

    public void Initialize()
    {
    }

    public void Tick()
    {
        UpdateInteraction();
    }

    public void UpdateInteraction()
    {
        bool hitSomething = _raycastService.Raycast(out RaycastHit hit, Mathf.Infinity);

        IInteractable newFocus = null;

        if (hitSomething)
        {
            newFocus = hit.collider.GetComponentInParent<IInteractable>();

            if (newFocus != null &&
                !newFocus.IsInRange(_raycastService.CurrentCamera.transform.position)) // или позиции игрока
            {
                newFocus = null;
            }
        }

        if (newFocus != _currentFocus)
        {
            _currentFocus?.OnFocusExit();
            newFocus?.OnFocusEnter();
            _currentFocus = newFocus;
            HintUpdated?.Invoke();
        }
    }

    public void TryInteract()
    {
        if (_currentFocus != null)
        {
            _currentFocus.Interact();
        }
    }

    public void Dispose()
    {
    }
}