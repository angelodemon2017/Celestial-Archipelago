using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileTestJson : MonoBehaviour
{
    [Header("Save Settings")]
    [SerializeField] private string fileName = "island_save.json";

    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    [ContextMenu("Save")]
    public void Save()
    {
        IslandData island = CreateTestIslandData();

        // Специальный wrapper для корректной сериализации
        var saveWrapper = new IslandSaveWrapper
        {
            id = $"{island.Id}",
            type = (int)island.EntityType,
            entitiesJson = new List<string>()
        };

/*        foreach (var entity in island.entities)
        {
            string entityJson = JsonUtility.ToJson(entity, true);
            saveWrapper.entitiesJson.Add(entityJson);
        }/**/

        string fullJson = JsonUtility.ToJson(saveWrapper, true);
        File.WriteAllText(FilePath, fullJson);

        Debug.Log($"✅ Сохранено в: {FilePath}");
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (!File.Exists(FilePath))
        {
            Debug.LogError($"Файл не найден: {FilePath}");
            return;
        }

        string json = File.ReadAllText(FilePath);
        IslandSaveWrapper wrapper = JsonUtility.FromJson<IslandSaveWrapper>(json);

        Debug.Log($"✅ Загружено {wrapper.entitiesJson.Count} сущностей:");

        foreach (string entityJson in wrapper.entitiesJson)
        {
            // Сначала десериализуем как базовый, чтобы узнать тип
            EntityData baseEntity = JsonUtility.FromJson<EntityData>(entityJson);

            EntityData entity = DeserializeByType(baseEntity.EntityType, entityJson);

/*            switch (entity.type)
            {
                case "Resource":
                    var resource = (ResourceEntityData)entity;
                    Debug.Log($"   • [Resource] {resource.resourceType} x{resource.quantity} | pos: {resource.position}");
                    break;

                case "Building":
                    var building = (BuildingEntityData)entity;
                    Debug.Log($"   • [Building] {building.buildingType} (lvl {building.level}) | pos: {building.position}");
                    break;

                case "NPC":
                    var npc = (NPCEntityData)entity;
                    Debug.Log($"   • [NPC] {npc.npcName} ({npc.npcId}) | pos: {npc.position}");
                    break;

                default:
                    Debug.Log($"   • [Unknown] type: {entity.type} | id: {entity.Id}");
                    break;
            }/**/
        }
    }

    private IslandData CreateTestIslandData()
    {
        var island = new IslandData();

/*        island.entities.Add(new ResourceEntityData
        {
            resourceType = "Wood",
            quantity = 250,
            position = new Vector3(10, 0, 15)
        });

        island.entities.Add(new BuildingEntityData
        {
            buildingType = "House",
            level = 2,
            position = new Vector3(0, 0, 0),
            rotation = Quaternion.Euler(0, 45, 0)
        });

        island.entities.Add(new NPCEntityData
        {
            npcId = "merchant_01",
            npcName = "Торговец Карл",
            dialogueId = "dialogue_start",
            position = new Vector3(12, 0, -8)
        });/**/

        return island;
    }

    [System.Serializable]
    private class IslandSaveWrapper
    {
        public string id;
        public int type;
        public List<string> entitiesJson = new List<string>();
    }

    private EntityData DeserializeByType(EEntityType type, string json)
    {
        switch (type)
        {
            case EEntityType.ResourceEntity: return JsonUtility.FromJson<ResourceEntityData>(json);
            case EEntityType.BuildingEntity: return JsonUtility.FromJson<BuildingEntityData>(json);
            default: return JsonUtility.FromJson<EntityData>(json);
        }
    }
}