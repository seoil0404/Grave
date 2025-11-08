using UnityEngine;

public class GrassGPUManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Mesh grassMesh;
    [SerializeField] private Material grassMaterial;
    [SerializeField] private ComputeShader grassCompute;

    [Header("Settings")]
    [SerializeField] private Vector2 areaSize = new(100, 100);
    [SerializeField] private int instanceCount = 100000;
    [SerializeField] private float noiseScale = 0.08f;
    [SerializeField] private float noiseThreshold = 0.4f;
    [SerializeField] private Vector2 scaleRange = new(0.8f, 1.3f);

    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;

    private readonly uint[] args = new uint[5]; // 인스턴싱 인자용 (DrawMeshInstancedIndirect)
    private int kernelID;

    private void Start()
    {
        InitBuffers();
        DispatchCompute();
    }

    private void InitBuffers()
    {
        kernelID = grassCompute.FindKernel("CSMain");

        // Position buffer (Vector4: xyz=pos, w=scale)
        positionBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 4);
        grassCompute.SetBuffer(kernelID, "_PositionBuffer", positionBuffer);

        // Indirect args buffer
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        uint indexCount = (grassMesh != null) ? grassMesh.GetIndexCount(0) : 0;
        args[0] = indexCount;
        args[1] = (uint)instanceCount;
        args[2] = 0;
        args[3] = 0;
        args[4] = 0;
        argsBuffer.SetData(args);

        // 매개변수 전달
        grassCompute.SetInt("_InstanceCount", instanceCount);
        grassCompute.SetFloats("_AreaSize", areaSize.x, areaSize.y);
        grassCompute.SetFloat("_NoiseScale", noiseScale);
        grassCompute.SetFloat("_NoiseThreshold", noiseThreshold);
        grassCompute.SetFloats("_ScaleRange", scaleRange.x, scaleRange.y);

        grassMaterial.SetBuffer("_PositionBuffer", positionBuffer);
    }

    private void DispatchCompute()
    {
        int threadGroupX = Mathf.CeilToInt(instanceCount / 64f);
        grassCompute.Dispatch(kernelID, threadGroupX, 1, 1);
    }

    private void Update()
    {
        // Draw call
        if (grassMesh != null && grassMaterial != null)
        {
            Graphics.DrawMeshInstancedIndirect(grassMesh, 0, grassMaterial,
                new Bounds(Vector3.zero, new Vector3(areaSize.x, 50, areaSize.y)), argsBuffer);
        }
    }

    private void OnDestroy()
    {
        positionBuffer?.Release();
        argsBuffer?.Release();
    }
}
