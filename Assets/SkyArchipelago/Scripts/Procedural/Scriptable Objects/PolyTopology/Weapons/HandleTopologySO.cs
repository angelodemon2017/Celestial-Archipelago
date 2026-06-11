using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Handle Topology")]
public class HandleTopologySO : BaseMeshTopologySO
{
    public HandleModelShape handleModelShape;

    public int handleSegments = 12;
    public int rings = 18;
    public int segments = 24;

    public override BaseModelShape DefShape => handleModelShape;

    public override ProceduralMeshGenerator MeshGenerator => new HandleMeshGenerator(this);

    public override void GeneratePols(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        float halfLength = handleModelShape.handleLength * 0.5f;

        // Основные кольца
        for (int ring = 0; ring <= rings; ring++)
        {
            float t = ring / (float)rings;
            float y = -halfLength + t * handleModelShape.handleLength;
            float wave = Mathf.Sin(t * Mathf.PI * 7f) * 0.028f;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                float r = handleModelShape.handleRadius + wave;
                float x = Mathf.Cos(angle) * r;
                float z = Mathf.Sin(angle) * r;

                vertices.Add(new Vector3(x, y, z));
                uvs.Add(new Vector2((float)i / segments, t));
            }
        }

        int vertsPerRing = segments + 1;

        // Боковая поверхность (исправленный winding)
        for (int ring = 0; ring < rings; ring++)
        {
            for (int i = 0; i < segments; i++)
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

        for (int i = 0; i < segments; i++)
        {
            int a = i;
            int b = (i + 1) % segments;
            triangles.Add(bottomCenter);
            triangles.Add(b);   // порядок для outward normal
            triangles.Add(a);
        }

        // Top cap (верхний конец)
        int topRingStart = rings * vertsPerRing;
        int topCenter = vertices.Count;
        vertices.Add(new Vector3(0, halfLength, 0));
        uvs.Add(new Vector2(0.5f, 1f));

        for (int i = 0; i < segments; i++)
        {
            int a = topRingStart + i;
            int b = topRingStart + (i + 1) % segments;
            triangles.Add(topCenter);
            triangles.Add(a);
            triangles.Add(b);
        }
    }
}