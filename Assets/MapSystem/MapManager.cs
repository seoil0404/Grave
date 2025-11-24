using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
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
    [SerializeField] private float maxRoomScale;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Map mapPrefab;

    private List<Map> mapList = new();
    private static Square currentSquare;

    private float currentHeightOffset = 0;
    private Square currentHighestSquare;

    public static Square CurrentSquare
    {
        get { return currentSquare; }
        set
        {
            currentSquare = value;
        }
    }

    public static MapManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        Square initSquare = new(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
        initSquare.Position = startPosition.position;
        CurrentSquare = initSquare;

        currentHighestSquare = initSquare;
    }

    public void AddMap()
    {
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
        squares.ForEach(t => t.Position = Vector3.up * (currentHeightOffset));
        List<Square> filteredSqaures = squares.Where(square => square.Area <= maxRoomScale).ToList();
        if(filteredSqaures.Count <= 0)
        {
            filteredSqaures.Add(squares.OrderBy(square => square.Area).First());
        }
        else squares = filteredSqaures;
        

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

        eulerianSquares.Add(square);
        currentHighestSquare = square;
        currentHighestSquare.TargetNode = null;

        currentHeightOffset += map.DrawMap(eulerianSquares);

        return map;
    }
}