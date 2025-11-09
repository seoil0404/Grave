using UnityEngine;

public class Grass : MonoBehaviour
{
    private Mesh grassMesh;
    private Material grassMaterial;
    private ComputeShader grassCompute;

    private Vector2 areaSize;
    private int instanceCount;
    private float randomStrength;
    private Vector3 position;
    private Vector2 scaleRange;

    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;

    private readonly uint[] args = new uint[5];
    private int kernelID;

    private bool isInitialized = false;

    public void DrawGrass(
        Square square, 
        float instanceCountScale,
        float heightOffset,
        float randomStrength, 
        Vector2 scaleRange,
        Mesh grassMesh, 
        Material grassMaterial, 
        ComputeShader grassCompute)
    {
        this.areaSize = new Vector2(square.Point2.x - square.Point1.x, square.Point2.y - square.Point3.y);
        this.position = square.Position + Vector3.up * heightOffset;
        this.instanceCount = (int)((square.Point2.x - square.Point1.x) * (square.Point2.y - square.Point3.y) * instanceCountScale);
        this.randomStrength = randomStrength;
        this.scaleRange = scaleRange;

        this.grassMesh = grassMesh;
        this.grassMaterial = grassMaterial;
        this.grassCompute = grassCompute;

        isInitialized = true;

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
        grassCompute.SetFloats("_ScaleRange", scaleRange.x, scaleRange.y);
        grassCompute.SetFloat("_RandomStrength", randomStrength);
        grassCompute.SetFloat("_Height", position.y);

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
        if (isInitialized)
        {
            Graphics.DrawMeshInstancedIndirect(grassMesh, 0, grassMaterial,
                new Bounds(position, new Vector3(areaSize.x, 50, areaSize.y)), argsBuffer);
        }
    }

    private void OnDestroy()
    {
        positionBuffer?.Release();
        argsBuffer?.Release();
    }
}
