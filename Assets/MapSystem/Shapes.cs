using UnityEngine;

public struct Triangle
{
    public Square A { get; set; }
    public Square B { get; set; }
    public Square C { get; set; }

    public bool IsCircumCircleContains(Square square)
    {
        Vector2 a = A.Center;
        Vector2 b = B.Center;
        Vector2 c = C.Center;
        Vector2 p = square.Center;

        float d = 2 * (a.x * (b.y - c.y) +
                       b.x * (c.y - a.y) +
                       c.x * (a.y - b.y));

        if (Mathf.Abs(d) < 1e-12f)
            return false;

        float ux = ((a.sqrMagnitude) * (b.y - c.y) +
                    (b.sqrMagnitude) * (c.y - a.y) +
                    (c.sqrMagnitude) * (a.y - b.y)) / d;

        float uy = ((a.sqrMagnitude) * (c.x - b.x) +
                    (b.sqrMagnitude) * (a.x - c.x) +
                    (c.sqrMagnitude) * (b.x - a.x)) / d;

        Vector2 circum = new(ux, uy);
        float radius = Vector2.Distance(a, circum);

        return Vector2.Distance(p, circum) <= radius;
    }

    public bool HasEdge(Edge edge)
    {
        return (A == edge.Point1 || B == edge.Point1 || C == edge.Point1) &&
               (A == edge.Point2 || B == edge.Point2 || C == edge.Point2);
    }

    public bool IsEqual(Triangle other)
    {
        return (A == other.A && B == other.B && C == other.C) ||
                (A == other.B && B == other.C && C == other.A) ||
                (A == other.C && B == other.A && C == other.B);
    }
}

public class Square
{
    public PriorityQueue<Node, float> Nodes { get; set; } = new(t => t.Distance, PriorityQueue<Node, float>.SortOrder.Descending);
    public Node TargetNode { get; set; } = null;
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
        private Square currentSquare;
        public Node() { }
        public Node(Square currentSquare, Square opponentSquare)
        {
            Square = opponentSquare;
            this.currentSquare = currentSquare;
        }

        public Square Square { get; private set; }
        public float Distance => currentSquare.Distance(Square);
    }
}

public struct Edge
{
    public Square Point1 { get; set; }
    public Square Point2 { get; set; }

    public bool IsEqual(Edge other)
    {
        return (Point1 == other.Point1 && Point2 == other.Point2) ||
               (Point1 == other.Point2 && Point2 == other.Point1);
    }

    public override int GetHashCode()
    {
        int h1 = Point1.GetHashCode() ^ Point2.GetHashCode();
        return h1;
    }
}