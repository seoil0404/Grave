
using System.Collections.Generic;
using UnityEngine;

public static class SpiralPointDistributor
{
    static readonly float GoldenAngle = Mathf.PI * (3f - Mathf.Sqrt(5f));

    public static List<Point> GeneratePoints(Square square, int count, float excludeRadius = 0f)
    {
        List<Point> points = new List<Point>(count);

        Vector2 center = square.Center;
        float maxR = Mathf.Min(square.Width, square.Height) * 0.5f;

        for (int index = 0; index < count; index++)
        {
            float t = (index + 1f) / (count + 1f);
            float r = Mathf.Sqrt(t) * maxR;
            float angle = index * GoldenAngle;

            Vector2 pos = new Vector2(
                center.x + Mathf.Cos(angle) * r,
                center.y + Mathf.Sin(angle) * r
            );

            if (Vector2.Distance(pos, center) < excludeRadius)
            {
                float extra = excludeRadius - Vector2.Distance(pos, center);
                pos += (pos - center).normalized * extra;
            }

            pos.x = Mathf.Clamp(pos.x, square.Point1.x, square.Point2.x);
            pos.y = Mathf.Clamp(pos.y, square.Point3.y, square.Point2.y);

            points.Add(new Point { Position = new Vector2(pos.x, pos.y) });
        }

        return points;
    }
}