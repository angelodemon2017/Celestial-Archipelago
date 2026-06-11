using MarchingCubesProject;
using System.Collections.Generic;
using UnityEngine;

public class FloatingIslandGenerator : MonoBehaviour
{
    public FloatingIslandConfig config;

    [ContextMenu("Generate New Island")]
    public void Generate()
    {
        if (config == null) { Debug.LogError("No config!"); return; }

        float[,,] density = GenerateDensityField();
        Mesh mesh = CreateMeshFromDensity(density);
        CreateIslandObject(mesh);
    }

    private float[,,] GenerateDensityField()
    {
        int w = config.gridSize.x;
        int h = config.gridSize.y;
        int d = config.gridSize.z;
        float[,,] density = new float[w, h, d];
        var centerGrid = config.gridSize / 2;

        Random.InitState(config.seed);
        GenerateRandomConeCenters();

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                for (int z = 0; z < d; z++)
                {
                    Vector3 wp = new Vector3(x, y, z);

                    float weight = float.MinValue;

                    foreach (var centerOffset in config.coneCenters)
                    {
                        Vector3Int center = centerGrid + centerOffset;
                        float shape = GetTestCone(wp, center);

                        weight = SmoothUnion(weight, shape, config.smoothRadius);
                    }

                    density[x, y, z] = weight;
                }

        return density;
    }

    [ContextMenu("Generate Random Cone Centers")]
    public void GenerateRandomConeCenters()
    {
        if (config == null)
        {
            Debug.LogError("Config is null!");
            return;
        }

        config.coneCenters.Clear();
        Random.InitState(config.seed);

        Vector3Int mainCenter = config.gridSize / 2;

        for (int i = 0; i < config.numCones; i++)
        {
            Vector3Int candidate;
            bool valid = false;
            int attempts = 0;

            do
            {
                candidate = new Vector3Int(
                    Random.Range(config.minOffset.x, config.maxOffset.x + 1),
                    Random.Range(config.minOffset.y, config.maxOffset.y + 1),
                    Random.Range(config.minOffset.z, config.maxOffset.z + 1)
                );

                // Проверяем расстояние до уже существующих центров
                valid = true;
                foreach (var existing in config.coneCenters)
                {
                    if (Vector3Int.Distance(candidate, existing) < config.minDistanceBetweenCones)
                    {
                        valid = false;
                        break;
                    }
                }

                attempts++;
            }
            while (!valid && attempts < 50); // защита от бесконечного цикла

            config.coneCenters.Add(candidate);
        }

        Debug.Log($"Generated {config.coneCenters.Count} random cone centers.");
    }

    private float SmoothUnion(float a, float b, float k = 1.0f)
    {
        float h = Mathf.Max(k - Mathf.Abs(a - b), 0.0f) / k;
        return Mathf.Max(a, b) + h * h * k * 0.25f;
    }

    public float ampl;
    public float widthWave;
    public float testSwiftWeight;
    public float testGroundWeight;
    public float testAirWeight;
    [Header("cone")]
    public float radCone;
    public float hightSwift;
    public float heightOfHat = 1f;
    public int swiftSecondCone = 0;

    private float GetTestCone(Vector3 wp, Vector3Int center)
    {
        Vector3 localPos = wp - center;

        float coneHeight = hightSwift;
        float maxRadius = radCone;

        // Сдвиг широкой части
        localPos.y += coneHeight * 0.55f;

        // Перевёрнутый конус
        float normalizedFromTop = Mathf.Clamp01(localPos.y / coneHeight);
        float radiusAtThisLevel = maxRadius * normalizedFromTop;

        float distToAxis = new Vector2(localPos.x, localPos.z).magnitude;

        float density = radiusAtThisLevel - distToAxis;

        if (localPos.y < 0f)
        {
            density -= Mathf.Abs(localPos.y) * 5f;
        }

        if (localPos.y > coneHeight + heightOfHat)
        {
            density = testAirWeight;
        }

        // Шум по краям
        float noise = Mathf.PerlinNoise(wp.x * 1.5f + config.seed, wp.z * 1.5f + config.seed * 1.2f);
        density += (noise - 0.5f) * 2f;

        return density;
    }

