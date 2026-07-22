using UnityEngine;

public class EntityModel<T> : EntityModel
    where T : EntityData
{
    protected T GetData => (T)_dataModel;
    public override Vector3 Position => GetData.position;
    public override Quaternion Rotation => GetData.rotation;

    public EntityModel(T data)
    {
        OnSpawned(data);
    }
}

[System.Serializable]
public class EntityModel : BaseModel<EntityData, ModelConfig>, IEntity
{
    public int ConfigId => _dataModel.ConfigId;
    public CtxFlag AvailableTags => _dataModel.AvailableFlags;

    public bool HaveChange { get; set; }
    public bool IsGrounded;
    public virtual float MoveSpeed => 0f;
    public virtual bool IsPhysical => false;
    public bool IsAvailableUpdate => true;//
    public virtual bool IsInteractable => false;
    public virtual string InteractionPrompt => "Interact";
    public virtual float MaxInteractionDistance => 1f;

    public virtual Vector3 Position
    {
        get => _dataModel.position;
        set => _dataModel.position = value;
    }
    public virtual Quaternion Rotation
    {
        get => _dataModel.rotation;
        set => _dataModel.rotation = value;
    }
    public override string ModelName => _dataModel.Config.ContentName;

    public virtual void OnSpawned(EntityData entityData)
    {
        _dataModel = entityData;
    }

    public virtual void OnDespawned()
    {
        EntityDataMap.ReturnData(_dataModel);
        _dataModel = null;
    }
}