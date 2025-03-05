using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    [Header("Node Prefabs")]
    public GameObject nodePrefab;  // Prefab de base avec SpriteRenderer
    public Sprite ruinsSprite;     // Sprite pour les ruines
    public Sprite citySprite;      // Sprite pour les villes
    public Sprite farmSprite;      // Sprite pour les fermes
    public Sprite dungeonSprite;   // Sprite pour les donjons

    public GameObject linePrefab;
    public float mapWidth;
    public float mapHeight;
    public int maxLevels;

    private int currentLevel = 1;
    private List<MapLevel> levels = new List<MapLevel>();
    public const float SIZE_INCREASE_PER_LEVEL = 10f;

    private void Start()
    {
        if (!ValidateComponents())
        {
            Debug.LogError("Missing required components on MapGenerator!");
            enabled = false;
        }
        InitializeMap();
    }
    private bool ValidateComponents()
    {
        if (nodePrefab == null || linePrefab == null)
        {
            return false;
        }

        if (GetComponent<MapTransition>() == null)
        {
            return false;
        }

        return true;
    }
    private void InitializeMap()
    {
        currentLevel = 1;
        var initialWidth = mapWidth;
        var initialHeight = mapHeight;
        var initialLevel = new MapLevel(1, initialWidth, initialHeight);
        levels.Add(initialLevel);

        // Create center node with no initial connections
        var centerPosition = Vector3.zero;
        var centerNode = CreatePointNode(centerPosition);
        centerNode.level = 1;
        initialLevel.points.Add(centerNode);

        // Spawn player on center node
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMapNode(centerNode);
            GameManager.Instance.SpawnPlayer(centerNode);
            Debug.Log($"Player spawned at center: {centerNode.transform.position}");
        }

        // Generate additional nodes for the first level (like before)
        var allNodes = new List<MapNode> { centerNode };
        var newNodes = new List<MapNode>();

        // Generate 10 new points with optimal positions
        for (var i = 0; i < 10; i++)
        {
            var newPosition = FindOptimalPosition(allNodes, initialWidth, initialHeight);
            var newNode = CreatePointNode(newPosition);
            newNode.level = 1;
            newNodes.Add(newNode);
            allNodes.Add(newNode);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddMapNode(newNode);
            }
        }

        // Connect the new nodes
        ConnectNewNodes(newNodes, new List<MapNode> { centerNode });

        // Add to level after connections are made
        initialLevel.points.AddRange(newNodes);

        var mapTransition = GetComponent<MapTransition>();
        mapTransition.SetCurrentMapSize(initialWidth, initialHeight);
    }

    public MapNode CreatePointNode(Vector3 position)
    {
        // Create the node at the exact position
        var pointObj = Instantiate(nodePrefab, position, Quaternion.identity, transform);
        var node = pointObj.GetComponent<MapNode>();
        
        // Ensure sprite renderer is set up correctly
        var spriteRenderer = pointObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 1;
        }

        // Make sure both the node's stored position and transform position match
        node.position = position;
        node.transform.position = position;
        
        pointObj.name = $"Node_{position}";
        node.connections = new List<MapNode>();
        node.level = 0;

        Debug.Log($"Created node at position: {position}, Transform position: {node.transform.position}");
        return node;
    }

    private static Vector3 FindOptimalPosition(List<MapNode> existingNodes, float currentWidth, float currentHeight)
    {
        var bestScore = float.MinValue;
        var bestPosition = Vector3.zero;
        var attempts = 30;

        var halfWidth = currentWidth / 2f;
        var halfHeight = currentHeight / 2f;

        for (var i = 0; i < attempts; i++)
        {
            var candidatePos = new Vector3(
                Random.Range(-halfWidth, halfWidth),
                Random.Range(-halfHeight, halfHeight),
                0f
            );

            var score = EvaluatePosition(candidatePos, existingNodes);

            if (score <= bestScore) continue;

            bestScore = score;
            bestPosition = candidatePos;
        }

        return bestPosition;
    }

    private static float EvaluatePosition(Vector3 position, List<MapNode> existingNodes)
    {
        if (existingNodes == null || existingNodes.Count == 0)
            return 1f;

        var minDistance = float.MaxValue;
        var maxDistance = 0f;

        var distances = existingNodes
            .Where(node => node != null)
            .Select(node => Vector3.Distance(position, node.position))
            .ToList();

        minDistance = distances.Min();
        maxDistance = distances.Max();

        var optimalDistance = 3f;
        var distanceScore = 1f - Mathf.Abs(minDistance - optimalDistance) / optimalDistance;
        var spreadScore = maxDistance > 0 ? minDistance / maxDistance : 0f;

        return distanceScore * 0.7f + spreadScore * 0.3f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentLevel < maxLevels)
        {
            if (currentLevel - 1 < levels.Count)
            {
                var currentWidth = mapWidth + (currentLevel * SIZE_INCREASE_PER_LEVEL);
                var currentHeight = mapHeight + (currentLevel * SIZE_INCREASE_PER_LEVEL);

                // Collect all existing nodes
                var allNodes = new List<MapNode>();
                foreach (var level in levels)
                {
                    allNodes.AddRange(level.points);
                }

                // Create new level with 10 points
                var newLevel = new MapLevel(10, currentWidth, currentHeight);
                var newNodes = new List<MapNode>();

                // Generate 10 new points with optimal positions
                for (var i = 0; i < 10; i++)
                {
                    var newPosition = FindOptimalPosition(allNodes, currentWidth, currentHeight);
                    var newNode = CreatePointNode(newPosition);
                    newNode.level = currentLevel + 1;
                    newNodes.Add(newNode);
                    allNodes.Add(newNode);

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.AddMapNode(newNode);
                    }
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
            ResetMap();
        }
    }

   private void ResetMap()
    {
        // First clean up the player if it exists
        if (GameManager.Instance != null)
        {
            var playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                DestroyImmediate(playerController.gameObject);
            }
            GameManager.Instance.InitializeGameManager();
        }

        // Then clean up map nodes
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        levels.Clear();
        InitializeMap();
    }
    private void UpdateVisibility()
    {
        levels?.ForEach(level =>
        {
            level?.points?.ForEach(node =>
            {
                node?.SetVisibility(currentLevel);
            });
        });
    }

    private void ConnectNewNodes(List<MapNode> newNodes, List<MapNode> existingNodes)
    {
        const int MAX_CONNECTIONS = 4;
        const float MAX_CONNECTION_DISTANCE = 8f;
        const float FALLBACK_DISTANCE = 12f; // Distance plus grande pour les points isolés

        foreach (var newNode in newNodes)
        {
            var isConnected = false;
            var searchDistance = MAX_CONNECTION_DISTANCE;

            while (!isConnected)
            {
                // Chercher connexion avec le graphe existant
                var closestExisting = existingNodes
                    .Where(n => Vector3.Distance(newNode.position, n.position) < searchDistance)
                    .Where(n => !WouldCreateIntersection(newNode.position, n.position))
                    .OrderBy(n => Vector3.Distance(newNode.position, n.position))
                    .FirstOrDefault();

                if (closestExisting != null)
                {
                    newNode.ConnectTo(closestExisting);
                    isConnected = true;
                }
                else
                {
                    searchDistance = FALLBACK_DISTANCE; // Augmenter la distance si pas de connexion trouvée
                }

                // Si toujours pas connecté, forcer une connexion au plus proche sans vérifier les intersections
                if (isConnected) continue;

                var forceConnect = existingNodes
                    .OrderBy(n => Vector3.Distance(newNode.position, n.position))
                    .First();

                newNode.ConnectTo(forceConnect);
                isConnected = true;

            }

            // Ajouter des connexions supplémentaires aux autres nouveaux nœuds
            var potentialNewConnections = newNodes
                .Where(n => n != newNode)
                .Where(n => Vector3.Distance(newNode.position, n.position) < MAX_CONNECTION_DISTANCE)
                .Where(n => !WouldCreateIntersection(newNode.position, n.position))
                .OrderBy(n => Vector3.Distance(newNode.position, n.position))
                .Take(2) // Limiter à 2 connexions supplémentaires
                .ToList();

            potentialNewConnections
                .Where(other => newNode.connections.Count < MAX_CONNECTIONS && other.connections.Count < MAX_CONNECTIONS)
                .ToList()
                .ForEach(other =>
                {
                    newNode.ConnectTo(other);
                });
        }

        // Vérification finale pour les points isolés
        foreach (var newNode in newNodes)
        {
            if (newNode.connections.Count != 0) continue;

            var closestNode = existingNodes
                .Concat(newNodes.Where(n => n != newNode))
                .OrderBy(n => Vector3.Distance(newNode.position, n.position))
                .First();

            newNode.ConnectTo(closestNode);
        }
    }

    private bool WouldCreateIntersection(Vector3 start, Vector3 end)
    {
        return levels
            .SelectMany(level => level.points)
            .SelectMany(node => node.connections, (node, connection) => new { node, connection })
            .Any(pair => LinesIntersect(start, end, pair.node.position, pair.connection.position));
    }

    private bool LinesIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        var denominator = ((p4.y - p3.y) * (p2.x - p1.x)) - ((p4.x - p3.x) * (p2.y - p1.y));

        if (denominator == 0) return false;

        var ua = (((p4.x - p3.x) * (p1.y - p3.y)) - ((p4.y - p3.y) * (p1.x - p3.x))) / denominator;
        var ub = (((p2.x - p1.x) * (p1.y - p3.y)) - ((p2.y - p1.y) * (p1.x - p3.x))) / denominator;

        return ua > 0 && ua < 1 && ub > 0 && ub < 1;
    }

    public List<MapLevel> GetLevels() => levels;
    public void AddLevel(MapLevel level)
    {
        levels.Add(level);
    }
}