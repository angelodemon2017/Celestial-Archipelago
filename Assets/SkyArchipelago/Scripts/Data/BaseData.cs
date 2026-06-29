using System;

[Serializable]
public abstract class BaseData
{
    public int Id;

    [NonSerialized]
    public Action DataUpdated;

    public string type;
}