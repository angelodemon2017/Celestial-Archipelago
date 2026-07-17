using UnityEngine;
using Zenject;
using System;

public class InputService : ITickable, IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;

    private IInputProviderContainer _inputProviderContainer;
    private IInputProvider _currentProvider;
    private bool _inputEnabled = true;

    public InputService(
        SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void Initialize()
    {
    }

    public void Dispose() { }

    public void Tick()
    {
        UpdateInput();
    }

    public void UpdateInput()
    {
        if (!_inputEnabled || _currentProvider == null) return;

        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        bool helpF1 = Input.GetKeyDown(KeyCode.F1);
        bool leftMouse = Input.GetMouseButtonDown(0);
        bool rightMouse = Input.GetMouseButtonDown(1);
        float scroll = Input.mouseScrollDelta.y;
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool tryInteract = Input.GetKeyDown(KeyCode.E);
        bool tab = Input.GetKeyDown(KeyCode.Tab);
        bool closing = Input.GetKeyDown(KeyCode.Q);

        _currentProvider.ProcessToggleKeyHints(helpF1);
        _currentProvider.ProcessLeftMouseButton(leftMouse);
        _currentProvider.ProcessRightMouseButton(rightMouse);
        _currentProvider.ProcessScrollMouse(scroll);
        _currentProvider.ProcessMovement(move);
        _currentProvider.ProcessLook(look);
        _currentProvider.ProcessJump(jumpPressed);
        _currentProvider.ProcessInteract(tryInteract);
        _currentProvider.ProcessTab(tab);
        _currentProvider.ProcessTryClose(closing);
    }

    public void SetInputProviderContainer(IInputProviderContainer inputProviderContainer)
    {
        if (_inputProviderContainer != null)
            _inputProviderContainer.InputProviderUpdated -= SetProviderFromContainer;

        _inputProviderContainer = inputProviderContainer;
        _inputProviderContainer.InputProviderUpdated += SetProviderFromContainer;
        SetProviderFromContainer();
    }

    private void SetProviderFromContainer()
    {
        SetInputProvider(_inputProviderContainer.InputProvider);
    }

    public void SetInputProvider(IInputProvider requester)
    {
        if (_currentProvider != null)
            _currentProvider.SetInputActive(false);

        _currentProvider = requester;

        if (_currentProvider != null)
            _currentProvider.SetInputActive(true);
    }

    public void EnableInput(bool enable)
    {
        _inputEnabled = enable;
        if (_currentProvider != null)
            _currentProvider.SetInputActive(enable);
    }

    public IInputProvider CurrentRequester => _currentProvider;
}