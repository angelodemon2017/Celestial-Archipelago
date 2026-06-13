using System;
using UnityEngine;

public abstract class BaseGameplayState : ISourceHint, IInputProviderContainer, IInputProvider
{
    public virtual IInputProvider InputProvider => this;
    public virtual string GetHint => string.Empty;
    public virtual bool CursorIsAvailable => true;

    public Action HintUpdated { get; set; }
    public Action InputProviderUpdated { get; set; }

    public virtual void StateOn()
    {
        Debug.Log($"Start state:{GetType().Name}");
    }

    public virtual void StateOff()
    {

    }

    protected virtual void HintUpdate()
    {
        HintUpdated?.Invoke();
    }

    public virtual void SetInputActive(bool active)
    {
    }

    public virtual void ProcessMovement(Vector2 moveInput)
    {
    }

    public virtual void ProcessLook(Vector2 lookInput)
    {
    }

    public virtual void ProcessJump(bool jumpPressed)
    {
    }

    public virtual void ProcessInteract(bool interacted)
    {
    }

    public virtual void ProcessTab(bool interact)
    {
    }
}