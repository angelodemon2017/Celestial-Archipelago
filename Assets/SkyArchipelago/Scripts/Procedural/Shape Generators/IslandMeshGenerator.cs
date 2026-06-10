using System.Collections.Generic;
using UnityEngine;

public class IslandMeshGenerator : ProceduralMeshGeneratorByConfig<IslandTopologySO>
{
    public IslandMeshGenerator(IslandTopologySO topConfig) : base(topConfig)
    {

    }

    protected override void GeneratePols(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        GenerateDiamondIsland(ref vertices, ref triangles, ref uvs, _topology.shape);
    }

    private void GenerateDiamondIsland(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, IslandShape shape)
    {
        int baseVertexCount = vertices.Count;

        GeneratePlateau(ref vertices, ref triangles, ref uvs);
        GenerateTransitionBody(ref vertices, ref triangles, ref uvs);
        GenerateStalactites(ref vertices, ref triangles, ref uvs, shape);

        RecalculateUVsY(vertices, uvs, baseVertexCount, vertices.Count);
    }

    private void GeneratePlateau(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        int segments = _topology.plateauSubdivision;
        float radius = _topology.plateauRadius;
        float height = _topology.plateauHeight;
        float topY = _topology.totalHeight * 0.5f - height * 0.5f;

        // Центр верхней плоскости
        int centerIndex = vertices.Count;
        vertices.Add(new Vector3(0, topY + height * 0.5f, 0));
        uvs.Add(new Vector2(0.5f, 0.5f));

        // Круг верха
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            float noise = Mathf.PerlinNoise(x * 0.15f, z * 0.15f) * _topology.maxHeightVariation * 0.4f;

            vertices.Add(new Vector3(x, topY + height * 0.5f + noise, z));
            uvs.Add(new Vector2((x / radius + 1f) * 0.5f, (z / radius + 1f) * 0.5f));
        }

        // Верх плато — Counter-clockwise
        for (int i = 0; i < segments; i++)
        {
            triangles.Add(centerIndex);                    // центр
            triangles.Add(centerIndex + 1 + (i + 1) % segments);
            triangles.Add(centerIndex + 1 + i);
        }

        // Боковая поверхность
        int sideStart = vertices.Count;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            vertices.Add(new Vector3(x, topY + height * 0.5f, z));   // top
            vertices.Add(new Vector3(x, topY - height * 0.5f, z));   // bottom
            uvs.Add(new Vector2((float)i / segments, 1f));
            uvs.Add(new Vector2((float)i / segments, 0f));
        }

        for (int i = 0; i < segments; i++)
        {
            int a = sideStart + i * 2;           // top
            int b = a + 1;                       // bottom
            int c = sideStart + ((i + 1) % segments) * 2;
            int d = c + 1;

            triangles.Add(a); triangles.Add(c); triangles.Add(b);
            triangles.Add(c); triangles.Add(d); triangles.Add(b);
        }
    }

    private void GenerateTransitionBody(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        int segments = 16;
        float topRadius = _topology.plateauRadius * 0.85f;
        float bottomRadius = _topology.plateauRadius * 0.45f;
        float topY = _topology.totalHeight * 0.5f - _topology.plateauHeight * 0.5f;
        float bottomY = _topology.plateauHeight * -0.3f;

        int start = vertices.Count;

        for (int ring = 0; ring <= 5; ring++)
        {
            float t = ring / 5f;
            float radius = Mathf.Lerp(topRadius, bottomRadius, t);
            float y = Mathf.Lerp(topY, bottomY, t);

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                vertices.Add(new Vector3(x, y, z));
                uvs.Add(new Vector2((float)i / segments, t));
            }
        }

        int vertsPerRing = segments + 1;
        for (int ring = 0; ring < 5; ring++)
        {
            for (int i = 0; i < segments; i++)
            {
                int a = start + ring * vertsPerRing + i;
                int b = a + 1;
                int c = a + vertsPerRing;
                int d = c + 1;

                triangles.Add(a); triangles.Add(b); triangles.Add(c);
                triangles.Add(c); triangles.Add(b); triangles.Add(d);
            }
        }
    }

    private void GenerateStalactites(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, IslandShape shape)
    {
        float baseY = _topology.plateauHeight * -0.3f;
        Random.InitState(shape.Seed);

        for (int i = 0; i < _topology.stalactiteCount; i++)
        {
            float angle = (i * 1.618f + shape.seedOffset) * Mathf.PI * 2f / _topology.stalactiteCount; // golden ratio для равномерности
            float dist = _topology.plateauRadius * (0.4f + Mathf.PerlinNoise(i * 0.3f, shape.Seed * 0.01f) * 0.45f);

            Vector3 center = new Vector3(Mathf.Cos(angle) * dist, baseY, Mathf.Sin(angle) * dist);

            float length = _topology.maxStalactiteLength * (0.65f + Mathf.PerlinNoise(i * 1.7f, shape.Seed * 0.02f) * 0.35f);
            float baseR = _topology.stalactiteBaseRadius * (0.8f + Mathf.PerlinNoise(i * 2.3f, shape.Seed * 0.015f) * 0.55f);

            int startVert = vertices.Count;
            int segments = 10;
            int rings = 7;

            for (int r = 0; r <= rings; r++)
            {
                float t = r / (float)rings;
                float currR = Mathf.Lerp(baseR, _topology.stalactiteTipRadius, t);
                float y = baseY - t * length;

                for (int s = 0; s <= segments; s++)
                {
                    float a = s * Mathf.PI * 2f / segments;
                    float x = center.x + Mathf.Cos(a) * currR;
                    float z = center.z + Mathf.Sin(a) * currR;

                    vertices.Add(new Vector3(x, y, z));
                    uvs.Add(new Vector2((float)s / segments, t));
                }
            }

            int vpr = segments + 1;
            for (int r = 0; r < rings; r++)
            {
                for (int s = 0; s < segments; s++)
                {
                    int a = startVert + r * vpr + s;
                    int b = a + 1;
                    int c = a + vpr;
                    int d = c + 1;

                    triangles.Add(a); triangles.Add(b); triangles.Add(c);
                    triangles.Add(c); triangles.Add(b); triangles.Add(d);
                }
            }

            int tipIndex = vertices.Count;
            vertices.Add(new Vector3(center.x, baseY - length, center.z));
            uvs.Add(new Vector2(0.5f, 1f));

            int lastRing = startVert + rings * vpr;
            for (int s = 0; s < segments; s++)
            {
                triangles.Add(tipIndex);
                triangles.Add(lastRing + s);
                triangles.Add(lastRing + (s + 1) % segments);
            }
        }
    }

    private void RecalculateUVsY(List<Vector3> vertices, List<Vector2> uvs, int startIndex, int endIndex)
    {
        if (vertices.Count == 0) return;

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = startIndex; i < endIndex && i < vertices.Count; i++)
        {
            minY = Mathf.Min(minY, vertices[i].y);
            maxY = Mathf.Max(maxY, vertices[i].y);
        }

        float range = maxY - minY + 0.001f;

        for (int i = startIndex; i < endIndex && i < uvs.Count; i++)
        {
            float t = (vertices[i].y - minY) / range;
            uvs[i] = new Vector2(uvs[i].x, t);
        }
    }
}