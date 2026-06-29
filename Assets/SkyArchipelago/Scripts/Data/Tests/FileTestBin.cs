using System.IO;
using System.IO.Compression;
using UnityEngine;
using System.Collections.Generic;

public class FileTestBin : MonoBehaviour
{
    [Header("Save Settings")]
    [SerializeField] private string fileName = "island_save.dat";

    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    [ContextMenu("Save")]
    public void Save()
    {
        IslandData island = CreateTestIslandData();
        SaveBinary(island);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (!File.Exists(FilePath))
        {
            Debug.LogError($"Файл не найден: {FilePath}");
            return;
        }

        IslandData loadedIsland = LoadBinary();
        PrintLoadedData(loadedIsland);
    }

    private void SaveBinary(IslandData island)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                /*                writer.Write(island.Id);
                                writer.Write(island.entities.Count);

                                foreach (var entity in island.entities)
                                {
                                    WriteEntity(writer, entity);
                                }/**/
            }

            // Сжимаем
            byte[] compressed = Compress(memoryStream.ToArray());
            File.WriteAllBytes(FilePath, compressed);
        }

        Debug.Log($"✅ Сохранено (Binary + GZip): {FilePath} | Размер: {new FileInfo(FilePath).Length} байт");
    }

    private IslandData LoadBinary()
    {
        byte[] compressed = File.ReadAllBytes(FilePath);
        byte[] decompressed = Decompress(compressed);

        using (var memoryStream = new MemoryStream(decompressed))
        using (var reader = new BinaryReader(memoryStream))
        {
            var island = new IslandData
            {
                Id = reader.ReadInt32()
            };

            int count = reader.ReadInt32();
/*            island.entities = new List<EntityData>(count);

            for (int i = 0; i < count; i++)
            {
                EntityData entity = ReadEntity(reader);
                if (entity != null)
                    island.entities.Add(entity);
            }/**/

            return island;
        }
    }

    // ====================== Сериализация сущностей ======================
    private void WriteEntity(BinaryWriter writer, EntityData entity)
    {
        writer.Write(entity.type ?? "");
        writer.Write(entity.Id);
        WriteVector3(writer, entity.position);
        WriteQuaternion(writer, entity.rotation);

        switch (entity.type)
        {
            case "Resource":
                var res = (ResourceEntityData)entity;
                writer.Write(res.resourceType ?? "");
                writer.Write(res.quantity);
                break;

            case "Building":
                var build = (BuildingEntityData)entity;
                writer.Write(build.buildingType ?? "");
                writer.Write(build.level);
                break;

            case "NPC":
                var npc = (NPCEntityData)entity;
                writer.Write(npc.npcId ?? "");
                writer.Write(npc.npcName ?? "");
                writer.Write(npc.dialogueId ?? "");
                break;
        }
    }

    private EntityData ReadEntity(BinaryReader reader)
    {
        string type = reader.ReadString();
        int id = reader.ReadInt32();
        Vector3 pos = ReadVector3(reader);
        Quaternion rot = ReadQuaternion(reader);

        EntityData entity = type switch
        {
            "Resource" => new ResourceEntityData(),
            "Building" => new BuildingEntityData(),
            "NPC" => new NPCEntityData(),
            _ => new EntityData()
        };

        entity.Id = id;
        entity.type = type;
        entity.position = pos;
        entity.rotation = rot;

        switch (type)
        {
            case "Resource":
                var res = (ResourceEntityData)entity;
                res.resourceType = reader.ReadString();
                res.quantity = reader.ReadInt32();
                break;

            case "Building":
                var build = (BuildingEntityData)entity;
                build.buildingType = reader.ReadString();
                build.level = reader.ReadInt32();
                break;

            case "NPC":
                var npc = (NPCEntityData)entity;
                npc.npcId = reader.ReadString();
                npc.npcName = reader.ReadString();
                npc.dialogueId = reader.ReadString();
                break;
        }

        return entity;
    }

    // ====================== Вспомогательные методы ======================
    private void WriteVector3(BinaryWriter writer, Vector3 v)
    {
        writer.Write(v.x);
        writer.Write(v.y);
        writer.Write(v.z);
    }

    private Vector3 ReadVector3(BinaryReader reader)
    {
        return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    private void WriteQuaternion(BinaryWriter writer, Quaternion q)
    {
        writer.Write(q.x);
        writer.Write(q.y);
        writer.Write(q.z);
        writer.Write(q.w);
    }

    private Quaternion ReadQuaternion(BinaryReader reader)
    {
        return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    private byte[] Compress(byte[] data)
    {
        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, CompressionMode.Compress))
        {
            gzip.Write(data, 0, data.Length);
        }
        return ms.ToArray();
    }

    private byte[] Decompress(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var gzip = new GZipStream(ms, CompressionMode.Decompress);
        using var result = new MemoryStream();
        gzip.CopyTo(result);
        return result.ToArray();
    }

    private void PrintLoadedData(IslandData island)
    {
//        Debug.Log($"✅ Загружено {island.entities.Count} сущностей:");

/*        foreach (var entity in island.entities)
        {
            switch (entity.type)
            {
                case "Resource":
                    var r = (ResourceEntityData)entity;
                    Debug.Log($"   • [Resource] {r.resourceType} x{r.quantity} | pos: {r.position}");
                    break;
                case "Building":
                    var b = (BuildingEntityData)entity;
                    Debug.Log($"   • [Building] {b.buildingType} (lvl {b.level}) | pos: {b.position}");
                    break;
                case "NPC":
                    var n = (NPCEntityData)entity;
                    Debug.Log($"   • [NPC] {n.npcName} ({n.npcId}) | pos: {n.position}");
                    break;
            }
        }/**/
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

        island.entities.Add(new ResourceEntityData
        {
            resourceType = "Stone",
            quantity = 360,
            position = new Vector3(1, 0, 5)
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
}