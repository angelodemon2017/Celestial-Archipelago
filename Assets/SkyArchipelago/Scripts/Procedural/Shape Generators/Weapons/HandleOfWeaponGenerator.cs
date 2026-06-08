using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Generators/Weapon Handle Generator")]
public class HandleOfWeaponGenerator : ProceduralPartGenerator<HandleTopologySO, HandleModelShape>
{
    public override void ApplyShape(Mesh mesh, HandleTopologySO topo, HandleModelShape shape)
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

    protected override void GeneratePols(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, HandleTopologySO topo)
    {
        float halfLength = topo.handleModelShape.handleLength * 0.5f;

        // Основные кольца
        for (int ring = 0; ring <= topo.rings; ring++)
        {
            float t = ring / (float)topo.rings;
            float y = -halfLength + t * topo.handleModelShape.handleLength;
            float wave = Mathf.Sin(t * Mathf.PI * 7f) * 0.028f;

            for (int i = 0; i <= topo.segments; i++)
            {
                float angle = i * Mathf.PI * 2f / topo.segments;
                float r = topo.handleModelShape.handleRadius + wave;
                float x = Mathf.Cos(angle) * r;
                float z = Mathf.Sin(angle) * r;

                vertices.Add(new Vector3(x, y, z));
                uvs.Add(new Vector2((float)i / topo.segments, t));
            }
        }

        int vertsPerRing = topo.segments + 1;

        // Боковая поверхность (исправленный winding)
        for (int ring = 0; ring < topo.rings; ring++)
        {
            for (int i = 0; i < topo.segments; i++)
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

        for (int i = 0; i < topo.segments; i++)
        {
            int a = i;
            int b = (i + 1) % topo.segments;
            triangles.Add(bottomCenter);
            triangles.Add(b);   // порядок для outward normal
            triangles.Add(a);
        }

        // Top cap (верхний конец)
        int topRingStart = topo.rings * vertsPerRing;
        int topCenter = vertices.Count;
        vertices.Add(new Vector3(0, halfLength, 0));
        uvs.Add(new Vector2(0.5f, 1f));

        for (int i = 0; i < topo.segments; i++)
        {
            int a = topRingStart + i;
            int b = topRingStart + (i + 1) % topo.segments;
            triangles.Add(topCenter);
            triangles.Add(a);
            triangles.Add(b);
        }
    }
}