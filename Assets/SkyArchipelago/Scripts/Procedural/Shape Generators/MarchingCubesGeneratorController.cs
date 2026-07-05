using MarchingCubesProject;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[ExecuteInEditMode]
public class MarchingCubesGeneratorController : MonoBehaviour
{
    public Transform RootModules;
    public MarchingCubesConfigSO config;

    private Vector3Int _offset;
    private DiContainer _diContainer = null;

    [Header("Output")]
    public Material material;
    public LayerMask meshLayer;
    public bool generateOnStart = false;
    public bool useChunking = false;
    public Vector3Int chunkSize = new Vector3Int(32, 32, 32);

    [Inject]
    private void Init(
        DiContainer diContainer)
    {
        _diContainer = diContainer;
        if (generateOnStart) Generate();
    }

    [ContextMenu("Clear")]
    private void Clear()
    {
        Transform parent = chunkParent != null ? chunkParent : transform;

        foreach (Transform child in parent)
        {
            DestroyImmediate(child.gameObject);
        }

        foreach (Transform child in RootModules)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (config == null) return;

        Clear();

        float[,,] density = GenerateDensityField();

        if (useChunking)
            GenerateChunked(density);
        else
            GenerateSingleMesh(density);

        ProcessContentItems();
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
                        float shapeValue = instance.shape.GetShape.Evaluate(pos + instance.shapeOffset, gs, GetRndSeed());

                        value = ApplyOperation(value, shapeValue, instance.operation, instance.smoothK);
                    }

                    foreach (var cont in config.contentInstances)
                    {
                        if (!cont.Entity.IsCuttingWeight)
                            continue;

                        if (cont.Entity.TryGetDensityInfluence(pos, gs, cont.positionOffset, GetRndSeed(), out float contValue))
                        {
//                            Debug.Log($"Try appl Density {contValue} in {pos}");
                            value = SmoothSubtraction(value, -contValue, cont.Entity.smoothK);
                        }
                    }

                    density[x, y, z] = value;
                }

        return ClampEmptyBorder(density, out _offset);
    }

    private float[,,] ClampEmptyBorder(float[,,] density, out Vector3Int offset, int padding = 1)
    {
        offset = Vector3Int.zero;
        int w = density.GetLength(0);
        int h = density.GetLength(1);
        int d = density.GetLength(2);

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

        Debug.Log($"Clamped density: x:{newMinX}->{newMaxX} (size {newMaxX - newMinX + 1}), " +
                  $"y:{newMinY}->{newMaxY} (size {newMaxY - newMinY + 1}), " +
                  $"z:{newMinZ}->{newMaxZ} (size {newMaxZ - newMinZ + 1}), orig {w}x{h}x{d}");

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

        return result;
    }

    private int GetRndSeed()
    {
        return Random.Range(0, 1000000);
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
        int w = density.GetLength(0);
        int h = density.GetLength(1);
        int d = density.GetLength(2);
        Vector3Int centr = config.gridSize / 2 - _offset;
            //new Vector3Int(w / 2, h / 2, d / 2);
        var mesh = GenerateMeshFromDensity(density, config.surfaceLevel, config.cellSize);

        CreateMeshObject(mesh, "Island_Mesh", -centr);
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
        int width = density.GetLength(0);
        int height = density.GetLength(1);
        int depth = density.GetLength(2);

        var voxelArray = new VoxelArray(width, height, depth);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
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
    public Transform chunkParent;

    private void CreateMeshObject(Mesh mesh, string name, Vector3 localPosition)
    {
        Transform parent = chunkParent != null ? chunkParent : transform;

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.layer = (int)Mathf.Log(meshLayer.value, 2);

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        var col = go.AddComponent<MeshCollider>();

        mf.mesh = mesh;
        mr.material = material;
        col.sharedMesh = mesh;
    }

    private void ProcessContentItems()
    {
        foreach (var item in config.contentInstances)
        {
            //show debug content
        }
    }

    [Header("Gizmo")]
    public bool GizmoOn = false;
    public float transparent = 0.5f;
    public float CenterRadius = 1f;

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(RootModules.transform.position, CenterRadius);

        if (!config || !GizmoOn)
            return;

        var gizCol = Color.yellow;
        gizCol.a = transparent;
        Gizmos.color = gizCol;

        foreach (var shapeInst in config.shapes)
        {
            var centr = -shapeInst.shapeOffset + transform.position;
            Gizmos.DrawCube(centr, shapeInst.shape.GetShape.GetSizeBound());
        }

        gizCol = Color.cyan;
        gizCol.a = transparent;
        Gizmos.color = gizCol;

        foreach (var cont in config.contentInstances)
        {
            var centr = cont.positionOffset + transform.position;
            Gizmos.DrawCube(centr, cont.Entity.GetSizeBound());
        }
    }
}