using UnityEngine;

public interface IInputProvider
{
    bool CursorIsAvailable { get; }
    void SetInputActive(bool active);           // Включаем/выключаем обработку

    void ProcessLeftMouseButton(bool lmb);
    void ProcessRightMouseButton(bool rmb);
    void ProcessScrollMouse(float scroll);
    void ProcessMovement(Vector2 moveInput);
    void ProcessLook(Vector2 lookInput);
    void ProcessJump(bool jumpPressed);
    void ProcessInteract(bool interacted);
    void ProcessTab(bool interact);
    void ProcessTryClose(bool isClosing);
    // Добавляй по мере необходимости:
    // void ProcessAttack(bool attackPressed);
    // void ProcessAbility(int index);
}