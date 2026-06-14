using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] protected string _interactionPrompt = "Взаимодействовать";
    [SerializeField] protected float _maxInteractionDistance = 3f;

    protected bool _isFocused = false;

    public string InteractionPrompt => _interactionPrompt;
    public float MaxInteractionDistance => _maxInteractionDistance;
    public virtual bool CanInteract => true;

    public virtual void OnFocusEnter()
    {
        _isFocused = true;
        // Можно добавить подсветку, particle и т.д.
    }

    public virtual void OnFocusExit()
    {
        _isFocused = false;
    }

    public abstract bool TryInteract(out InteractionResult result);

    public bool IsInRange(Vector3 playerPosition)
    {
        return Vector3.Distance(playerPosition, transform.position) <= _maxInteractionDistance;
    }
}