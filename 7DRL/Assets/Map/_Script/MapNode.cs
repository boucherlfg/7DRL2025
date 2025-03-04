using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapNode : MonoBehaviour
{
    public int level;
    public Vector3 position;
    public List<MapNode> connections = new List<MapNode>();

    public void ConnectTo(MapNode otherNode)
    {
        if (!connections.Contains(otherNode))
    {
        connections.Add(otherNode);
        otherNode.connections.Add(this);
        CreateVisualConnection(otherNode);
    }
    }

    public void SetVisibility(int currentLevel)
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = level <= currentLevel;
        }

        if (transform.parent != null)
        {
            foreach (Transform child in transform.parent)
            {
                LineRenderer line = child.GetComponent<LineRenderer>();
                if (line != null && IsConnectedToLine(line))
                {
                    // Only show lines connecting nodes of visible levels
                    Vector3 start = line.GetPosition(0);
                    Vector3 end = line.GetPosition(1);
                    
                    MapNode startNode = GetNodeAtPosition(start);
                    MapNode endNode = GetNodeAtPosition(end);
                    
                    if (startNode != null && endNode != null)
                    {
                        line.enabled = startNode.level <= currentLevel && endNode.level <= currentLevel;
                    }
                }
            }
        }
    }

    private MapNode GetNodeAtPosition(Vector3 position)
    {
        if (transform.parent == null) return null;
        
        foreach (Transform child in transform.parent)
        {
            MapNode node = child.GetComponent<MapNode>();
            if (node != null && Vector3.Distance(node.transform.position, position) < 0.1f)
            {
                return node;
            }
        }
        return null;
    }

    private bool IsConnectedToLine(LineRenderer line)
    {
        Vector3 start = line.GetPosition(0);
        Vector3 end = line.GetPosition(1);
        return Vector3.Distance(start, transform.position) < 0.1f || 
               Vector3.Distance(end, transform.position) < 0.1f;
    }

    private void CreateVisualConnection(MapNode otherNode)
    {
        if (transform.parent == null) return;
        
        var mapGen = transform.parent.GetComponent<MapGenerator>();
        GameObject lineObj = Instantiate(mapGen.linePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer line = lineObj.GetComponent<LineRenderer>();
        
        Material lineMaterial = new Material(Shader.Find("Sprites/Default"));
        line.material = lineMaterial;
        
        // Vérifier d'abord si c'est une connexion de niveau 1
        Color lineColor;
        if (level == 1 && otherNode.level == 1)
        {
            lineColor = Color.white;
            Debug.Log("Creating level 1 connection (white)");
        }
        else
        {
            int connectionLevel = Mathf.Max(level, otherNode.level);
            lineColor = GetColorForLevel(connectionLevel);
            Debug.Log($"Creating connection level {connectionLevel}");
        }
        
        line.startColor = lineColor;
        line.endColor = lineColor;
        
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        
        line.SetPosition(0, transform.position);
        line.SetPosition(1, otherNode.transform.position);
        lineObj.transform.parent = transform.parent;
    }

    private Color GetColorForLevel(int level)
    {
        // Niveau 1 reste toujours blanc
        if (level == 1) return Color.white;
        
        // Utiliser le niveau comme seed pour avoir une couleur consistante par niveau
        Random.InitState(level);
        
        return new Color(
            Random.Range(0.2f, 1f),  // Red - minimum 0.2 pour éviter les couleurs trop sombres
            Random.Range(0.2f, 1f),  // Green
            Random.Range(0.2f, 1f),  // Blue
            1f                       // Alpha
        );
    }
}