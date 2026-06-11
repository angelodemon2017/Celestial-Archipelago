using MarchingCubesProject;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MarchingCubesGeneratorController : MonoBehaviour
{
    public MarchingCubesConfigSO config;

    [Header("Output")]
    public Material material;
    public bool generateOnStart = false;
    public bool useChunking = false;
    public Vector3Int chunkSize = new Vector3Int(32, 32, 32);

    private void Start()
    {
        if (generateOnStart) Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (config == null) return;

        float[,,] density = GenerateDensityField();

        if (useChunking)
            GenerateChunked(density);
        else
            GenerateSingleMesh(density);
    }

    private float[,,] GenerateDensityField()
    {
        var gs = config.gridSize;
        float[,,] density = new float[gs.x, gs.y, gs.z];

        for (int x = 0; x < gs.x; x++)
            for (int y = 0; y < gs.y; y++)
                for (int z = 0; z < gs.z; z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    float value = -1000f;

                    foreach (var instance in config.shapes)
                    {
                        float shapeValue = instance.shape.GetShape.Evaluate(pos + instance.shapeOffset, gs, config.globalSeed);

                        value = ApplyOperation(value, shapeValue, instance.operation, instance.smoothK);
                    }

                    density[x, y, z] = value;
                }

        return density;
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

    private void GenerateSingleMesh(float[,,] density)
    {
        var mesh = GenerateMeshFromDensity(density, config.surfaceLevel, config.cellSize);

        CreateMeshObject(mesh, "Island_Mesh", Vector3.zero);
    }

    private void GenerateChunked(float[,,] fullDensity)
    {
        Vector3Int gridSize = config.gridSize;
        Vector3Int chunkSize = this.chunkSize;

        int chunksX = Mathf.CeilToInt((float)gridSize.x / chunkSize.x);
        int chunksY = Mathf.CeilToInt((float)gridSize.y / chunkSize.y);
        int chunksZ = Mathf.CeilToInt((float)gridSize.z / chunkSize.z);

        Debug.Log($"Generating {chunksX}×{chunksY}×{chunksZ} chunks...");

        for (int cx = 0; cx < chunksX; cx++)
            for (int cy = 0; cy < chunksY; cy++)
                for (int cz = 0; cz < chunksZ; cz++)
                {
                    Vector3Int chunkOrigin = new Vector3Int(
                        cx * chunkSize.x,
                        cy * chunkSize.y,
                        cz * chunkSize.z
                    );

                    float[,,] chunkDensity = ExtractChunkDensity(fullDensity, chunkOrigin, chunkSize, gridSize);

                    Mesh chunkMesh = GenerateMeshFromDensity(chunkDensity,
                        config.surfaceLevel,
                        config.cellSize);

                    if (chunkMesh.vertexCount == 0) continue;

                    Vector3 worldOffset = new Vector3(
                        chunkOrigin.x * config.cellSize,
                        chunkOrigin.y * config.cellSize,
                        chunkOrigin.z * config.cellSize
                    );

                    CreateMeshObject(chunkMesh, $"Chunk_{cx}_{cy}_{cz}", worldOffset);
                }
    }

    private Mesh GenerateMeshFromDensity(float[,,] density, float surfaceLevel, float cellSize)
    {
        var voxelArray = new VoxelArray(config.gridSize.x, config.gridSize.y, config.gridSize.z);

        for (int x = 0; x < config.gridSize.x; x++)
            for (int y = 0; y < config.gridSize.y; y++)
                for (int z = 0; z < config.gridSize.z; z++)
                    voxelArray[x, y, z] = density[x, y, z];

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        var marching = new MarchingCubes();
        marching.Surface = surfaceLevel;
        marching.Generate(voxelArray.Voxels, verts, indices);

        List<Color> colors = new List<Color>(verts.Count);
        for (int i = 0; i < verts.Count; i++)
        {
            Color col = config.GetVertexColor(verts[i]);
            colors.Add(col);
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(verts);
        mesh.SetTriangles(indices, 0);
        mesh.SetColors(colors);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }

    private float[,,] ExtractChunkDensity(float[,,] fullDensity, Vector3Int origin, Vector3Int chunkSize, Vector3Int fullSize)
    {
        // Добавляем 1 voxel padding для корректного Marching Cubes на границах
        int paddedX = chunkSize.x + 2;
        int paddedY = chunkSize.y + 2;
        int paddedZ = chunkSize.z + 2;

        float[,,] chunk = new float[paddedX, paddedY, paddedZ];

        for (int x = 0; x < paddedX; x++)
            for (int y = 0; y < paddedY; y++)
                for (int z = 0; z < paddedZ; z++)
                {
                    int fx = origin.x + x - 1;
                    int fy = origin.y + y - 1;
                    int fz = origin.z + z - 1;

                    if (fx >= 0 && fx < fullSize.x && fy >= 0 && fy < fullSize.y && fz >= 0 && fz < fullSize.z)
                    {
                        chunk[x, y, z] = fullDensity[fx, fy, fz];
                    }
                    else
                    {
                        chunk[x, y, z] = -1000f; // воздух
                    }
                }

        return chunk;
    }

    [Header("Chunking Settings")]
    public Transform chunkParent; // если хочешь отдельный объект-контейнер

    private void CreateMeshObject(Mesh mesh, string name, Vector3 localPosition)
    {
        GameObject go = new GameObject(name);

        Transform parent = chunkParent != null ? chunkParent : transform;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        var col = go.AddComponent<MeshCollider>();

        mf.mesh = mesh;
        mr.material = material ?? new Material(Shader.Find("Universal Render Pipeline/Lit"));
        col.sharedMesh = mesh;

        // Опционально: LODGroup позже
    }
}