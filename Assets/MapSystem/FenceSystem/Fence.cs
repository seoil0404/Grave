using UnityEngine;

public class Fence : MonoBehaviour
{
    private float fenceScale;
    private Square bound;

    public void DrawFence(Square bound, MeshFilter fencePrefab, float fenceScale)
    {
        this.bound = bound;
        this.fenceScale = fenceScale;

        PlaceFenceAlongEdge(bound.Point1, bound.Point2, fencePrefab);
        PlaceFenceAlongEdge(bound.Point2, bound.Point3, fencePrefab);
        PlaceFenceAlongEdge(bound.Point3, bound.Point4, fencePrefab);
        PlaceFenceAlongEdge(bound.Point4, bound.Point1, fencePrefab);
    }


    private void PlaceFenceAlongEdge(Vector2 a, Vector2 b, MeshFilter prefab)
    {
        Vector3 p1 = new Vector3(a.x, bound.Position.y, a.y);
        Vector3 p2 = new Vector3(b.x, bound.Position.y, b.y);

        Vector3 dir = p2 - p1;
        float edgeLength = dir.magnitude;
        Vector3 dirN = dir.normalized;

        float fenceWidth = prefab.sharedMesh.bounds.size.x * fenceScale;

        int count = Mathf.CeilToInt(edgeLength / (fenceWidth));

        float correctedScale = edgeLength / (count * fenceWidth);

        // Fence¿« √÷¡æ ∆¯
        float finalFenceWidth = fenceWidth * correctedScale;

        for (int index = 0; index < count; index++)
        {
            Vector3 pos = p1 + dirN * (index * finalFenceWidth + finalFenceWidth / 2);

            Quaternion rot = Quaternion.LookRotation(dirN);

            MeshFilter fence = Instantiate(prefab, pos, rot, transform);

            Vector3 scale = fence.transform.localScale;

            scale.x *= correctedScale;
            scale *= fenceScale;

            fence.transform.localScale = scale;

            fence.transform.eulerAngles = new Vector3(
                90,
                90 + fence.transform.eulerAngles.y,
                fence.transform.eulerAngles.z
            );

            fence.transform.position += Vector3.up * 1.5f;
        }
    }
}
