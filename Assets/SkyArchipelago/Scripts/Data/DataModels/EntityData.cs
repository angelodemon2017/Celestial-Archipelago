using System.IO;
using UnityEngine;

[System.Serializable]
public class EntityData : BaseData
{
    public Vector3 position;
    public Quaternion rotation = Quaternion.identity;

    public virtual string DebugLog => $"EntityData.{EntityType}.{Id}";
    public virtual EntityModel CreateModel()
    {
        return new EntityModel();
    }

    public virtual void LoadFromBinary(BinaryReader binaryReader)
    {
        Id = binaryReader.ReadInt32();
        position = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        rotation = new Quaternion(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
    }

    public virtual void SaveToBinary(BinaryWriter writer)
    {
        writer.Write((int)EntityType);
        writer.Write(Id);
        writer.Write(position.x);
        writer.Write(position.y);
        writer.Write(position.z);
        writer.Write(rotation.x);
        writer.Write(rotation.y);
        writer.Write(rotation.z);
        writer.Write(rotation.w);
    }
}

[System.Serializable]
public class ResourceEntityData : EntityData
{
    public string resourceType = "Wood";
    public int quantity = 1;

    public override string DebugLog => base.DebugLog + $".{resourceType}.{quantity}";

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        resourceType = binaryReader.ReadString();
        quantity = binaryReader.ReadInt32();
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(resourceType ?? "");
        writer.Write(quantity);
    }
    public ResourceEntityData() => EntityType = EEntityType.ResourceEntity;
}

[System.Serializable]
public class BuildingEntityData : EntityData
{
    public string buildingType = "House";
    public int level = 1;

    public override string DebugLog => base.DebugLog + $".{buildingType}.{level}";

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        buildingType = binaryReader.ReadString();
        level = binaryReader.ReadInt32();
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(buildingType ?? "");
        writer.Write(level);
    }
    public BuildingEntityData() => EntityType = EEntityType.BuildingEntity;
}

[System.Serializable]
public class NPCEntityData : EntityData
{
    public string npcId = "";
    public string npcName = "NPC";
    public string dialogueId = "";

    public override string DebugLog => base.DebugLog + $".{npcId}.{npcName}.{dialogueId}";

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        npcId = binaryReader.ReadString();
        npcName = binaryReader.ReadString();
        dialogueId = binaryReader.ReadString();
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(npcId ?? "");
        writer.Write(npcName ?? "");
        writer.Write(dialogueId ?? "");
    }
    public NPCEntityData() => EntityType = EEntityType.NPCEntity;
}