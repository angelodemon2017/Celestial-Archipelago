public abstract class BaseModel<T>
    where T : BaseData
{
    internal T _dataModel;

    public virtual int Id => _dataModel?.Id ?? -1;
    public abstract string ModelName { get; }
    public virtual string DebugName => $"{Id}.{ModelName}";
}