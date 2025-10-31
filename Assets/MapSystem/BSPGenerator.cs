using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BSPGenerator
{
    public class Node
    {
        public Square area;
        public Node left;
        public Node right;
        public bool IsLeaf => left == null && right == null;
        public Node(Square square) { area = square; }
    }

    public Node root;
    private float minSize;
    private float gap;
    private float minSplitRatio;
    private float maxSplitRatio;

    public BSPGenerator(Square rootArea, float minSize = 5f, float gap = 0.9f, float minSplitRatio = 0.3f, float maxSplitRatio = 0.7f)
    {
        root = new Node(rootArea);
        this.minSize = minSize;
        this.gap = gap;
        this.maxSplitRatio = maxSplitRatio;
        this.minSplitRatio = minSplitRatio;
    }

    public void Split(Node node, int depth)
    {
        if (node == null || depth <= 0)
            return;

        float width = Vector2.Distance(node.area.Point1, node.area.Point2);
        float height = Vector2.Distance(node.area.Point1, node.area.Point4);

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

        Vector2 p1 = node.area.Point1; // 왼쪽 위
        Vector2 p2 = node.area.Point2; // 오른쪽 위
        Vector2 p3 = node.area.Point3; // 오른쪽 아래
        Vector2 p4 = node.area.Point4; // 왼쪽 아래

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

            node.left = new Node(bottom);
            node.right = new Node(top);
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

            node.left = new Node(left);
            node.right = new Node(right);
        }

        Split(node.left, depth - 1);
        Split(node.right, depth - 1);
    }

    public List<Square> GetLeafAreas(Node node)
    {
        List<Square> result = new();
        if (node == null) return result;

        if (node.IsLeaf)
            result.Add(node.area);
        else
        {
            result.AddRange(GetLeafAreas(node.left));
            result.AddRange(GetLeafAreas(node.right));
        }

        return result;
    }
}
