using System.Collections.Generic;
using UnityEngine;

public class WorldGeneratorService
{
    private readonly CatalogIslandConfigs _catalogConfig;
    private readonly DataService _dataService;
    private readonly WorldGeneratorConfig _worldGeneratorConfig;

    public WorldGeneratorService(
        CatalogIslandConfigs catalogConfig,
        DataService dataService,
        WorldGeneratorConfig worldGeneratorConfig)
    {
        _catalogConfig = catalogConfig;
        _dataService = dataService;
        _worldGeneratorConfig = worldGeneratorConfig;
    }

    public void GenerateNewWorld()
    {
        _dataService.CreateNewWorld();
        GenerateChunk(0,0);
    }

    public void GenerateChunk(int x, int y)
    {
        List<IslandData> cashIslands = new();

        if (x == 0 && y == 0)
        {
            cashIslands.Add(GenStartIsland(_catalogConfig.StartIslandConfig, new Vector3(0f, 0f, 0f)));
        }

        var radians = 360 / _worldGeneratorConfig.CountTestIslands * Mathf.Deg2Rad;
        var radius = _worldGeneratorConfig.RadiusTestIslands;
        for (int i = 0; i < _worldGeneratorConfig.CountTestIslands; i++)
        {
            var radPos = new Vector3(radius * Mathf.Cos(radians * i), 0f, radius * Mathf.Sin(radians * i));
            var island = GenStartIsland(_catalogConfig.TestIslands[0], radPos);
            cashIslands.Add(island);
        }

        _dataService.SetChunk(x, y, cashIslands);
    }

    private IslandData GenStartIsland(MarchingCubesConfigSO _config, Vector3 position)
    {
        IslandData resultIsland = new IslandData();

        int seed = _dataService.GetSeed + (int)position.x - (int)position.z;
        resultIsland.ConfigId = _config.IdConfig;
        resultIsland.Position = position;
        var gs = _config.gridSize;
        var tempdensity = new float[gs.x, gs.y, gs.z];

        for (int x = 0; x < gs.x; x++)
            for (int y = 0; y < gs.y; y++)
                for (int z = 0; z < gs.z; z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    float value = -1000f;

                    foreach (var instance in _config.shapes)
                    {
                        float shapeValue = instance.shape.GetShape.Evaluate(pos + instance.shapeOffset, gs, seed);

                        value = ApplyOperation(value, shapeValue, instance.operation, instance.smoothK);
                    }

                    foreach (var cont in _config.contentItems)
                    {
                        if (!cont.IsCuttingWeight)
                            continue;

                        if (cont.TryGetDensityInfluence(pos, gs, seed, out float contValue))
                        {
                            value = SmoothSubtraction(value, -contValue, cont.smoothK);
                        }
                    }

                    tempdensity[x, y, z] = value;
                }

        resultIsland.density = ClampEmptyBorder(tempdensity,
            out Vector3Int offset, out resultIsland.IslandSize);

        resultIsland.Center = -(_config.gridSize / 2 - offset);

        return resultIsland;
    }

    private float[,,] ClampEmptyBorder(float[,,] density, out Vector3Int offset, out Vector3Int size, int padding = 1)
    {
        offset = Vector3Int.zero;
        int w = density.GetLength(0);
        int h = density.GetLength(1);
        int d = density.GetLength(2);
        size = new Vector3Int(w, h, d);

        if (w <= 6 || h <= 6 || d <= 6)
            return density;

        int minX = w, maxX = -1;
        int minY = h, maxY = -1;
        int minZ = d, maxZ = -1;

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                for (int z = 0; z < d; z++)
                {
                    if (density[x, y, z] > 0f)
                    {
                        minX = Mathf.Min(minX, x);
                        maxX = Mathf.Max(maxX, x);
                        minY = Mathf.Min(minY, y);
                        maxY = Mathf.Max(maxY, y);
                        minZ = Mathf.Min(minZ, z);
                        maxZ = Mathf.Max(maxZ, z);
                    }
                }

        int newMinX = Mathf.Max(0, minX - padding);
        int newMaxX = Mathf.Min(w - 1, maxX + padding);
        int newMinY = Mathf.Max(0, minY - padding);
        int newMaxY = Mathf.Min(h - 1, maxY + padding);
        int newMinZ = Mathf.Max(0, minZ - padding);
        int newMaxZ = Mathf.Min(d - 1, maxZ + padding);

/*        Debug.Log($"Clamped density: x:{newMinX}->{newMaxX} (size {newMaxX - newMinX + 1}), " +
                  $"y:{newMinY}->{newMaxY} (size {newMaxY - newMinY + 1}), " +
                  $"z:{newMinZ}->{newMaxZ} (size {newMaxZ - newMinZ + 1}), orig {w}x{h}x{d}");/**/

        if (newMinX == 0 && newMaxX == w - 1 &&
            newMinY == 0 && newMaxY == h - 1 &&
            newMinZ == 0 && newMaxZ == d - 1)
            return density;

        int nw = newMaxX - newMinX + 1;
        int nh = newMaxY - newMinY + 1;
        int nd = newMaxZ - newMinZ + 1;

        float[,,] result = new float[nw, nh, nd];

        for (int x = 0; x < nw; x++)
            for (int y = 0; y < nh; y++)
                for (int z = 0; z < nd; z++)
                {
                    result[x, y, z] = density[newMinX + x, newMinY + y, newMinZ + z];
                }
        offset = new Vector3Int(newMinX, newMinY, newMinZ);
        size.x = nw;
        size.y = nh;
        size.z = nd;

        return result;
    }

    private float ApplyOperation(float a, float b, ShapeOperation op, float k)
    {
        switch (op)
        {
            case ShapeOperation.Union: return Mathf.Max(a, b);
            case ShapeOperation.SmoothUnion: return SmoothUnion(a, b, k);
            case ShapeOperation.Subtraction: return Mathf.Min(a, -b);
            case ShapeOperation.SmoothSubtraction: return SmoothSubtraction(a, b, k);
            case ShapeOperation.Intersection: return Mathf.Min(a, b);
            case ShapeOperation.SmoothIntersection: return SmoothIntersection(a, b, k);
            default: return Mathf.Max(a, b);
        }
    }

    private float SmoothUnion(float a, float b, float k) =>
    Mathf.Max(a, b) + Mathf.Pow(Mathf.Max(k - Mathf.Abs(a - b), 0f) / k, 2) * k * 0.25f;

    private float SmoothSubtraction(float a, float b, float k) =>
        Mathf.Min(a, -b) + Mathf.Pow(Mathf.Max(k - Mathf.Abs(-a - b), 0f) / k, 2) * k * 0.25f;

    private float SmoothIntersection(float a, float b, float k) =>
        Mathf.Min(a, b) + Mathf.Pow(Mathf.Max(k - Mathf.Abs(a - b), 0f) / k, 2) * k * 0.25f;
}