using System.Collections.Generic;
using UnityEngine;

public class GrassManager : MonoBehaviour
{
    public static GrassManager Instance { get; private set; }

    [Header("Queue Setting")]
    [SerializeField] private int maxGrassQueueCount;

    [Header("Draw Setting")]
    [SerializeField] private float instanceCountScale;
    [SerializeField] private float heightOffset;
    [SerializeField] private float randomStrength;
    [SerializeField] private Vector2 scaleRange;
    [SerializeField] private Mesh grassMesh;
    [SerializeField] private Material grassMaterial;
    [SerializeField] private ComputeShader grassCompute;

    private Queue<Grass> grassQueue = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else return;
    }

    public void DrawGrass(Square square)
    {
        GameObject newObject = new GameObject("Grass");
        newObject.transform.parent = transform;

        var newGrass = newObject.AddComponent<Grass>();
        newGrass.DrawGrass(square, instanceCountScale, heightOffset, randomStrength, scaleRange, grassMesh, Instantiate(grassMaterial), grassCompute);

        if(grassQueue.Count >= maxGrassQueueCount) Destroy(grassQueue.Dequeue().gameObject);
        grassQueue.Enqueue(newGrass);
    }
}