    private float GetTestSphere(Vector3 wp, Vector3Int center)
    {
        //return Mathf.Sin(wp.x * widthWave) * Mathf.Sin(wp.z * widthWave) * ampl + center.y - wp.y;
        var sphereWeight = center.magnitude - Vector3.Distance(wp, center) - center.magnitude / 2;
        return sphereWeight;

        return 0f;
    }

    private Vector3 GetWorldPosition(int x, int y, int z)
    {
        return new Vector3(
            (x - config.gridSize.x * 0.5f) * config.cellSize,
            (y - config.bottomThickness + config.centerYOffset) * config.cellSize, // bottomThickness можно добавить в Config
            (z - config.gridSize.z * 0.5f) * config.cellSize
        );
    }

    private float CalculateDiamondBase(Vector3 wp)
    {
        float distXZ = new Vector2(wp.x, wp.z).magnitude;
        float normDist = distXZ / config.baseRadius;

        // ЖЁСТКИЙ CUTOFF — это главное сейчас
        if (normDist > 1.05f)
            return -10f; // сильно отрицательное значение за пределами острова

        float vertical = Mathf.Abs(wp.y) / config.maxHeight;

        float shape = config.maxHeight * (1f - normDist * config.edgeFalloff)
                    * (1f - vertical * 1.9f)
                    - Mathf.Abs(wp.y) * 0.8f;

        return shape;
    }

    private float GetIrregularPerimeter(Vector3 wp)
    {
        // Несколько слоёв шума с разными частотами и сдвигами
        float n1 = Mathf.PerlinNoise(wp.x * config.perimeterNoiseScale + config.seed * 0.3f,
                                    wp.z * config.perimeterNoiseScale * 1.3f + config.seed * 1.7f);

        float n2 = Mathf.PerlinNoise(wp.x * config.perimeterNoiseScale * 2.4f + config.seed * 2.1f,
                                    wp.z * config.perimeterNoiseScale * 1.8f + config.seed * 0.9f);

        float n3 = Mathf.PerlinNoise(wp.x * config.perimeterNoiseScale * 0.6f,
                                    wp.z * config.perimeterNoiseScale * 0.7f + config.seed * 3.3f);

        // Worley-like эффект через abs + sin
        float angular = Mathf.Sin(wp.x * 0.045f + wp.z * 0.037f + config.seed) * 0.5f + 0.5f;

        float combined = (n1 * 0.5f + n2 * 0.35f + n3 * 0.25f) + angular * 0.25f;

        return 0.65f + combined * 0.75f; // сильно варьирует радиус
    }

    private float ApplyPerimeterNoise(Vector3 wp)
    {
        float noise = Mathf.PerlinNoise(wp.x * config.perimeterNoiseScale + config.seed,
                                       wp.z * config.perimeterNoiseScale * 1.4f + config.seed * 2.1f);
        return 0.75f + noise * 0.55f; // делает периметр неровным
    }

    private float GenerateStalactites(Vector3 wp, float perimeterMod)
    {
        if (wp.y > 2f) return 0f; // только снизу

        float n1 = Mathf.PerlinNoise(wp.x * config.detailNoiseScale * 2.8f, wp.z * config.detailNoiseScale * 2.5f);
        float n2 = Mathf.PerlinNoise(wp.x * config.detailNoiseScale * 0.6f + config.seed, wp.z * config.detailNoiseScale * 0.7f);

        float strength = config.stalactiteStrength * (1f - Mathf.Abs(wp.y) * 0.3f) * perimeterMod;

        return (n1 * 1.6f + n2 * 1.1f - 0.8f) * strength * Mathf.Exp(wp.y * 1.4f); // чем ниже — тем сильнее
    }

