using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Blade Topology")]
public class BladeTopologySO : BaseMeshTopologySO
{
    public BladeModelShape bladeModelShape;

    public int bladeSegments = 24;

    public override ProceduralMeshGenerator MeshGenerator => new BladeMeshGenerator(this);

    public override BaseModelShape DefShape => bladeModelShape;

    public override void GeneratePols(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector2> uvs)
    {
        float halfL = bladeModelShape.bladeLength * 0.5f;
        float halfW = bladeModelShape.bladeWidth * 0.5f;
        float halfT = bladeModelShape.bladeThickness * 0.5f;

        for (int i = 0; i <= bladeSegments; i++)
        {
            float t = i / (float)       bladeSegments;
            float y = -halfL + t * bladeModelShape.bladeLength;

            // === Профиль ширины ===
            float localT;
            float currentWidth;

            if (t * bladeModelShape.bladeLength < bladeModelShape.bladeBaseLength) // Широкая часть
            {
                localT = (t * bladeModelShape.bladeLength) / bladeModelShape.bladeBaseLength;
                currentWidth = halfW * Mathf.Lerp(1f, bladeModelShape.bladeTaperScale, localT * 0.3f); // лёгкое предварительное сужение
            }
            else // Сужающаяся часть
            {
                float tipProgress = ((t * bladeModelShape.bladeLength) - bladeModelShape.bladeBaseLength) / bladeModelShape.bladeTipLength;
                currentWidth = halfW * bladeModelShape.bladeTaperScale * (1f - tipProgress);
            }

            float currThick = halfT * (1f - t * 0.75f);
            float edge = currentWidth * bladeModelShape.bladeSharpness;

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

        for (int i = 0; i < bladeSegments; i++)
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

        int lastRing = bladeSegments * vertsPerRing;
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