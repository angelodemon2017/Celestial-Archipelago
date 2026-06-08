using UnityEngine;
using System.Collections.Generic;

public static class ProceduralPrimitives
{
    public static Mesh CreateCylinder(float radius, float height, int segments = 16)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float halfHeight = height * 0.5f;
        int seg = Mathf.Max(8, segments);

        // Боковая поверхность
        for (int i = 0; i <= seg; i++)
        {
            float angle = i * Mathf.PI * 2f / seg;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            vertices.Add(new Vector3(x, halfHeight, z));   // top
            uvs.Add(new Vector2((float)i / seg, 1f));

            vertices.Add(new Vector3(x, -halfHeight, z));  // bottom
            uvs.Add(new Vector2((float)i / seg, 0f));
        }

        int topCenter = vertices.Count;
        vertices.Add(new Vector3(0, halfHeight, 0));
        uvs.Add(new Vector2(0.5f, 0.5f));

        int bottomCenter = vertices.Count;
        vertices.Add(new Vector3(0, -halfHeight, 0));
        uvs.Add(new Vector2(0.5f, 0.5f));

        // Боковые грани (исправленный winding order)
        for (int i = 0; i < seg; i++)
        {
            int top1 = i * 2;
            int top2 = ((i + 1) % seg) * 2;
            int bot1 = top1 + 1;
            int bot2 = top2 + 1;

            // Counter-clockwise для outward normals
            triangles.Add(top1); triangles.Add(top2); triangles.Add(bot1);
            triangles.Add(top2); triangles.Add(bot2); triangles.Add(bot1);
        }

        // Крышки
        for (int i = 0; i < seg; i++)
        {
            int current = i * 2;
            int next = ((i + 1) % seg) * 2;

            // Top cap
            triangles.Add(topCenter); triangles.Add(next); triangles.Add(current);
            // Bottom cap
            triangles.Add(bottomCenter); triangles.Add(current + 1); triangles.Add(next + 1);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh CreateWavyHandle(float radius = 0.08f, float length = 1.2f, int segments = 24, int rings = 18)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float halfLength = length * 0.5f;

        // Основные кольца
        for (int ring = 0; ring <= rings; ring++)
        {
            float t = ring / (float)rings;
            float y = -halfLength + t * length;
            float wave = Mathf.Sin(t * Mathf.PI * 7f) * 0.028f;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                float r = radius + wave;
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

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh CreateBox(float width, float height, float depth)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float hw = width * 0.5f;
        float hh = height * 0.5f;
        float hd = depth * 0.5f;

        // 8 вершин куба
        vertices.Add(new Vector3(-hw, -hh, hd)); // 0
        vertices.Add(new Vector3(hw, -hh, hd)); // 1
        vertices.Add(new Vector3(hw, hh, hd)); // 2
        vertices.Add(new Vector3(-hw, hh, hd)); // 3

        vertices.Add(new Vector3(-hw, -hh, -hd)); // 4
        vertices.Add(new Vector3(hw, -hh, -hd)); // 5
        vertices.Add(new Vector3(hw, hh, -hd)); // 6
        vertices.Add(new Vector3(-hw, hh, -hd)); // 7

        // Простые UV (можно улучшить позже)
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < 8; i++) uvs.Add(new Vector2(0f, 0f));

        int[] tris = {
            0,1,2,  0,2,3,   // front
            5,4,7,  5,7,6,   // back
            4,0,3,  4,3,7,   // left
            1,5,6,  1,6,2,   // right
            3,2,6,  3,6,7,   // top
            4,5,1,  4,1,0    // bottom
        };

        triangles.AddRange(tris);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh CreateCrossGuard(float width = 0.6f, float thickness = 0.05f, float height = 0.2f)
    {
        // Основная горизонтальная перекладина
        Mesh mainBar = CreateBox(width, thickness * 1.6f, thickness * 3.2f);

        // Вертикальная часть
        Mesh vertical = CreateBox(thickness * 2.4f, height, thickness * 1.4f);

        // Дополнительные "крылья"/детали на концах (как на твоём рисунке)
        Mesh leftWing = CreateBox(thickness * 1.8f, thickness * 2.2f, thickness * 1.2f);
        Mesh rightWing = CreateBox(thickness * 1.8f, thickness * 2.2f, thickness * 1.2f);

        CombineInstance[] combine = new CombineInstance[5];
        combine[0].mesh = mainBar;
        combine[0].transform = Matrix4x4.identity;

        combine[1].mesh = vertical;
        combine[1].transform = Matrix4x4.Translate(new Vector3(0, thickness * 0.3f, 0));

        // Левое крыло
        combine[2].mesh = leftWing;
        combine[2].transform = Matrix4x4.Translate(new Vector3(-width * 0.45f, thickness * 0.4f, 0));

        // Правое крыло
        combine[3].mesh = rightWing;
        combine[3].transform = Matrix4x4.Translate(new Vector3(width * 0.45f, thickness * 0.4f, 0));

        // Центральный элемент
        combine[4].mesh = CreateBox(thickness * 2.8f, thickness * 2.8f, thickness * 2.8f);
        combine[4].transform = Matrix4x4.Translate(new Vector3(0, thickness * 0.2f, 0));

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine, true, true);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh CreateTaperedBox(float w, float h, float d, float taperAmount)
    {
        // Пока используем обычный box (можно сделать tapered позже)
        return CreateBox(w, h, d);
    }

    public static Mesh CreateBlade(float length = 2.5f, float width = 0.25f, float thickness = 0.04f,
                                   float sharpness = 0.95f,
                                   float baseLength = 1.6f, float taperScale = 0.35f, float tipLength = 0.9f,
                                   int segments = 24)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float halfL = length * 0.5f;
        float halfW = width * 0.5f;
        float halfT = thickness * 0.5f;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;                    // 0..1 по всей длине
            float y = -halfL + t * length;

            // === Профиль ширины ===
            float localT;
            float currentWidth;

            if (t * length < baseLength) // Широкая часть
            {
                localT = (t * length) / baseLength;
                currentWidth = halfW * Mathf.Lerp(1f, taperScale, localT * 0.3f); // лёгкое предварительное сужение
            }
            else // Сужающаяся часть
            {
                float tipProgress = ((t * length) - baseLength) / tipLength;
                currentWidth = halfW * taperScale * (1f - tipProgress);
            }

            float currThick = halfT * (1f - t * 0.75f);
            float edge = currentWidth * sharpness;

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

        for (int i = 0; i < segments; i++)
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

        int lastRing = segments * vertsPerRing;
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

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static void AddQuad(List<int> tris, int a, int b, int c, int d)
    {
        tris.Add(a); tris.Add(b); tris.Add(c);
        tris.Add(a); tris.Add(c); tris.Add(d);
    }
}