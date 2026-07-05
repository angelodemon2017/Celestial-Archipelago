using System;
using System.IO;

[Serializable]
public abstract class BaseData<T>
    where T : BaseDataConfig
{
    public EEntityType EntityType;
    public int Id;
    public T Config;

    [NonSerialized]
    public Action DataUpdated;

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