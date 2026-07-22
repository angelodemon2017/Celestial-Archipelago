using System;
using System.IO;

[Serializable]
public abstract class BaseData<T>
    where T : BaseDataConfig
{
    public int IdOwner = -1;
    public EEntityType EntityType;//??
    public int Id;
    [NonSerialized]
    public T Config;

    public virtual void Copy<T2>(T2 data)
        where T2 : BaseData<T>
    {
        EntityType = data.EntityType;
        Id = data.Id;
        Config = data.Config;
    }

    public virtual void InitConfig(T config)
    {
        Config = config;
    }

    public virtual void LoadFromBinary(BinaryReader binaryReader)
    {
        Id = binaryReader.ReadInt32();
    }

    public virtual void SaveToBinary(BinaryWriter writer)
    {
        writer.Write((int)EntityType);
        writer.Write(Id);
    }
}