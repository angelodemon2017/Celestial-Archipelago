using UnityEngine;

public class EntityModel<T> : EntityModel
    where T : EntityData
{
    protected T GetModel => (T)_dataModel;
    public override Vector3 Position => GetModel.position;
    public override Quaternion Rotation => GetModel.rotation;

    public EntityModel(T data)
    {
        _dataModel = data;
    }
}

public class EntityModel : BaseModel<BaseData>
{
    public CtxFlag AvailableFlag;
    public bool IsGrounded;
    public virtual float MoveSpeed => 0f;
    public virtual bool IsPhysical => false;
    public virtual Vector3 Position => Vector3.zero;
    public virtual Quaternion Rotation => Quaternion.identity;
    public override string ModelName => "Some Entity";
}