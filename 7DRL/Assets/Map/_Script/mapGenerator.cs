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
    private const float SIZE_INCREASE_PER_LEVEL = 10f;


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
    public List<MapLevel> GetLevels() => levels;
    public void AddLevel(MapLevel level)
    {
        levels.Add(level);
    }

    void InitializeMap()
    {
        currentLevel = 1;
        float initialWidth = mapWidth;
        float initialHeight = mapHeight;
        MapLevel initialLevel = new MapLevel(1, initialWidth, initialHeight);
        levels.Add(initialLevel);
        
        // Create center node with no initial connections
        Vector3 centerPosition = Vector3.zero;
        MapNode centerNode = CreatePointNode(centerPosition);
        centerNode.level = currentLevel;
        centerNode.connections.Clear(); // Ensure no connections
        initialLevel.points.Add(centerNode);

        Debug.Log($"Initial node created with {centerNode.connections.Count} connections");
        
        var mapTransition = GetComponent<MapTransition>();
        mapTransition.SetCurrentMapSize(initialWidth, initialHeight);
        mapTransition.TransitionToNewLevel(new List<MapNode> { centerNode });
    }

    public MapNode CreatePointNode(Vector3 position)
    {
        GameObject pointObj = Instantiate(pointPrefab, position, Quaternion.identity);
        MapNode node = pointObj.GetComponent<MapNode>();
        node.position = position;
        node.level = currentLevel;  // Changed from currentLevel + 1
        node.transform.parent = transform;
        node.connections = new List<MapNode>();  // Initialize empty connections list
        Debug.Log($"Created new node at level {node.level} with {node.connections.Count} connections");
        return node;
    }

    private Vector3 FindOptimalPosition(List<MapNode> existingNodes, float currentWidth, float currentHeight)
    {
        float bestScore = float.MinValue;
        Vector3 bestPosition = Vector3.zero;
        int attempts = 30;
        
        float halfWidth = currentWidth / 2f;
        float halfHeight = currentHeight / 2f;
        
        for (int i = 0; i < attempts; i++)
        {
            Vector3 candidatePos = new Vector3(
                Random.Range(-halfWidth, halfWidth),
                Random.Range(-halfHeight, halfHeight),
                0f
            );
            
            float score = EvaluatePosition(candidatePos, existingNodes);
            if (score > bestScore)
            {
                bestScore = score;
                bestPosition = candidatePos;
            }
        }
        
        return bestPosition;
    }
    
    private float EvaluatePosition(Vector3 position, List<MapNode> existingNodes)
    {
        if (existingNodes == null || existingNodes.Count == 0)
            return 1f;

        float minDistance = float.MaxValue;
        float maxDistance = 0f;
        
        foreach (var node in existingNodes)
        {
            if (node == null) continue;
            float dist = Vector3.Distance(position, node.position);
            minDistance = Mathf.Min(minDistance, dist);
            maxDistance = Mathf.Max(maxDistance, dist);
        }
        
        float optimalDistance = 3f;
        float distanceScore = 1f - Mathf.Abs(minDistance - optimalDistance) / optimalDistance;
        float spreadScore = maxDistance > 0 ? minDistance / maxDistance : 0f;
        
        return distanceScore * 0.7f + spreadScore * 0.3f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentLevel < maxLevels)
        {
            if (currentLevel - 1 < levels.Count)
            {
                float currentWidth = mapWidth + (currentLevel * SIZE_INCREASE_PER_LEVEL);
                float currentHeight = mapHeight + (currentLevel * SIZE_INCREASE_PER_LEVEL);
                
                // Collect all existing nodes
                var allNodes = new List<MapNode>();
                foreach (var level in levels)
                {
                    allNodes.AddRange(level.points);
                }
                
                // Create new level with 10 points
                MapLevel newLevel = new MapLevel(10, currentWidth, currentHeight);
                var newNodes = new List<MapNode>();
                
                // Generate 10 new points with optimal positions
                for (int i = 0; i < 10; i++)
                {
                    Vector3 newPosition = FindOptimalPosition(allNodes, currentWidth, currentHeight);
                    MapNode newNode = CreatePointNode(newPosition);
                    newNode.level = currentLevel + 1;
                    newNodes.Add(newNode);
                    allNodes.Add(newNode);
                }
                
                // Connect the new nodes
                ConnectNewNodes(newNodes, allNodes.Except(newNodes).ToList());
                
                // Add to level after connections are made
                newLevel.points.AddRange(newNodes);
                levels.Add(newLevel);
                
                currentLevel++;
                UpdateVisibility();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log($"R pressed - Current Level: {currentLevel}, Max Levels: {maxLevels}");
            ResetMap();
        }
    }

    public void ResetMap()
    {
        // Destroy all child objects
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        levels.Clear();
        InitializeMap();
    }

    void UpdateVisibility()
    {
        if (levels == null) return;
        
        foreach (var level in levels)
        {
            if (level == null) continue;
            foreach (var node in level.points)
            {
                if (node != null)
                {
                    node.SetVisibility(currentLevel);
                }
            }
        }
    }

    private void ConnectNewNodes(List<MapNode> newNodes, List<MapNode> existingNodes)
    {
        const int MAX_CONNECTIONS = 3;
        const float MAX_CONNECTION_DISTANCE = 8f; // Augmenté pour permettre plus de connexions
        const int MIN_CONNECTIONS_TO_EXISTING = 1;

        foreach (var newNode in newNodes)
        {
            Debug.Log($"Finding connections for new node at position {newNode.position}");
            
            // Trouver le nœud existant le plus proche et forcer une connexion
            var closestExisting = existingNodes
                .OrderBy(n => Vector3.Distance(newNode.position, n.position))
                .FirstOrDefault();

            if (closestExisting != null)
            {
                float distToClosest = Vector3.Distance(newNode.position, closestExisting.position);
                Debug.Log($"Closest existing node distance: {distToClosest}");
                
                // Forcer au moins une connexion avec le graphe existant
                if (!WouldCreateIntersection(newNode.position, closestExisting.position))
                {
                    newNode.ConnectTo(closestExisting);
                    Debug.Log($"Connected to closest existing node at {closestExisting.position}");
                }
                else
                {
                    Debug.Log("Connection to closest would create intersection, trying others");
                    // Essayer de trouver une autre connexion possible
                    var alternativeConnection = existingNodes
                        .Where(n => !WouldCreateIntersection(newNode.position, n.position))
                        .OrderBy(n => Vector3.Distance(newNode.position, n.position))
                        .FirstOrDefault();

                    if (alternativeConnection != null)
                    {
                        newNode.ConnectTo(alternativeConnection);
                        Debug.Log($"Connected to alternative existing node at {alternativeConnection.position}");
                    }
                }
            }

            // Ajouter des connexions supplémentaires aux autres nouveaux nœuds
            var potentialNewConnections = newNodes
                .Where(n => n != newNode)
                .Where(n => Vector3.Distance(newNode.position, n.position) < MAX_CONNECTION_DISTANCE)
                .Where(n => !WouldCreateIntersection(newNode.position, n.position))
                .OrderBy(n => Vector3.Distance(newNode.position, n.position))
                .ToList();

            foreach (var other in potentialNewConnections)
            {
                if (newNode.connections.Count >= MAX_CONNECTIONS || 
                    other.connections.Count >= MAX_CONNECTIONS) continue;

                newNode.ConnectTo(other);
                Debug.Log($"Added connection to new node at {other.position}");
            }

            Debug.Log($"Final connection count for node: {newNode.connections.Count}");
        }
    }

    private bool WouldCreateIntersection(Vector3 start, Vector3 end)
    {
        foreach (var level in levels)
        {
            foreach (var node in level.points)
            {
                foreach (var connection in node.connections)
                {
                    if (LinesIntersect(start, end, node.position, connection.position))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool LinesIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        float denominator = ((p4.y - p3.y) * (p2.x - p1.x)) - ((p4.x - p3.x) * (p2.y - p1.y));
        
        if (denominator == 0) return false;
        
        float ua = (((p4.x - p3.x) * (p1.y - p3.y)) - ((p4.y - p3.y) * (p1.x - p3.x))) / denominator;
        float ub = (((p2.x - p1.x) * (p1.y - p3.y)) - ((p2.y - p1.y) * (p1.x - p3.x))) / denominator;
        
        return ua > 0 && ua < 1 && ub > 0 && ub < 1;
    }
    
}