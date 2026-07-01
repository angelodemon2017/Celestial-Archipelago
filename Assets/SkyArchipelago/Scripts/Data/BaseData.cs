using System;

[Serializable]
public abstract class BaseData
{
    public EEntityType EntityType;
    public int Id;

    [NonSerialized]
    public Action DataUpdated;
}