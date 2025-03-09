using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapTransition : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public float distanceFromExistingPoints;
    public float minDistanceBetweenPoints;
    [SerializeField]private int pointsPerLevel = 10;
    [SerializeField]private int priceForTransitionning = 30;
    [SerializeField]private int maxConnections = 4;

    private float currentWidth;
    private float currentHeight;

    public void SetCurrentMapSize(float width, float height)
    {
        currentWidth = width;
        currentHeight = height;
    }

    public void TransitionToNewLevel(List<MapNode> edgeNodes)
    {
        // Calculate points to add for this level
        
        // Create new level with the additional points
        var newLevel = new MapLevel(pointsPerLevel, currentWidth, currentHeight);
        mapGenerator.AddLevel(newLevel);

        // Distribute points among edge nodes
        var pointsPerNode = pointsPerLevel / edgeNodes.Count;
        var remainingPoints = pointsPerLevel % edgeNodes.Count;

        foreach (var edgeNode in edgeNodes)
        {
            var pointsToGenerate = pointsPerNode;
            if (remainingPoints > 0)
            {
                pointsToGenerate++;
                remainingPoints--;
            }
            GenerateNewPointsAroundEdgeNode(edgeNode, pointsToGenerate, newLevel);
        }
    }

    private bool IsPositionValid(Vector3 position, List<MapNode> existingNodes)
    {
        var halfWidth = mapGenerator.mapWidth / 2f;
        var halfHeight = mapGenerator.mapHeight / 2f;
        
        if (position.x < -halfWidth || position.x > halfWidth || 
            position.y < -halfHeight || position.y > halfHeight)
        {
            return false;
        }

        // Check distance from all existing nodes in current generation
        if (existingNodes.Any(node => Vector3.Distance(position, node.transform.position) < minDistanceBetweenPoints))
            return false;

        // Check distance from ALL existing nodes in ALL levels

        return mapGenerator is not null && !mapGenerator.GetLevels().SelectMany(level => level.points)
            .Any(node =>
                node is not null && Vector3.Distance(position, node.transform.position) < minDistanceBetweenPoints);
    }

    private Vector3 GetValidPosition(Vector3 originPosition, List<MapNode> existingNodes)
    {
        var bestPosition = originPosition;
        var bestScore = float.MinValue;
        var numAngles = 16;  // Nombre d'angles à tester
        var numDistances = 5; // Nombre de distances à tester

        var maxPossibleDistance = GetMaxPossibleDistance(originPosition);

        for (var i = 0; i < numAngles; i++)
        {
            var angle = (360f / numAngles) * i;
            for (var j = 0; j < numDistances; j++)
            {
                var distance = maxPossibleDistance * ((j + 1f) / numDistances);
                var direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
                var testPosition = originPosition + direction * distance;

                if (IsPositionValid(testPosition, existingNodes))
                {
                    // Calcul du score pour cette position
                    var score = EvaluatePosition(testPosition, originPosition, existingNodes);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestPosition = testPosition;
                    }
                }
            }
        }

        return bestPosition;
    }

    private float EvaluatePosition(Vector3 position, Vector3 originPosition, List<MapNode> existingNodes)
    {
        var score = 0f;
        
        // Distance par rapport au point d'origine
        var distanceFromOrigin = Vector3.Distance(position, originPosition);
        var maxDistance = GetMaxPossibleDistance(originPosition);
        
        if (existingNodes.Count > 0)
        {
            // Pénalise les positions trop éloignées du centre
            var distanceRatio = distanceFromOrigin / maxDistance;
            score = -distanceRatio + Random.Range(-0.2f, 0.2f);
        }
        else
        {
            // Pour le premier point, on garde une distance modérée
            var idealDistance = maxDistance * 0.4f;
            var distanceDiff = Mathf.Abs(distanceFromOrigin - idealDistance);
            score = -distanceDiff + Random.Range(-0.1f, 0.1f);
        }
        
        return score;
    }

    private float GetMaxPossibleDistance(Vector3 origin)
    {
        var halfWidth = mapGenerator.mapWidth / 2f;
        var halfHeight = mapGenerator.mapHeight / 2f;
        
        // Calculate distance to each border based on the current position
        var distToRight = halfWidth - origin.x;
        var distToLeft = halfWidth + origin.x;
        var distToTop = halfHeight - origin.y;
        var distToBottom = halfHeight + origin.y;
        
        // Calculate the maximum possible distance in the current direction
        var horizontalDist = Mathf.Min(distToRight, distToLeft);
        var verticalDist = Mathf.Min(distToTop, distToBottom);
        
        // Use Pythagorean theorem to get the maximum diagonal distance
        return Mathf.Sqrt(horizontalDist * horizontalDist + verticalDist * verticalDist);
    }
    
    private void GenerateNewPointsAroundEdgeNode(MapNode edgeNode, int exactPointCount, MapLevel targetLevel)
    {
        var newNodes = new List<MapNode>();
        
        // First generate all points with correct level
        for (var i = 0; i < exactPointCount; i++)
        {
            var newPosition = GetValidPosition(edgeNode.transform.position, newNodes);
            var newNode = mapGenerator.CreatePointNode(newPosition);
            newNode.level = edgeNode.level + 1;  // Set level before any connections
            newNodes.Add(newNode);
            targetLevel.points.Add(newNode);
        }

        // Connect to edge node first - this should be level 1
        
        foreach (var newNode in newNodes)
        {
            if (edgeNode.level == 1)
                ConnectNewNodeToEdge(edgeNode, newNode);
        }

        // Then create connections between new nodes
        var connectedNodes = new List<MapNode> { edgeNode };
        var unconnectedNodes = new List<MapNode>(newNodes);


        while (unconnectedNodes.Count > 0)
        {
            MapNode closestUnconnected = null;
            MapNode closestConnected = null;
            var minDistance = float.MaxValue;

            // Find closest pair between connected and unconnected nodes
            foreach (var connected in connectedNodes)
            {
                foreach (var unconnected in unconnectedNodes)
                {
                    var dist = Vector3.Distance(connected.transform.position, unconnected.transform.position);
                    if (dist >= minDistance) continue;
                    
                    minDistance = dist;
                    closestConnected = connected;
                    closestUnconnected = unconnected;
                    
                }
            }

            // Connect the closest pair
            if (closestConnected is null) continue;
            
            ConnectNewNodeToEdge(closestConnected, closestUnconnected);
            connectedNodes.Add(closestUnconnected);
            unconnectedNodes.Remove(closestUnconnected);
            
        }

        // Add some additional connections for redundancy
        foreach (var node in newNodes)
        {
            var potentialConnections = connectedNodes
                .Where(n => n != node && 
                    n.connections.Count < maxConnections && 
                    !node.connections.Contains(n))
                .OrderBy(n => Vector3.Distance(n.transform.position, node.transform.position))
                .Take(2);

            foreach (var target in potentialConnections)
            {
                ConnectNewNodeToEdge(node, target);
            }
        }
    }

    private void ConnectNewNodeToEdge(MapNode sourceNode, MapNode targetNode)
    {
        if (sourceNode.connections.Count < maxConnections && 
            targetNode.connections.Count < maxConnections)
        {
            // Create one-way connection to avoid double connections
            sourceNode.ConnectTo(targetNode);
        }
    }
}