using UnityEngine;

public class EntityModel<T> : EntityModel
    where T : EntityData
{
    protected T GetData => (T)_dataModel;
    public override Vector3 Position => GetData.position;
    public override Quaternion Rotation => GetData.rotation;

    public EntityModel(T data)
    {
        _dataModel = data;
    }
}

public class EntityModel : BaseModel<EntityData, ModelConfig>
{
    public CtxFlag AvailableFlag;
    public CtxFlag MyTag => _dataModel.Config.tag;

    public bool IsGrounded;
    public virtual float MoveSpeed => 0f;
    public virtual bool IsPhysical => false;

    public virtual bool IsInteractable => false;
    public virtual string InteractionPrompt => "Interact";
    public virtual float MaxInteractionDistance => 1f;

    public virtual Vector3 Position => Vector3.zero;
    public virtual Quaternion Rotation => Quaternion.identity;
    public override string ModelName => "Some Entity";
}