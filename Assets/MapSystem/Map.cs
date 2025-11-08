using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent (typeof(MeshCollider))]
public class Map : MonoBehaviour
{
    private Mesh mesh;

    private float heightOffset, height, heightInterval;

    public void Init(float heightOffset, float height, float heightInterval)
    {
        this.height = height;
        this.heightInterval = heightInterval;
        this.heightOffset = heightOffset;
    }

    private void DrawSquare(Square square, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, float currentHeightInterval)
    {
        Vector3 p1 = new(square.Point1.x, heightOffset + currentHeightInterval, square.Point1.y);
        Vector3 p2 = new(square.Point2.x, heightOffset + currentHeightInterval, square.Point2.y);
        Vector3 p3 = new(square.Point3.x, heightOffset + currentHeightInterval, square.Point3.y);
        Vector3 p4 = new(square.Point4.x, heightOffset + currentHeightInterval, square.Point4.y);

        // 천장
        Vector3 p5 = new(square.Point1.x, heightOffset + height + currentHeightInterval, square.Point1.y);
        Vector3 p6 = new(square.Point2.x, heightOffset + height + currentHeightInterval, square.Point2.y);
        Vector3 p7 = new(square.Point3.x, heightOffset + height + currentHeightInterval, square.Point3.y);
        Vector3 p8 = new(square.Point4.x, heightOffset + height + currentHeightInterval, square.Point4.y);

        square.Position = new Vector3(square.Center.x, heightOffset + height + currentHeightInterval, square.Center.y);

        int start;

        // 바닥
        start = vertices.Count;
        vertices.AddRange(new[] { p1, p2, p3, p4 });
        uvs.AddRange(UV());
        triangles.AddRange(new[] { start, start + 2, start + 1, start, start + 3, start + 2 });

        // 천장 (반대 방향)
        start = vertices.Count;
        vertices.AddRange(new[] { p5, p6, p7, p8 });
        uvs.AddRange(UV());
        triangles.AddRange(new[] { start, start + 1, start + 2, start, start + 2, start + 3 });

        // 앞면 (p4-p3-p7-p8)
        start = vertices.Count;
        vertices.AddRange(new[] { p4, p3, p7, p8 });
        uvs.AddRange(UV());
        triangles.AddRange(new[] { start, start + 2, start + 1, start, start + 3, start + 2 });

        // 뒷면 (p1-p2-p6-p5)
        start = vertices.Count;
        vertices.AddRange(new[] { p1, p2, p6, p5 });
        uvs.AddRange(UV());
        triangles.AddRange(new[] { start, start + 1, start + 2, start, start + 2, start + 3 });

        // 왼쪽 면 (p1-p4-p8-p5)
        start = vertices.Count;
        vertices.AddRange(new[] { p1, p4, p8, p5 });
        uvs.AddRange(UV());
        triangles.AddRange(new[] { start, start + 2, start + 1, start, start + 3, start + 2 });

        // 오른쪽 면 (p2-p3-p7-p6)
        start = vertices.Count;
        vertices.AddRange(new[] { p2, p3, p7, p6 });
        uvs.AddRange(UV());
        triangles.AddRange(new[] { start, start + 1, start + 2, start, start + 2, start + 3 });
    }

    /// <summary>
    /// Draws the given list of squares as stacked mesh layers and 
    /// returns the total height offset accumulated during the drawing process.
    /// </summary>
    /// <param name="squares">The list of squares to be drawn as mesh segments.</param>
    /// <returns>The total height offset (sum of all height intervals) applied while drawing.</returns>

    public float DrawMap(List<Square> squares)
    {
        mesh = new Mesh();

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        float currentHeightInterval = 0;
        foreach(var square in squares)
        {
            DrawSquare(square, vertices, triangles, uvs, currentHeightInterval);
            currentHeightInterval += heightInterval;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;

        var collider = gameObject.GetComponent<MeshCollider>();

        collider.sharedMesh = mesh;
        collider.convex = false;

        return currentHeightInterval;
    }


    private static Vector2[] UV()
    {
        return new[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };
    }
}
