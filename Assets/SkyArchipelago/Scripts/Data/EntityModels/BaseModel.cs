using System;

public abstract class BaseModel<T, T2>
    where T : BaseData<T2>
    where T2 : BaseDataConfig
{
    public T _dataModel;
    public T2 ConfigModel => _dataModel?.Config;

    public EEntityType EntType => _dataModel.EntityType;
    public int Id => _dataModel?.Id ?? -1;
    public abstract string ModelName { get; }
    public virtual string DebugName => $"{Id}.{ModelName}";

    public Action Changed;
}