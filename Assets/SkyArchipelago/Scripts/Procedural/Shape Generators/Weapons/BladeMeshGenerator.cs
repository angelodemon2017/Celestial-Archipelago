using System.Collections.Generic;
using UnityEngine;

public class BladeMeshGenerator : ProceduralMeshGeneratorByConfig<BladeTopologySO>
{
    public BladeMeshGenerator(BladeTopologySO topConfig) : base(topConfig) { }

    protected override void GeneratePols(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        float halfL = _topology.bladeModelShape.bladeLength * 0.5f;
        float halfW = _topology.bladeModelShape.bladeWidth * 0.5f;
        float halfT = _topology.bladeModelShape.bladeThickness * 0.5f;

        for (int i = 0; i <= _topology.bladeSegments; i++)
        {
            float t = i / (float)_topology.bladeSegments;
            float y = -halfL + t * _topology.bladeModelShape.bladeLength;

            // === Профиль ширины ===
            float localT;
            float currentWidth;

            if (t * _topology.bladeModelShape.bladeLength < _topology.bladeModelShape.bladeBaseLength) // Широкая часть
            {
                localT = (t * _topology.bladeModelShape.bladeLength) / _topology.bladeModelShape.bladeBaseLength;
                currentWidth = halfW * Mathf.Lerp(1f, _topology.bladeModelShape.bladeTaperScale, localT * 0.3f); // лёгкое предварительное сужение
            }
            else // Сужающаяся часть
            {
                float tipProgress = ((t * _topology.bladeModelShape.bladeLength) - _topology.bladeModelShape.bladeBaseLength) / _topology.bladeModelShape.bladeTipLength;
                currentWidth = halfW * _topology.bladeModelShape.bladeTaperScale * (1f - tipProgress);
            }

            float currThick = halfT * (1f - t * 0.75f);
            float edge = currentWidth * _topology.bladeModelShape.bladeSharpness;

            // 8-гранное сечение
            vertices.Add(new Vector3(-currThick, y, 0));                    // 0
            vertices.Add(new Vector3(-currThick * 0.5f, y, -edge));        // 1
            vertices.Add(new Vector3(0, y, -currentWidth));                // 2
            vertices.Add(new Vector3(currThick * 0.5f, y, -edge));         // 3
            vertices.Add(new Vector3(currThick, y, 0));                    // 4
            vertices.Add(new Vector3(currThick * 0.5f, y, edge));          // 5
            vertices.Add(new Vector3(0, y, currentWidth));                 // 6
            vertices.Add(new Vector3(-currThick * 0.5f, y, edge));         // 7
        }

        int vertsPerRing = 8;

        for (int i = 0; i < _topology.bladeSegments; i++)
        {
            int b = i * vertsPerRing;
            int t = b + vertsPerRing;

            for (int j = 0; j < 8; j++)
            {
                int a = b + j;
                int nextA = b + (j + 1) % 8;
                int c = t + j;
                int nextC = t + (j + 1) % 8;

                triangles.Add(a); triangles.Add(c); triangles.Add(nextA);
                triangles.Add(nextA); triangles.Add(c); triangles.Add(nextC);
            }
        }

        // Острый кончик
        int tipIndex = vertices.Count;
        vertices.Add(new Vector3(0, halfL, 0));

        int lastRing = _topology.bladeSegments * vertsPerRing;
        for (int i = 0; i < 8; i++)
        {
            int a = lastRing + i;
            int b = lastRing + ((i + 1) % 8);
            triangles.Add(tipIndex);
            triangles.Add(a);
            triangles.Add(b);
        }

        for (int i = 0; i < vertices.Count; i++)
            uvs.Add(new Vector2(0.5f, (float)i / vertices.Count));
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