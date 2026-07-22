using System.Collections.Generic;
using UnityEngine;

public class InteractHandlerMB : MonoBehaviour
{
    public List<GameObject> GOsOfColliders;

    public EntityModel _model;//private
    protected bool _isFocused = false;
    private float _powOfInteractDist;

    public string InteractionPrompt => _model.InteractionPrompt;
    public virtual bool CanInteract => _model.IsInteractable;
    public EntityModel GetModel => _model;

    public void SetModel(EntityModel model)
    {
        _model = model;
        if(_model != null)
            _powOfInteractDist = _model.MaxInteractionDistance * _model.MaxInteractionDistance;        
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
        var sqr = (playerPosition - transform.position).sqrMagnitude;
        return sqr <= _powOfInteractDist;
    }
}