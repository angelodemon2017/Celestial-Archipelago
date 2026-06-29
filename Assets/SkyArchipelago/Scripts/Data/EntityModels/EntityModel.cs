public class EntityModel<T> : EntityModel
    where T : BaseData
{
    protected T GetModel => (T)_dataModel;
}

public class EntityModel : BaseModel<BaseData>
{
    public CtxFlag AvailableFlag;
    public override string ModelName => "Some Entity";
}