using UnityEngine;
using Zenject;

public abstract class EntityView : MonoBehaviour
{
    protected EntityModelProto Model { get; private set; }

    [Inject]
    protected virtual void Construct(EntityModelProto model)
    {
        Model = model;
        SubscribeToModel();
    }

    protected virtual void SubscribeToModel()
    {
        if (Model == null) return;

        Model.OnPositionChanged += UpdatePosition;
        Model.OnDestroyed += HandleDestroy;
    }

    protected virtual void OnDestroy()
    {
        if (Model != null)
        {
            Model.OnPositionChanged -= UpdatePosition;
            Model.OnDestroyed -= HandleDestroy;
        }
    }

    protected virtual void UpdatePosition()
    {
        transform.position = Model.Position;
        transform.rotation = Model.Rotation;
    }

    protected virtual void HandleDestroy(EntityModelProto entity)
    {
        Destroy(gameObject);
    }

    // Вызывается из Input / Raycast и т.д.
    public virtual void OnPlayerInteract()
    {
//        var result = Model.TryInteract(null); // или передать PlayerModel
        // Обработка результата через InteractionHandlerService
    }
}