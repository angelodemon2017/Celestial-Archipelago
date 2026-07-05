using System;

public abstract class BaseModel<T, T2>
    where T : BaseData<T2>
    where T2 : BaseDataConfig
{
    internal T _dataModel;
    internal T2 _configModel => _dataModel?.Config;

    public EEntityType EntType => _dataModel.EntityType;
    public virtual int Id => _dataModel?.Id ?? -1;
    public abstract string ModelName { get; }
    public virtual string DebugName => $"{Id}.{ModelName}";

    public Action Changed;
}