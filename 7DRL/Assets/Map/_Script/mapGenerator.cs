using UnityEngine;
using System.Collections.Generic;
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
        if (!ValidateComponents()) return;
        InitializeMap();
    }

    private bool ValidateComponents()
    {
        if (pointPrefab == null || linePrefab == null)
        {
            Debug.LogError("Missing prefabs!");
            return false;
        }
        
        if (GetComponent<MapTransition>() == null)
        {
            Debug.LogError("MapTransition component is required!");
            return false;
        }
        return true;
    }

    void InitializeMap()
    {
        MapLevel initialLevel = new MapLevel(0, mapWidth, mapHeight);
        levels.Add(initialLevel);
        
        Vector3 centerPosition = Vector3.zero;
        MapNode centerNode = CreatePointNode(centerPosition);
        centerNode.level = 1;
        initialLevel.points.Add(centerNode);
        
        GetComponent<MapTransition>().TransitionToNewLevel(new List<MapNode> { centerNode });
    }

    public List<MapLevel> GetLevels() => levels;

    public MapNode CreatePointNode(Vector3 position)
    {
        GameObject pointObj = Instantiate(pointPrefab, position, Quaternion.identity);
        MapNode node = pointObj.GetComponent<MapNode>();
        node.position = position;
        node.level = currentLevel;
        node.transform.parent = transform;
        return node;
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

    public void ResetMap()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);
        levels.Clear();
        currentLevel = 1;
        InitializeMap();
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        foreach (var level in levels)
            foreach (var node in level.points)
                if (node != null) node.SetVisibility(currentLevel);
    }
}