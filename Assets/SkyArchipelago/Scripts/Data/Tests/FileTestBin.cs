using System.IO;
using System.IO.Compression;
using UnityEngine;
using Zenject;

public class FileTestBin : MonoBehaviour
{
    public WorldData DebugWorld;

    [Header("Save Settings")]
    [SerializeField] private string fileName = "island_save.dat";

    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    [Inject]
    public void Construct(DataService dataService)
    {
        DebugWorld = dataService.worldData;
    }

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
    }

    private void SaveBinary(IslandData island)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write(island.Id);
                writer.Write(island.entities.Datas.Count);

                foreach (var entity in island.entities.Datas)
                {
                    entity.SaveToBinary(writer);
                }
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

            for (int i = 0; i < count; i++)
            {
                EEntityType type = (EEntityType)reader.ReadInt32();
                EntityData entity = EntityDataMap.CreateData(type);
                entity.LoadFromBinary(reader);
                Debug.Log($"Loaded entity:{entity.DebugLog}");
                if (entity != null)
                    island.entities.AddNewData(entity);
            }

            return island;
        }
    }

    // ====================== Вспомогательные методы ======================

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

    private IslandData CreateTestIslandData()
    {
        var island = new IslandData();

        island.entities.AddNewData(new DemoNPCData
        {
            NpcId = "merchant_01",
            position = new Vector3(12, 0, -8)
        });/**/

        return island;
    }
}