using UnityEngine;

[ExecuteAlways]
public class ParabolicMoverGizmo : MonoBehaviour
{
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] float height = 3f;
    [SerializeField] float duration = 1f;
    [SerializeField] int resolution = 30; // °î¼± Á¤¹Ðµµ

    float time;

    void Update()
    {
        if (Application.isPlaying)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            transform.position = GetParabolaPoint(startPoint.position, endPoint.position, height, t);
        }
    }

    Vector3 GetParabolaPoint(Vector3 start, Vector3 end, float height, float t)
    {
        Vector3 mid = Vector3.Lerp(start, end, t);
        float parabola = 4 * height * t * (1 - t);
        mid.y += parabola;
        return mid;
    }

    void OnDrawGizmos()
    {
        if (startPoint == null || endPoint == null) return;

        Gizmos.color = Color.yellow;
        Vector3 prevPos = startPoint.position;

        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 pos = GetParabolaPoint(startPoint.position, endPoint.position, height, t);
            Gizmos.DrawLine(prevPos, pos);
            prevPos = pos;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(startPoint.position, 0.05f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(endPoint.position, 0.05f);
    }
}
