using UnityEngine;
using System.Collections.Generic;

public class MapLevel
{
    public List<MapNode> points = new List<MapNode>();
    public const float MIN_DISTANCE = 2f;

    private float width;
    private float height;
    private int pointCount;

    public MapLevel(int initialPoints, float mapWidth, float mapHeight)
    {
        pointCount = initialPoints;
        width = mapWidth;
        height = mapHeight;
        points = new List<MapNode>();
    }

    public Vector3[] GeneratePoints()
    {
        List<Vector3> positions = new List<Vector3>();
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        float cellSize = Mathf.Sqrt((width * height) / pointCount);

        // Génère une grille de positions possibles
        for (float x = -halfWidth; x < halfWidth; x += cellSize)
        {
            for (float y = -halfHeight; y < halfHeight; y += cellSize)
            {
                positions.Add(new Vector3(
                    x + Random.Range(cellSize * 0.2f, cellSize * 0.8f),
                    y + Random.Range(cellSize * 0.2f, cellSize * 0.8f),
                    0f
                ));
            }
        }

        // Mélange les positions et prend les n premières
        System.Random rng = new System.Random();
        int n = positions.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Vector3 temp = positions[k];
            positions[k] = positions[n];
            positions[n] = temp;
        }

        return positions.GetRange(0, Mathf.Min(pointCount, positions.Count)).ToArray();
    }
}