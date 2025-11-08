using System;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private float height = 1f;
    [SerializeField] private float heightOffset = 0f;
    [SerializeField] private float heightInterval = 2f;
    [SerializeField] private Vector2 mapSize = new(50, 50);
    [SerializeField] private Vector3 defaultPosition = new(0, 0, 0);
    [SerializeField] private float minRoomSize = 1f;
    [SerializeField] private float gap = 0.9f;
    [SerializeField] private int splitDepth = 5;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Map mapPrefab;

    private List<Map> mapList = new();

    public static Square CurrentSquare { get; set; }

    private float currentHeightOffset = 0;
    private Square currentHighestSquare;

    private void Awake()
    {
        Square initSquare = new(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
        initSquare.Position = startPosition.position;
        CurrentSquare = initSquare;

        currentHighestSquare = initSquare;

        mapList.Add(GenerateMap());
        mapList.Add(GenerateMap());
        mapList.Add(GenerateMap());
    }


    private Map GenerateMap()
    {
        Map map = Instantiate(mapPrefab, transform);
        map.transform.position = new Vector3(0, currentHeightOffset, 0) + defaultPosition;
        map.Init(0, height, heightInterval);

        BSPGenerator bspGenerator = new();
        bspGenerator.GenerateBSP(mapSize, splitDepth, minRoomSize, gap);
        List<Square> squares = bspGenerator.GetAreas();
        squares.ForEach(t => t.Position += Vector3.up * currentHeightOffset);

        List<Edge> edges = TriangulationGenerator.Generate(squares);
        
        NodeGenerator.GenerateNode(edges);
        
        NodeGenerator.GenerateEulerianPath(squares[0]);

        List<Square> eulerianSquares = new();
        Square square = squares[0];
        currentHighestSquare.TargetNode = new Square.Node(currentHighestSquare, square);
        while(square.TargetNode != null)
        {
            eulerianSquares.Add(square);
            square = square.TargetNode.Square;
        }

        currentHighestSquare = square;
        currentHeightOffset += map.DrawMap(eulerianSquares);

        return map;
    }
}