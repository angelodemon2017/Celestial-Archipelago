using UnityEngine;

[System.Serializable]
public class EntityData : BaseData
{
    public Vector3 position;
    public Quaternion rotation = Quaternion.identity;

}

[System.Serializable]
public class ResourceEntityData : EntityData
{
    public string resourceType = "Wood";
    public int quantity = 1;

    public ResourceEntityData() => type = "Resource";
}

[System.Serializable]
public class BuildingEntityData : EntityData
{
    public string buildingType = "House";
    public int level = 1;

    public BuildingEntityData() => type = "Building";
}

[System.Serializable]
public class NPCEntityData : EntityData
{
    public string npcId = "";
    public string npcName = "NPC";
    public string dialogueId = "";

    public NPCEntityData() => type = "NPC";
}