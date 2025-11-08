using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BSPGenerator
{
    private class Node
    {
        public Square Area { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public bool IsLeaf => Left == null && Right == null;
        public Node(Square square) { Area = square; }
    }

    private Node root;
    private float minSize;
    private float gap;
    private float minSplitRatio;
    private float maxSplitRatio;

    public void GenerateBSP(
        Vector2 bspSize, 
        int splitDepth,
        float minSize = 5f, 
        float gap = 0.9f, 
        float minSplitRatio = 0.3f, 
        float maxSplitRatio = 0.7f)
    {
        root = new Node(new Square(
                    new Vector2(-bspSize.x / 2, bspSize.y / 2),
                    new Vector2(bspSize.x / 2, bspSize.y / 2),
                    new Vector2(bspSize.x / 2, -bspSize.y / 2),
                    new Vector2(-bspSize.x / 2, -bspSize.y / 2)
                ));

        this.minSize = minSize;
        this.gap = gap;
        this.maxSplitRatio = maxSplitRatio;
        this.minSplitRatio = minSplitRatio;

        Split(root, splitDepth);
    }

    private void Split(Node node, int depth)
    {
        if (node == null || depth <= 0)
            return;

        float width = Vector2.Distance(node.Area.Point1, node.Area.Point2);
        float height = Vector2.Distance(node.Area.Point1, node.Area.Point4);

        bool splitHorizontally = Random.value > 0.5f;

        if (width > height && width / height >= 1.25f)
            splitHorizontally = false;
        else if (height > width && height / width >= 1.25f)
            splitHorizontally = true;

        float size = splitHorizontally ? height : width;
        if (size <= minSize)
            return;

        float minSplit = size * minSplitRatio;
        float maxSplit = size * maxSplitRatio;
        float split = Random.Range(minSplit, maxSplit);

        Vector2 p1 = node.Area.Point1; // 왼쪽 위
        Vector2 p2 = node.Area.Point2; // 오른쪽 위
        Vector2 p3 = node.Area.Point3; // 오른쪽 아래
        Vector2 p4 = node.Area.Point4; // 왼쪽 아래

        if (splitHorizontally)
        {
            // 수평 분할 (위/아래로 나눔)
            float ySplit = Mathf.Lerp(p4.y, p1.y, split / height);

            // 아래쪽 사각형 (Bottom)
            Square bottom = new Square(
                new Vector2(p4.x, ySplit),  // 왼쪽 위
                new Vector2(p3.x, ySplit),  // 오른쪽 위
                p3,                         // 오른쪽 아래
                p4                          // 왼쪽 아래
            );

            // 위쪽 사각형 (Top)
            Square top = new Square(
                p1,                         // 왼쪽 위
                p2,                         // 오른쪽 위
                new Vector2(p2.x, ySplit),  // 오른쪽 아래
                new Vector2(p1.x, ySplit)   // 왼쪽 아래
            );

            node.Left = new Node(bottom);
            node.Right = new Node(top);
        }
        else
        {
            // 수직 분할 (좌/우로 나눔)
            float xSplit = Mathf.Lerp(p1.x, p2.x, split / width);

            // 왼쪽 사각형 (Left)
            Square left = new Square(
                p1,                         // 왼쪽 위
                new Vector2(xSplit, p1.y),  // 오른쪽 위
                new Vector2(xSplit, p4.y),  // 오른쪽 아래
                p4                          // 왼쪽 아래
            );

            // 오른쪽 사각형 (Right)
            Square right = new Square(
                new Vector2(xSplit, p2.y),  // 왼쪽 위
                p2,                         // 오른쪽 위
                p3,                         // 오른쪽 아래
                new Vector2(xSplit, p3.y)   // 왼쪽 아래
            );

            node.Left = new Node(left);
            node.Right = new Node(right);
        }

        node.Area = ApplyGap(node.Area);

        Split(node.Left, depth - 1);
        Split(node.Right, depth - 1);
    }
    private Square ApplyGap(Square sq)
    {
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
        sq.Position = new Vector3(sq.Center.x, position.y, sq.Center.y);

        return sq;
    }
    public List<Square> GetAreas()
    {
        List<Square> result = new();
        if (root == null) return result;

        if (root.IsLeaf)
        {
            root.Area = ApplyGap(root.Area);
            result.Add(root.Area);
        }
        else
        {
            result.AddRange(GetAreas(root.Left));
            result.AddRange(GetAreas(root.Right));
        }

        return result;
    }
    private List<Square> GetAreas(Node node)
    {
        List<Square> result = new();
        if (node == null) return result;

        if (node.IsLeaf)
        {
            node.Area = ApplyGap(node.Area);
            result.Add(node.Area);
        }
        else
        {
            result.AddRange(GetAreas(node.Left));
            result.AddRange(GetAreas(node.Right));
        }

        return result;
    }
}
