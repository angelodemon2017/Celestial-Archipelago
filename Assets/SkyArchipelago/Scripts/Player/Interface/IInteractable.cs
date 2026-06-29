using UnityEngine;

public interface IInteractable
{
    string InteractionPrompt { get; }
    float MaxInteractionDistance { get; }
    bool CanInteract { get; }

    void OnFocusEnter();                        // Когда игрок навёл взгляд
    void OnFocusExit();                         // Когда взгляд ушёл
    bool TryInteract(out InteractionResult result);
    bool TryInteract(out EntityModel model);
    bool IsInRange(Vector3 playerPosition);
}