using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;

public class PolygonUtil : MonoBehaviour {

    public static Vector2[] toVector2(List<IntPoint> inPoints) {
        Vector2[] outPoints = new Vector2[inPoints.Count];
        for (int i = 0; i < inPoints.Count; i++) {
            outPoints[i] = new Vector2(inPoints[i].X, inPoints[i].Y);
        }
        return outPoints;
    }

    public static Vector2[] toVector2(List<Vector2> inPoints) {
        Vector2[] outPoints = new Vector2[inPoints.Count];
        for (int i = 0; i < inPoints.Count; i++) {
            outPoints[i] = new Vector2(inPoints[i].x, inPoints[i].y);
        }
        return outPoints;
    }

    public static Vector3[] toVector3(Vector2[] inPoints) {
        Vector3[] outPoints = new Vector3[inPoints.Length + 1];
        for (int i = 0; i < inPoints.Length; i++) {
            outPoints[i] = new Vector3(inPoints[i].x, inPoints[i].y);
        }
        return outPoints;
    }

    public static List<IntPoint> fromVector2(Vector2[] inPoints) {
        List<IntPoint> outPoints = new List<IntPoint>();
        foreach (Vector2 point in inPoints) {
            outPoints.Add(new IntPoint(point.x, point.y));
        }
        return outPoints;
    }

    public static float getDifferenceArea(Vector2[] polygon1, Vector2[] polygon2, float resolutionScalar) {
        //scale up polygon2
        for (int i = 0; i < polygon1.Length; i++) {
            polygon1[i] *= resolutionScalar;
        }

        for (int i = 0; i < polygon2.Length; i++) {
            polygon2[i] *= resolutionScalar;
        }


        Clipper clipper = new Clipper();
        List<List<IntPoint>> solution = new List<List<IntPoint>>();

        List<IntPoint> polygon1Points = fromVector2(polygon1);
        List<IntPoint> polygon2Points = fromVector2(polygon2);
        polygon1Points.Reverse();

        clipper.AddPath(polygon1Points, PolyType.ptSubject, true);
        clipper.AddPath(polygon2Points, PolyType.ptClip, true);
        clipper.Execute(ClipType.ctXor, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);

        List<Vector2[]> pathsOut = new List<Vector2[]>();
        foreach (List<IntPoint> path in solution) {
            pathsOut.Add(toVector2(path));
        }

        double area = 0;
        foreach (Vector2[] path in pathsOut) {
             area += Clipper.Area(fromVector2(path));
        }

        area /= (resolutionScalar * resolutionScalar);
        return (float) area;
    }


    public static Vector2[] getIntersection(Vector2[] polygon1, Vector2[] polygon2, float resolutionScalar) {

        //scale up polygon2
        for (int i = 0; i < polygon1.Length; i++) {
            polygon1[i] *= resolutionScalar;
        }

        for (int i = 0; i < polygon2.Length; i++) {
            polygon2[i] *= resolutionScalar;
        }


        Clipper clipper = new Clipper();
        List<List<IntPoint>> solution = new List<List<IntPoint>>();

        clipper.AddPath(fromVector2(polygon1), PolyType.ptSubject, true);
        clipper.AddPath(fromVector2(polygon2), PolyType.ptClip, true);
        clipper.Execute(ClipType.ctIntersection, solution, PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd);

        List<Vector2[]> pathsOut = new List<Vector2[]>();
        foreach (List<IntPoint> path in solution) {
            pathsOut.Add(toVector2(path));
        }

        //scale down polygon
        foreach (Vector2[] path in pathsOut) {
            for (int i = 0; i < path.Length; i++) {
                path[i] /= resolutionScalar;
            }
        }

        if (pathsOut.Count > 0) {
            return pathsOut[0];
        } else {
            return new Vector2[0];
        }
    }

    //variance should be between like 2 and 6ish
    public static Vector2[] getNewPolygon(int numberOfVerts, float variance, float radius, int seed, bool inner) {

        System.Random rng = new System.Random(seed);
        float randomWidthScalar = 1.5f + (float)rng.NextDouble() * 0.5f;
        float varianceScalar = 2;
        Vector2[] vertices = new Vector2[numberOfVerts];

        Vector2 sampleCircleCenter = new Vector2(rng.Next(-1000, 1000), rng.Next(-1000, 1000));
        List<float> sampleHeights = new List<float>();
        

        float sliceAngle = (float)(360 / numberOfVerts);
        for (int i = 0; i < numberOfVerts; i++) {
            Vector2 sampleLocation = sampleCircleCenter + MathUtil.polarToCartesian(new Vector2(variance / 2, 360 - (sliceAngle * i)));
            sampleHeights.Add(Mathf.PerlinNoise(sampleLocation.x, sampleLocation.y));
        }

        for (int i = 0; i < numberOfVerts; i++) {
            if (inner) {
                vertices[i] = MathUtil.polarToCartesian(new Vector2(radius + (sampleHeights[i] * varianceScalar), 360 - (sliceAngle * i)));
            } else {
                vertices[i] = MathUtil.polarToCartesian(new Vector2(Random.Range(0.1f, 0.75f) + radius + (sampleHeights[i] * varianceScalar), 360 - (sliceAngle * i)));
            }
           
            vertices[i].x /= 1.5f;
            vertices[i].y /= 1.5f;
            vertices[i].x *= randomWidthScalar;
        }

        return vertices;
    }
}
