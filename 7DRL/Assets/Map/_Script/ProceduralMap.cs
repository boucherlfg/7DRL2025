using UnityEngine;
using System.Collections.Generic;

public class ProceduralMap : MonoBehaviour
{
    public int numberOfPoints = 10;
    public float mapSize = 10f;
    public GameObject pointPrefab;
    public GameObject linePrefab;

    private void Start()
    {
        GenerateMap();
    }

    Vector3[] GeneratePoints()
    {
        Vector3[] points = new Vector3[numberOfPoints];
        float halfMapSize = mapSize / 2;
        float minDistance = 1.0f; // Distance minimale entre les points

        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector3 newPoint;
            bool tooClose;
            do
            {
                newPoint = new Vector3(Random.Range(-halfMapSize, halfMapSize), Random.Range(-halfMapSize, halfMapSize), 0);
                tooClose = false;
                for (int j = 0; j < i; j++)
                {
                    if (Vector3.Distance(newPoint, points[j]) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }
            } while (tooClose);

            points[i] = newPoint;
        }
        return points;
    }

    void GenerateMap()
    {
        Vector3[] points = GeneratePoints();

        // Calculer la position moyenne des points
        Vector3 centroid = Vector3.zero;
        foreach (Vector3 point in points)
        {
            centroid += point;
        }
        centroid /= points.Length;

        // Déplacer les points pour centrer la carte autour de l'origine
        for (int i = 0; i < points.Length; i++)
        {
            points[i] -= centroid;
        }

        // Instancier les points
        for (int i = 0; i < points.Length; i++)
        {
            GameObject point = Instantiate(pointPrefab, points[i], Quaternion.identity);
            point.transform.parent = transform;
        }

        List<int>[] connections = new List<int>[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            connections[i] = new List<int>();
        }

        // Utiliser l'algorithme de Kruskal pour générer un MST
        List<(int, int, float)> edges = new List<(int, int, float)>();
        for (int i = 0; i < points.Length; i++)
        {
            for (int j = i + 1; j < points.Length; j++)
            {
                float distance = Vector3.Distance(points[i], points[j]);
                edges.Add((i, j, distance));
            }
        }
        edges.Sort((a, b) => a.Item3.CompareTo(b.Item3));

        int[] parent = new int[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            parent[i] = i;
        }

        int Find(int x)
        {
            if (parent[x] == x)
                return x;
            return parent[x] = Find(parent[x]);
        }

        void Union(int x, int y)
        {
            parent[Find(x)] = Find(y);
        }

        foreach (var edge in edges)
        {
            int u = edge.Item1;
            int v = edge.Item2;
            if (Find(u) != Find(v))
            {
                Union(u, v);
                connections[u].Add(v);
                connections[v].Add(u);

                GameObject line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                line.transform.parent = transform;
                LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, points[u]);
                lineRenderer.SetPosition(1, points[v]);
            }
        }

        // Ajouter des connexions supplémentaires tout en vérifiant la distance minimale entre les lignes et les points
        float minLineDistance = 0.5f; // Distance minimale entre les lignes et les points
        for (int i = 0; i < points.Length; i++)
        {
            List<(int, float)> distances = new List<(int, float)>();
            for (int j = 0; j < points.Length; j++)
            {
                if (i != j && !connections[i].Contains(j))
                {
                    float distance = Vector3.Distance(points[i], points[j]);
                    distances.Add((j, distance));
                }
            }
            distances.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            int connectionsCount = Random.Range(1, 4);
            if (connectionsCount == 4 && Random.value > 0.75f) // 25% chance to have 4 connections
            {
                connectionsCount = 3;
            }

            for (int k = 0; k < connectionsCount && k < distances.Count; k++)
            {
                int closestPointIndex = distances[k].Item1;
                if (connections[i].Count < 4 && connections[closestPointIndex].Count < 4 && !connections[i].Contains(closestPointIndex) && !connections[closestPointIndex].Contains(i))
                {
                    bool tooClose = false;
                    foreach (Vector3 point in points)
                    {
                        if (point != points[i] && point != points[closestPointIndex])
                        {
                            float distanceToLine = DistancePointToLineSegment(point, points[i], points[closestPointIndex]);
                            if (distanceToLine < minLineDistance)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                    }
                    if (!tooClose)
                    {
                        connections[i].Add(closestPointIndex);
                        connections[closestPointIndex].Add(i);

                        GameObject line = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
                        line.transform.parent = transform;
                        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
                        lineRenderer.SetPosition(0, points[i]);
                        lineRenderer.SetPosition(1, points[closestPointIndex]);
                    }
                }
            }
        }
    }

    float DistancePointToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        float lineLength = Vector3.Distance(lineStart, lineEnd);
        if (lineLength == 0)
            return Vector3.Distance(point, lineStart);

        float t = Mathf.Clamp01(Vector3.Dot(point - lineStart, lineEnd - lineStart) / (lineLength * lineLength));
        Vector3 projection = lineStart + t * (lineEnd - lineStart);
        return Vector3.Distance(point, projection);
    }
}