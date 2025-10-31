using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MapManager : MonoBehaviour
{
    [SerializeField] private float height = 1f;
    [SerializeField] private float heightOffset = 0f;
    [SerializeField] private float heightInterval = 2f;
    [SerializeField] private Vector2 mapSize = new(50, 50);
    [SerializeField] private float minRoomSize = 1f;
    [SerializeField] private float gap = 0.9f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private List<Square> squares = new();
    private Mesh mesh;

    public static Square CurrentSquare { get; set; }

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        GenerateWithBSP();
        MSTGenerator.GenerateNode(squares);
        DrawMesh();

        for(int index = 0; index < squares.Count; index++)
        {
            Debug.DrawLine(squares[index].Position, squares[index].NodeList[0].Square.Position, Color.red, 10000);
        }

        CurrentSquare = squares[0];
    }

    private void GenerateWithBSP()
    {
        BSPGenerator bsp = new BSPGenerator(
            new Square(
                new Vector2(-mapSize.x / 2, mapSize.y/2),             // Point1: 왼쪽 위
                new Vector2(mapSize.x/2, mapSize.y/2),     // Point2: 오른쪽 위
                new Vector2(mapSize.x/2, -mapSize.y / 2),             // Point3: 오른쪽 아래
                new Vector2(-mapSize.x / 2, -mapSize.y / 2)                      // Point4: 왼쪽 아래
            ),
            minSize: minRoomSize,
            gap: gap
        );

        bsp.Split(bsp.root, 6);
        squares = bsp.GetLeafAreas(bsp.root);

        for (int i = 0; i < squares.Count; i++)
        {
            squares[i] = ApplyGap(squares[i]);
        }

    }

    private Square ApplyGap(Square sq)
    {
        // 중심은 대각선 기준 (왼쪽 위, 오른쪽 아래)
        Vector2 center = (sq.Point1 + sq.Point3) / 2f;
        Vector3 position = sq.Position;

        float width = sq.Point2.x - sq.Point1.x;
        width *= gap;
        float halfWidth = width / 2f;

        float height = sq.Point1.y - sq.Point4.y;
        height *= gap;
        float halfHeight = height / 2f;

        Vector2 p1 = center + new Vector2(-halfWidth, halfHeight);  // 왼쪽 위
        Vector2 p2 = center + new Vector2(halfWidth, halfHeight);   // 오른쪽 위
        Vector2 p3 = center + new Vector2(halfWidth, -halfHeight);  // 오른쪽 아래
        Vector2 p4 = center + new Vector2(-halfWidth, -halfHeight); // 왼쪽 아래

        sq = new Square(p1, p2, p3, p4);
        sq.Position = position;

        return sq;
    }

    private void DrawMesh()
    {
        mesh = new Mesh();

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        float currentHeightInterval = 0;
        Square square = squares[0];
        while(square.NodeList.Count == 1)
        {
            // 바닥(Top-Down 좌표)
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

            currentHeightInterval += heightInterval;

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

            square = square.NodeList[0].Square;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;

        var collider = gameObject.GetComponent<MeshCollider>();
        
        collider.sharedMesh = mesh;
        collider.convex = false;

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

public class Square
{
    public List<Node> NodeList { get; set; } = new();

    public bool IsVisit { get; set; } = false;

    public Vector2 Point1 { get; } 
    public Vector2 Point2 { get; } 
    public Vector2 Point3 { get; } 
    public Vector2 Point4 { get; } 

    public Vector2 Center => (Point1 + Point3) / 2;
    public Vector3 Position { get; set; }
    public float Width => Point2.x - Point1.x;
    public float Height => Point2.y - Point3.y;
    public float Distance(Square other)
    {
        float distance = Vector2.Distance(
                            Center,
                            other.Center
                            );

        return distance;
    }

    public Square(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        Point1 = p1;
        Point2 = p2;
        Point3 = p3;
        Point4 = p4;
    }

    public class Node
    {
        public Node() { }
        public Node(Square opponentSquare, float distance)
        {
            Square = opponentSquare;
            Distance = distance;
        }
        public Square Square { get; private set; }
        public float Distance { get; private set; }
    }
}
