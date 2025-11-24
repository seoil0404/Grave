using System;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiskManager
{
    public static List<Point> Sample(Square square, float minDist = 3, float excludeRadius = 3)
    {
        Rect area = new Rect(
            square.Point1.x,
            square.Point4.y,
            square.Width,
            square.Height
        );

        Vector2 center = square.Center;
        float cellSize = minDist / Mathf.Sqrt(2f);

        int gridW = Mathf.CeilToInt(area.width / cellSize);
        int gridH = Mathf.CeilToInt(area.height / cellSize);

        Point?[][] grid = new Point?[gridW][];
        for (int i = 0; i < gridW; i++)
            grid[i] = new Point?[gridH];

        List<Point> points = new List<Point>();
        List<Point> active = new List<Point>();

        Point initial = RandomPoint(area, center, excludeRadius);
        points.Add(initial);
        active.Add(initial);

        int initGX = (int)((initial.Position.x - area.xMin) / cellSize);
        int initGY = (int)((initial.Position.y - area.yMin) / cellSize);
        grid[initGX][initGY] = initial;

        int k = 30;

        while (active.Count > 0)
        {
            int idx = UnityEngine.Random.Range(0, active.Count);
            Point basePoint = active[idx];
            bool found = false;

            for (int attempt = 0; attempt < k; attempt++)
            {
                Point candidate = GenerateCandidate(basePoint, minDist);

                if (!InsideRect(candidate.Position, area))
                    continue;

                if (Vector2.Distance(candidate.Position, center) < excludeRadius)
                    continue;

                int gx = (int)((candidate.Position.x - area.xMin) / cellSize);
                int gy = (int)((candidate.Position.y - area.yMin) / cellSize);

                if (!IsValid(grid, gridW, gridH, candidate, gx, gy, minDist))
                    continue;

                grid[gx][gy] = candidate;
                points.Add(candidate);
                active.Add(candidate);
                found = true;
                break;
            }

            if (!found)
                active.RemoveAt(idx);
        }

        return points;
    }

    static Point RandomPoint(Rect area, Vector2 center, float excludeRadius)
    {
        while (true)
        {
            float x = UnityEngine.Random.Range(area.xMin, area.xMax);
            float y = UnityEngine.Random.Range(area.yMin, area.yMax);

            Vector2 p = new Vector2(x, y);
            if (Vector2.Distance(p, center) >= excludeRadius)
                return new Point { Position = p };
        }
    }

    static Point GenerateCandidate(Point origin, float minDist)
    {
        float radius = UnityEngine.Random.Range(minDist, minDist * 2f);
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

        Vector2 pos = new Vector2(
            origin.Position.x + Mathf.Cos(angle) * radius,
            origin.Position.y + Mathf.Sin(angle) * radius
        );

        return new Point { Position = pos };
    }

    static bool InsideRect(Vector2 p, Rect area)
    {
        return p.x >= area.xMin && p.x <= area.xMax &&
               p.y >= area.yMin && p.y <= area.yMax;
    }

    static bool IsValid(
        Point?[][] grid, int gw, int gh,
        Point candidate, int gx, int gy,
        float minDist)
    {
        int startX = Mathf.Max(gx - 2, 0);
        int endX = Mathf.Min(gx + 2, gw - 1);
        int startY = Mathf.Max(gy - 2, 0);
        int endY = Mathf.Min(gy + 2, gh - 1);

        float minDistSq = minDist * minDist;

        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                if (grid[x][y].HasValue)
                {
                    float d = (grid[x][y].Value.Position - candidate.Position).sqrMagnitude;
                    if (d < minDistSq)
                        return false;
                }
            }
        }

        return true;
    }
}
