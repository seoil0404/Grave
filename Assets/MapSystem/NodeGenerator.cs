using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public static class NodeGenerator
{
    public static void GenerateNode(List<Edge> edges)
    {
        foreach (Edge edge in edges)
        {
            edge.Point1.Nodes.Enqueue(new(edge.Point1, edge.Point2));
            edge.Point2.Nodes.Enqueue(new(edge.Point2, edge.Point1));
        }
    }

    public static void GenerateEulerianPath(Square startVertex)
    {
        Square currentVertex = startVertex;
        while(currentVertex.Nodes.Count > 0)
        {
            if (currentVertex.Nodes.Peek().Square.IsVisit == false)
            {
                currentVertex.IsVisit = true;
                currentVertex.TargetNode = currentVertex.Nodes.Peek();
                currentVertex = currentVertex.Nodes.Peek().Square;
            }
            else currentVertex.Nodes.Dequeue();
        }
    }

    public static void GenerateNode(List<Square> vertices)
    {
        GenerateNodeByBrute(vertices);
    }

    private static void GenerateNodeByBrute(List<Square> squares)
    {
        for(int firstIndex = 0; firstIndex < squares.Count; firstIndex++)
        {
            for(int secondIndex = 0; secondIndex < squares.Count; secondIndex++)
            {
                if (secondIndex == firstIndex) continue;

                squares[firstIndex].Nodes.Enqueue(
                        new Square.Node(
                            squares[firstIndex],
                            squares[secondIndex]
                        )
                    );
            }
        }

        Square square = squares[0];
        int count = 0;
        while(true)
        {
            square.IsVisit = true;
            bool isNodable = false;
            while(square.Nodes.Count >= 1)
            {
                if (square.Nodes.Peek().Square.IsVisit)
                {
                    square.Nodes.Dequeue();
                    continue;
                }
                else
                {
                    isNodable = true;
                    square = square.Nodes.Peek().Square;

                    break;
                }
            }

            if(!isNodable) break;

            count++;

            if (count >= 1000)
            {
                throw new System.Exception("Wtf");
            }
        }
    }

    private static void GenerateNodeByTriangulation(List<Square> vertices)
    {

    }
}