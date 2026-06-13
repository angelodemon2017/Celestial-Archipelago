using System;
using UnityEngine;
using Zenject;

public class CursorService : IInitializable, IDisposable
{
    private bool _isLocked = true;

    [Inject]
    public CursorService() { }

    public void Initialize()
    {

    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _isLocked = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isLocked = false;
    }

    public void ToggleCursor()
    {
        if (_isLocked)
            UnlockCursor();
        else
            LockCursor();
    }

    public void Dispose() => UnlockCursor();
}