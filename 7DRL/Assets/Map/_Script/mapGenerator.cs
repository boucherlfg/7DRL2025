using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject linePrefab;
    public float mapWidth;
    public float mapHeight;
    public int maxLevels;

    private int currentLevel = 1;

    private List<MapLevel> levels = new List<MapLevel>();

    void Start()
    {
        if (pointPrefab == null)
        {
            Debug.LogError("Point prefab is not assigned!");
            return;
        }

        if (linePrefab == null)
        {
            Debug.LogError("Line prefab is not assigned!");
            return;
        }

        var mapTransition = GetComponent<MapTransition>();
        if (mapTransition == null)
        {
            Debug.LogError("MapTransition component is required!");
            return;
        }

        mapTransition.mapGenerator = this;
        InitializeMap();
    }

    void InitializeMap()
    {
        MapLevel initialLevel = new MapLevel(0, mapWidth, mapHeight);
        levels.Add(initialLevel);
        
        // Create first set of points through MapTransition instead
        var mapTransition = GetComponent<MapTransition>();
        if (mapTransition != null)
        {
            // Create a dummy node at the center
            Vector3 centerPosition = Vector3.zero;
            MapNode centerNode = CreatePointNode(centerPosition);
            centerNode.level = 1;
            initialLevel.points.Add(centerNode);
            
            // Use this center node as the starting point
            mapTransition.TransitionToNewLevel(new List<MapNode> { centerNode });
        }
    }

    public List<MapLevel> GetLevels()
    {
        return levels;
    }

    void GeneratePoints(MapLevel level)
    {
        if (levels.Count < maxLevels)
        {
            MapLevel newLevel = new MapLevel(0, mapWidth, mapHeight);
            levels.Add(newLevel);
            ConnectLevels(level, newLevel);
            currentLevel = levels.Count;
        }
    }

    public MapNode CreatePointNode(Vector3 position)
    {
        GameObject pointObj = Instantiate(pointPrefab, position, Quaternion.identity);
        MapNode node = pointObj.GetComponent<MapNode>();
        node.position = position;
        node.level = currentLevel;
        node.transform.parent = transform;
        return node;
    }

    void ConnectLevels(MapLevel currentLevel, MapLevel newLevel)
    {
        if (currentLevel == null || newLevel == null)
        {
            Debug.LogError("Cannot connect null levels!");
            return;
        }

        // Only use edge nodes with less than 2 connections
        List<MapNode> edgeNodes = currentLevel.points
            .Where(n => n != null && n.connections.Count < 2)
            .Take(2)  // Limit to 2 edge nodes maximum
            .ToList();

        if (edgeNodes.Count == 0)
        {
            Debug.LogWarning("No edge nodes found to connect levels!");
            return;
        }

        var mapTransition = GetComponent<MapTransition>();
        if (mapTransition != null)
        {
            mapTransition.TransitionToNewLevel(edgeNodes);
        }
        else
        {
            Debug.LogError("MapTransition component not found!");
        }
    }

    public void ResetMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
        levels.Clear();
        currentLevel = 1;
        
        // Reinitialize
        InitializeMap();
        UpdateVisibility();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentLevel < maxLevels)
        {
            currentLevel++;
            UpdateVisibility();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetMap();
        }
    }

    void UpdateVisibility()
    {
        foreach (var level in levels)
        {
            foreach (var node in level.points)
            {
                if (node != null)
                {
                    node.SetVisibility(currentLevel);
                }
            }
        }
    }
}