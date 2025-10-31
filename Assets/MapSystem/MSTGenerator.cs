using System.Collections.Generic;
using System.Linq;

public class MSTGenerator
{
    public static void GenerateNode(List<Square> vertices)
    {
        GenerateNodeByBrute(vertices);
    }

    private static void GenerateNodeByBrute(List<Square> vertices)
    {
        for(int firstIndex = 0; firstIndex < vertices.Count; firstIndex++)
        {
            for(int secondIndex = 0; secondIndex < vertices.Count; secondIndex++)
            {
                if (secondIndex == firstIndex) continue;

                vertices[firstIndex].NodeList.Add(
                        new Square.Node(
                            vertices[secondIndex],
                            vertices[firstIndex].Distance(vertices[secondIndex])
                        )
                    );
            }
        }

        foreach(var vertex in vertices)
        {
            vertex.NodeList = vertex.NodeList.OrderBy(n => n.Distance).ToList();
        }

        Square square = vertices[0];
        while(true)
        {
            square.IsVisit = true;
            bool isNodable = false;
            for(int index = 0; index < square.NodeList.Count; index++)
            {
                if (square.NodeList[index].Square.IsVisit) continue;

                Square.Node node = square.NodeList[index];

                square.NodeList = new List<Square.Node> { node };

                square = square.NodeList[0].Square;
                isNodable = true;

                break;
            }

            if(!isNodable) break;
        }
    }

    private static void GenerateNodeByTriangulation(List<Square> vertices)
    {

    }
}