using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TriangulationGenerator
{
    public static List<Edge> Generate(List<Square> vertices)
    {
        if (vertices == null || vertices.Count < 3)
            throw new ArgumentException("At least 3 squares are required");

        // Super Triangle 생성
        float minX = vertices.Min(v => v.Center.x);
        float minY = vertices.Min(v => v.Center.y);
        float maxX = vertices.Max(v => v.Center.x);
        float maxY = vertices.Max(v => v.Center.y);

        float dx = maxX - minX;
        float dy = maxY - minY;
        float deltaMax = Mathf.Max(dx, dy);
        float midx = (minX + maxX) / 2f;
        float midy = (minY + maxY) / 2f;

        // Super Square (임시)
        Square s1 = new(new(midx - 2 * deltaMax, midy - deltaMax),
                        new(midx - 2 * deltaMax, midy - deltaMax),
                        new(midx - 2 * deltaMax, midy - deltaMax),
                        new(midx - 2 * deltaMax, midy - deltaMax));

        Square s2 = new(new(midx, midy + 2 * deltaMax),
                        new(midx, midy + 2 * deltaMax),
                        new(midx, midy + 2 * deltaMax),
                        new(midx, midy + 2 * deltaMax));

        Square s3 = new(new(midx + 2 * deltaMax, midy - deltaMax),
                        new(midx + 2 * deltaMax, midy - deltaMax),
                        new(midx + 2 * deltaMax, midy - deltaMax),
                        new(midx + 2 * deltaMax, midy - deltaMax));

        List<Triangle> triangles = new()
        {
            new Triangle { A = s1, B = s2, C = s3 }
        };

        // Bowyer Watson 알고리즘
        foreach (var square in vertices)
        {
            List<Triangle> badTriangles = new();

            foreach (var tri in triangles)
            {
                if (tri.IsCircumCircleContains(square))
                    badTriangles.Add(tri);
            }

            // Polygon 경계 계산
            List<Edge> polygon = new();
            foreach (var tri in badTriangles)
            {
                var edges = new Edge[]
                {
                    new Edge { Point1 = tri.A, Point2 = tri.B },
                    new Edge { Point1 = tri.B, Point2 = tri.C },
                    new Edge { Point1 = tri.C, Point2 = tri.A }
                };

                foreach (var edge in edges)
                {
                    bool shared = false;
                    foreach (var other in badTriangles)
                    {
                        if (tri.IsEqual(other)) continue;
                        if (other.HasEdge(edge))
                        {
                            shared = true;
                            break;
                        }
                    }
                    if (!shared)
                        polygon.Add(edge);
                }
            }

            // badTriangles 제거
            foreach (var tri in badTriangles)
                triangles.Remove(tri);

            // 새 삼각형 추가
            foreach (var edge in polygon)
            {
                Triangle newTri = new()
                {
                    A = edge.Point1,
                    B = edge.Point2,
                    C = square
                };
                triangles.Add(newTri);
            }
        }

        // Super Triangle과 관련된 삼각형 제거
        triangles.RemoveAll(t =>
            t.A == s1 || t.A == s2 || t.A == s3 ||
            t.B == s1 || t.B == s2 || t.B == s3 ||
            t.C == s1 || t.C == s2 || t.C == s3);

        // --- 고유한 Edge만 반환 ---
        HashSet<Edge> edgeData = new();

        void AddEdge(Square a, Square b)
        {
            Edge edge = NormalizeEdge(a, b);
            edgeData.Add(edge);
        }

        foreach (var tri in triangles)
        {
            AddEdge(tri.A, tri.B);
            AddEdge(tri.B, tri.C);
            AddEdge(tri.C, tri.A);
        }

        List<Edge> uniqueEdges = new();
        foreach (var kvp in edgeData)
            uniqueEdges.Add(kvp);

        return uniqueEdges;
    }

    // 방향성 제거용 Edge 정규화
    private static Edge NormalizeEdge(Square a, Square b)
    {
        var ac = a.Center;
        var bc = b.Center;

        if (ac.x < bc.x) return new Edge { Point1 = a, Point2 = b };
        if (ac.x > bc.x) return new Edge { Point1 = b, Point2 = a };
        if (ac.y < bc.y) return new Edge { Point1 = a, Point2 = b };
        return new Edge { Point1 = b, Point2 = a };
    }
}