    private float GenerateTopDetails(Vector3 wp)
    {
        if (wp.y < 1f) return 0f;
        float noise = Mathf.PerlinNoise(wp.x * config.mainNoiseScale, wp.z * config.mainNoiseScale);
        return (noise - 0.35f) * config.noiseStrength;
    }

    private float GenerateCaves(Vector3 wp)
    {
        float noise = Mathf.PerlinNoise(wp.x * config.caveNoiseScale, wp.y * config.caveNoiseScale * 1.6f + wp.z * config.caveNoiseScale);
        return noise * config.caveStrength;
    }

    private Mesh CreateMeshFromDensity(float[,,] density)
    {
        var marching = new MarchingCubes();
        marching.Surface = 0f;

        var voxelArray = new VoxelArray(config.gridSize.x, config.gridSize.y, config.gridSize.z);

        for (int x = 0; x < config.gridSize.x; x++)
            for (int y = 0; y < config.gridSize.y; y++)
                for (int z = 0; z < config.gridSize.z; z++)
                    voxelArray[x, y, z] = density[x, y, z];

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        marching.Generate(voxelArray.Voxels, verts, indices);

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }

    private void CreateIslandObject(Mesh mesh)
    {
        GameObject island = new GameObject("FloatingIsland");
        island.transform.SetParent(transform, false);
        island.transform.localPosition = new Vector3(0, 10f, 0);

        var mf = island.AddComponent<MeshFilter>();
        var mr = island.AddComponent<MeshRenderer>();
        var col = island.AddComponent<MeshCollider>();

        mf.mesh = mesh;
        mr.material = config.material != null ? config.material : new Material(Shader.Find("Universal Render Pipeline/Lit"));
        col.sharedMesh = mesh;

        Debug.Log($"Island generated! Verts: {mesh.vertexCount}");
    }

    [Header("Gizmos Visualization")]
    public bool showGridBounds = true;
    public Color gridColor = new Color(0.2f, 0.8f, 1f, 0.5f);
    public Color centerColor = Color.yellow;

    private void OnDrawGizmos()
    {
        if (config == null || !showGridBounds) return;

        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3Int size = config.gridSize;
        float cell = config.cellSize;

        // Центр грида
        Vector3 center = new Vector3(size.x / 2, size.y / 2, size.z / 2);
            //GetWorldPosition(size.x / 2, size.y / 2, size.z / 2);

        // Размер бокса в мировых координатах
        Vector3 boundsSize = new Vector3(
            size.x * cell,
            size.y * cell,
            size.z * cell
        );

        // Рисуем полупрозрачный бокс
        Gizmos.color = gridColor;
        Gizmos.DrawWireCube(center, boundsSize);

        // Опционально: более жирные линии по границам
        Gizmos.color = new Color(gridColor.r, gridColor.g, gridColor.b, 0.8f);
        Gizmos.DrawWireCube(center, boundsSize);

        // Центр
        Gizmos.color = centerColor;
        Gizmos.DrawSphere(center, cell * 0.3f);

        // Маленькие маркеры по углам (для понимания ориентации)
        DrawCornerMarkers(center, boundsSize * 0.5f, cell);
    }

    private void DrawCornerMarkers(Vector3 center, Vector3 halfSize, float cellSize)
    {
        Vector3[] corners = new Vector3[8];
        int idx = 0;
        for (int x = -1; x <= 1; x += 2)
            for (int y = -1; y <= 1; y += 2)
                for (int z = -1; z <= 1; z += 2)
                {
                    corners[idx++] = center + new Vector3(x * halfSize.x, y * halfSize.y, z * halfSize.z);
                }

        Gizmos.color = Color.red;
        foreach (var c in corners)
        {
            Gizmos.DrawSphere(c, cellSize * 0.15f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        OnDrawGizmos(); // чтобы лучше видно было при выделении
    }
}