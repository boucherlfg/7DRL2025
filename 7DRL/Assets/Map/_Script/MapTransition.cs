using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapTransition : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public float distanceFromExistingPoints;
    public float minDistanceBetweenPoints;
    private const int TARGET_POINTS = 10;
    private const int MAX_CONNECTIONS = 5;

    public void TransitionToNewLevel(List<MapNode> edgeNodes)
    {
        // Calculate exact number of points needed
        int totalPointsNeeded = TARGET_POINTS - 1; // -1 for center node
        int pointsPerNode = totalPointsNeeded / edgeNodes.Count;
        int remainingPoints = totalPointsNeeded % edgeNodes.Count;

        foreach (MapNode edgeNode in edgeNodes)
        {
            int pointsToGenerate = pointsPerNode;
            if (remainingPoints > 0)
            {
                pointsToGenerate++;
                remainingPoints--;
            }
            GenerateNewPointsAroundEdgeNode(edgeNode, pointsToGenerate);
        }
    }

    private bool IsPositionValid(Vector3 position, List<MapNode> existingNodes)
    {
        float halfWidth = mapGenerator.mapWidth / 2f;
        float halfHeight = mapGenerator.mapHeight / 2f;
        
        if (position.x < -halfWidth || position.x > halfWidth || 
            position.y < -halfHeight || position.y > halfHeight)
        {
            return false;
        }

        // Check distance from all existing nodes in current generation
        foreach (var node in existingNodes)
        {
            if (Vector3.Distance(position, node.transform.position) < minDistanceBetweenPoints)
            {
                return false;
            }
        }

        // Check distance from ALL existing nodes in ALL levels
        if (mapGenerator != null)
        {
            foreach (var level in mapGenerator.GetLevels())
            {
                foreach (var node in level.points)
                {
                    if (node != null && 
                        Vector3.Distance(position, node.transform.position) < minDistanceBetweenPoints)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private Vector3 GetValidPosition(Vector3 originPosition, List<MapNode> existingNodes)
    {
        Vector3 newPosition;
        int maxAttempts = 20;  // Increased attempts to find valid position
        int attempts = 0;

        do
        {
            float angle = Random.Range(0f, 360f);
            // Reduce the maximum distance if we're near the map edge
            float maxPossibleDistance = GetMaxPossibleDistance(originPosition);
            float distance = Mathf.Min(distanceFromExistingPoints, maxPossibleDistance) * Random.Range(0.8f, 1f);
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            newPosition = originPosition + direction * distance;

            attempts++;
            if (attempts >= maxAttempts)
            {
                // If we can't find a valid position, try closer to the origin
                distance *= 0.5f;
                newPosition = originPosition + direction * distance;
                
                if (!IsPositionValid(newPosition, existingNodes))
                {
                    newPosition = new Vector3(
                        Mathf.Clamp(newPosition.x, -mapGenerator.mapWidth/2f, mapGenerator.mapWidth/2f),
                        Mathf.Clamp(newPosition.y, -mapGenerator.mapHeight/2f, mapGenerator.mapHeight/2f),
                        0f
                    );
                }
                break;
            }

        } while (!IsPositionValid(newPosition, existingNodes));

        return newPosition;
    }

    private float GetMaxPossibleDistance(Vector3 origin)
    {
        float halfWidth = mapGenerator.mapWidth / 2f;
        float halfHeight = mapGenerator.mapHeight / 2f;
        
        // Calculate distance to each border based on the current position
        float distToRight = halfWidth - origin.x;
        float distToLeft = halfWidth + origin.x;
        float distToTop = halfHeight - origin.y;
        float distToBottom = halfHeight + origin.y;
        
        // Calculate the maximum possible distance in the current direction
        float horizontalDist = Mathf.Min(distToRight, distToLeft);
        float verticalDist = Mathf.Min(distToTop, distToBottom);
        
        // Use Pythagorean theorem to get the maximum diagonal distance
        return Mathf.Sqrt(horizontalDist * horizontalDist + verticalDist * verticalDist);
    }
    
    private void GenerateNewPointsAroundEdgeNode(MapNode edgeNode, int exactPointCount)
    {
        List<MapNode> newNodes = new List<MapNode>();
        
        // First generate all points
        for (int i = 0; i < exactPointCount; i++)
        {
            Vector3 newPosition = GetValidPosition(edgeNode.transform.position, newNodes);
            MapNode newNode = mapGenerator.CreatePointNode(newPosition);
            newNode.level = edgeNode.level + 1;
            newNodes.Add(newNode);
        }

        // Create minimum spanning tree to ensure connectivity
        List<MapNode> connectedNodes = new List<MapNode> { edgeNode };
        List<MapNode> unconnectedNodes = new List<MapNode>(newNodes);

        while (unconnectedNodes.Count > 0)
        {
            MapNode closestUnconnected = null;
            MapNode closestConnected = null;
            float minDistance = float.MaxValue;

            // Find closest pair between connected and unconnected nodes
            foreach (var connected in connectedNodes)
            {
                foreach (var unconnected in unconnectedNodes)
                {
                    float dist = Vector3.Distance(connected.transform.position, unconnected.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closestConnected = connected;
                        closestUnconnected = unconnected;
                    }
                }
            }

            // Connect the closest pair
            if (closestConnected != null && closestUnconnected != null)
            {
                ConnectNewNodeToEdge(closestConnected, closestUnconnected);
                connectedNodes.Add(closestUnconnected);
                unconnectedNodes.Remove(closestUnconnected);
            }
        }

        // Add some additional connections for redundancy
        foreach (var node in newNodes)
        {
            var potentialConnections = connectedNodes
                .Where(n => n != node && 
                    n.connections.Count < MAX_CONNECTIONS && 
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
        if (sourceNode.connections.Count < MAX_CONNECTIONS && 
            targetNode.connections.Count < MAX_CONNECTIONS)
        {
            sourceNode.ConnectTo(targetNode);
            targetNode.ConnectTo(sourceNode);
        }
    }
}