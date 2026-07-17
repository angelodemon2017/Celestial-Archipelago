using UnityEngine;

public class EntityModel<T> : EntityModel
    where T : EntityData
{
    private CtxFlag testFlag;
    protected T GetData => (T)_dataModel;
    public override Vector3 Position => GetData.position;
    public override Quaternion Rotation => GetData.rotation;

    public EntityModel(T data)
    {
        _dataModel = data;
        testFlag = CtxFlag.None;
        ConfigModel?.ModuleConfigs.ForEach(m => testFlag |= m.KeyFlag);
    }
}

public class EntityModel : BaseModel<EntityData, ModelConfig>, IEntity
{
    public int Uid => ConfigModel.Uid;
    public CtxFlag AvailableTags => _dataModel.AvailableFlags;

    public bool HaveChange { get; set; }
    public bool IsGrounded;
    public virtual float MoveSpeed => 0f;
    public virtual bool IsPhysical => false;
    public bool IsAvailableUpdate => true;//
    public virtual bool IsInteractable => false;
    public virtual string InteractionPrompt => "Interact";
    public virtual float MaxInteractionDistance => 1f;

    public virtual Vector3 Position => Vector3.zero;
    public virtual Quaternion Rotation => Quaternion.identity;
    public override string ModelName => "Model name";
}