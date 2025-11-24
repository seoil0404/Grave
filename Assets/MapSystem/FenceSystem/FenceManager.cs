using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FenceManager : MonoBehaviour
{
    public static FenceManager Instance { get; private set; }

    [SerializeField] private MeshFilter fencePrefab;

    private Queue<Fence> fences = new();

    private void Awake()
    {
        Instance = this;
    }

    public void DrawFence(Square bound)
    {
        if (fences.Count > 0)
            Destroy(fences.Dequeue().gameObject, 5f);

        GameObject fenceGameObject = new GameObject("Fence");
        fenceGameObject.transform.parent = transform;

        Fence fence = fenceGameObject.AddComponent<Fence>();
        fence.DrawFence(bound, fencePrefab, 4f);
        fences.Enqueue(fence);
    }
}