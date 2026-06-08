using System.Collections.Generic;
using UnityEngine;

public class BladeOfWeaponGenerator : ProceduralPartGenerator<BladeTopologySO, BladeModelShape>
{
    public override void ApplyShape(Mesh mesh, BladeTopologySO topo, BladeModelShape shape)
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

    protected override void GeneratePols(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs, BladeTopologySO topo)
    {
        float halfL = topo.bladeModelShape.bladeLength * 0.5f;
        float halfW = topo.bladeModelShape.bladeWidth * 0.5f;
        float halfT = topo.bladeModelShape.bladeThickness * 0.5f;

        for (int i = 0; i <= topo.bladeSegments; i++)
        {
            float t = i / (float)topo.bladeSegments;
            float y = -halfL + t * topo.bladeModelShape.bladeLength;

            // === Профиль ширины ===
            float localT;
            float currentWidth;

            if (t * topo.bladeModelShape.bladeLength < topo.bladeModelShape.bladeBaseLength) // Широкая часть
            {
                localT = (t * topo.bladeModelShape.bladeLength) / topo.bladeModelShape.bladeBaseLength;
                currentWidth = halfW * Mathf.Lerp(1f, topo.bladeModelShape.bladeTaperScale, localT * 0.3f); // лёгкое предварительное сужение
            }
            else // Сужающаяся часть
            {
                float tipProgress = ((t * topo.bladeModelShape.bladeLength) - topo.bladeModelShape.bladeBaseLength) / topo.bladeModelShape.bladeTipLength;
                currentWidth = halfW * topo.bladeModelShape.bladeTaperScale * (1f - tipProgress);
            }

            float currThick = halfT * (1f - t * 0.75f);
            float edge = currentWidth * topo.bladeModelShape.bladeSharpness;

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

        for (int i = 0; i < topo.bladeSegments; i++)
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

        int lastRing = topo.bladeSegments * vertsPerRing;
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
}