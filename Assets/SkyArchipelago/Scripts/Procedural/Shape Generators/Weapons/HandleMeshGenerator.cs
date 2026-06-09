using System.Collections.Generic;
using UnityEngine;

public class HandleMeshGenerator : ProceduralMeshGeneratorByConfig<HandleTopologySO>
{
    public HandleMeshGenerator(HandleTopologySO topConfig) : base(topConfig) { }

    protected override void GeneratePols(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        float halfLength = _topology.handleModelShape.handleLength * 0.5f;

        // Основные кольца
        for (int ring = 0; ring <= _topology.rings; ring++)
        {
            float t = ring / (float)_topology.rings;
            float y = -halfLength + t * _topology.handleModelShape.handleLength;
            float wave = Mathf.Sin(t * Mathf.PI * 7f) * 0.028f;

            for (int i = 0; i <= _topology.segments; i++)
            {
                float angle = i * Mathf.PI * 2f / _topology.segments;
                float r = _topology.handleModelShape.handleRadius + wave;
                float x = Mathf.Cos(angle) * r;
                float z = Mathf.Sin(angle) * r;

                vertices.Add(new Vector3(x, y, z));
                uvs.Add(new Vector2((float)i / _topology.segments, t));
            }
        }

        int vertsPerRing = _topology.segments + 1;

        // Боковая поверхность (исправленный winding)
        for (int ring = 0; ring < _topology.rings; ring++)
        {
            for (int i = 0; i < _topology.segments; i++)
            {
                int curr = ring * vertsPerRing + i;
                int next = curr + 1;
                int up = curr + vertsPerRing;
                int upNext = up + 1;

                triangles.Add(curr); triangles.Add(up); triangles.Add(next);      // ← исправлено
                triangles.Add(next); triangles.Add(up); triangles.Add(upNext);
            }
        }

        // Bottom cap (нижний конец)
        int bottomCenter = vertices.Count;
        vertices.Add(new Vector3(0, -halfLength, 0));
        uvs.Add(new Vector2(0.5f, 0f));

        for (int i = 0; i < _topology.segments; i++)
        {
            int a = i;
            int b = (i + 1) % _topology.segments;
            triangles.Add(bottomCenter);
            triangles.Add(b);   // порядок для outward normal
            triangles.Add(a);
        }

        // Top cap (верхний конец)
        int topRingStart = _topology.rings * vertsPerRing;
        int topCenter = vertices.Count;
        vertices.Add(new Vector3(0, halfLength, 0));
        uvs.Add(new Vector2(0.5f, 1f));

        for (int i = 0; i < _topology.segments; i++)
        {
            int a = topRingStart + i;
            int b = topRingStart + (i + 1) % _topology.segments;
            triangles.Add(topCenter);
            triangles.Add(a);
            triangles.Add(b);
        }
    }

    public override void ApplyShape<T>(Mesh mesh, T model)
    {
        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            //apply shape
        }
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}