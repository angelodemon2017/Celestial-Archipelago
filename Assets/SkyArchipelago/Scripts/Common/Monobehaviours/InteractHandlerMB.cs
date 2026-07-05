using UnityEngine;

public class InteractHandlerMB : MonoBehaviour
{
    private EntityModel _model;
    protected bool _isFocused = false;

    public string InteractionPrompt => _model.InteractionPrompt;
    public float MaxInteractionDistance => _model.MaxInteractionDistance;
    public virtual bool CanInteract => _model.IsInteractable;
    public EntityModel GetModel => _model;

    public void InitHandler(EntityModel model)
    {
        _model = model;
    }

    public virtual void OnFocusEnter()
    {
        _isFocused = true;
        // Можно добавить подсветку, particle и т.д.
    }

    public virtual void OnFocusExit()
    {
        _isFocused = false;
    }

    public bool IsInRange(Vector3 playerPosition)
    {
        return Vector3.Distance(playerPosition, transform.position) <= _model.MaxInteractionDistance;
    }
}