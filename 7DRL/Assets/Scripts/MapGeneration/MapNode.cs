using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class MapNode : MonoBehaviour
{
    public int level;
    public Vector3 position;
    public List<MapNode> connections = new List<MapNode>();
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private NodeType nodeType;
    private bool isPlayerHere = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        
        // Forcer l'utilisation du shader Sprites/Default
        if (spriteRenderer != null && spriteRenderer.material.shader.name != "Sprites/Default")
        {
            spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
        
        UpdateSpriteVisibility();
        AssignRandomNodeType();
        //UpdateNodeConnections();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMapNode(this);
        }
    }

    public void OnPlayerEnter()
    {
        isPlayerHere = true;
        UpdateSpriteVisibility();
    }

    public void OnPlayerExit()
    {
        isPlayerHere = false;
        UpdateSpriteVisibility();
    }

    private void UpdateSpriteVisibility()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = !isPlayerHere;
        }
    }

    private void AssignRandomNodeType()
    {
        if(GameManager.Instance.IsNodeSaved(this))
        {
            nodeType = GameManager.Instance.GetNodeType(this.position);
            UpdateNodeSprite();
            return;
        }
        else
        {
            nodeType = (NodeType)Random.Range(0, System.Enum.GetValues(typeof(NodeType)).Length);
            UpdateNodeSprite();
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateMapNode(this);
        }
    }

    private void UpdateNodeSprite()
    {
        if (spriteRenderer == null) return;
        
        var mapGen = GetComponentInParent<MapGenerator>();
        if (mapGen == null) return;

        // Add default case with _ pattern
        (Sprite sprite, Color color) = nodeType switch
        {
            NodeType.Ruins => (mapGen.ruinsSprite, new Color(1f, 0.2f, 0.2f, 1f)),    // Rouge plus vif
            NodeType.City => (mapGen.citySprite, new Color(0.2f, 0.2f, 1f, 1f)),      // Bleu plus vif
            NodeType.Farm => (mapGen.farmSprite, new Color(0.2f, 1f, 0.2f, 1f)),      // Vert plus vif
            NodeType.Dungeon => (mapGen.dungeonSprite, new Color(0.8f, 0.2f, 0.8f, 1f)), // Violet plus vif
            _ => (mapGen.ruinsSprite, Color.white)  // Default case handles all remaining values
        };

        spriteRenderer.sprite = sprite;
        spriteRenderer.color = color;
                
        transform.localScale = new Vector3(1f, 1f, 1f);
    }

    // private void UpdateNodeColor()
    // {
    //     if (spriteRenderer == null) return;
    //     spriteRenderer.color = GetColorForNodeType(nodeType);
    // }

    // private Color GetColorForNodeType(NodeType type)
    // {
    //     return type switch
    //     {
    //         NodeType.Ruins => new Color(0.7f, 0.0f, 0.0f, 1f),    // Rouge foncé
    //         NodeType.City => new Color(0.0f, 0.0f, 0.7f, 1f),     // Bleu foncé
    //         NodeType.Farm => new Color(0.0f, 0.7f, 0.0f, 1f),     // Vert foncé
    //         NodeType.Dungeon => new Color(0.5f, 0.0f, 0.5f, 1f),  // Violet
    //         _ => Color.black
    //     };
    // }

    public void ConnectTo(MapNode otherNode)
    {
        if (otherNode == null || connections.Contains(otherNode)) return;

        connections.Add(otherNode);
        otherNode.connections.Add(this);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMapLink(this, otherNode);
            Color roadColor = GameManager.Instance.GetRoadColor(this, otherNode);
            CreateVisualConnection(otherNode, roadColor);
        }
    }

    private bool IsNodeConnectedToLine(MapNode node, LineRenderer line)
    {
        if (line == null || node == null) return false;
        var start = line.GetPosition(0);
        var end = line.GetPosition(1);
        return Vector3.Distance(start, node.transform.position) < 0.1f || 
            Vector3.Distance(end, node.transform.position) < 0.1f;
    }

    public void SetVisibility(int currentLevel)
    {
        if (spriteRenderer != null)
        {
            // Ne modifie la visibilité que si le joueur n'est pas sur ce node
            if (!isPlayerHere)
            {
                spriteRenderer.enabled = level <= currentLevel;
            }
            else
            {
                spriteRenderer.enabled = false; // Force le sprite à rester caché si le joueur est là
            }
        }

        // Reste du code pour les lignes
        if (transform.parent == null) return;

        foreach (Transform child in transform.parent)
        {
            var line = child.GetComponent<LineRenderer>();
            if (line == null || !IsConnectedToLine(line)) continue;

            var start = line.GetPosition(0);
            var end = line.GetPosition(1);

            var startNode = GetNodeAtPosition(start);
            var endNode = GetNodeAtPosition(end);

            if (startNode != null && endNode != null)
            {
                line.enabled = startNode.level <= currentLevel && endNode.level <= currentLevel;
            }
        }
    }

    private MapNode GetNodeAtPosition(Vector3 position)
        => transform.parent?.GetComponentsInChildren<MapNode>()
            .FirstOrDefault(node => Vector3.Distance(node.transform.position, position) < 0.1f);

    private bool IsConnectedToLine(LineRenderer line)
    {
        var start = line.GetPosition(0);
        var end = line.GetPosition(1);

        return Vector3.Distance(start, transform.position) < 0.1f || 
               Vector3.Distance(end, transform.position) < 0.1f;
    }

    private bool DoLinesIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        float denominator = ((p4.y - p3.y) * (p2.x - p1.x)) - ((p4.x - p3.x) * (p2.y - p1.y));

        // Les lignes sont parallèles
        if (Mathf.Abs(denominator) < 0.0001f)
            return false;

        float ua = (((p4.x - p3.x) * (p1.y - p3.y)) - ((p4.y - p3.y) * (p1.x - p3.x))) / denominator;
        float ub = (((p2.x - p1.x) * (p1.y - p3.y)) - ((p2.y - p1.y) * (p1.x - p3.x))) / denominator;

        // Vérifie si l'intersection est sur les segments
        return ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
    }

    private bool WouldIntersectExistingLines(Vector3 startPos, Vector3 endPos)
    {
        if (transform.parent == null) return false;

        var existingLines = transform.parent.GetComponentsInChildren<LineRenderer>();
        foreach (var existingLine in existingLines)
        {
            if (existingLine == null) continue;

            var lineStart = existingLine.GetPosition(0);
            var lineEnd = existingLine.GetPosition(1);

            // Vérifie si les lignes s'intersectent
            if (DoLinesIntersect(startPos, endPos, lineStart, lineEnd))
            {
                // Ignore l'intersection si c'est aux extrémités (les points connectés)
                float endPointThreshold = 0.1f;
                bool isEndPoint = Vector3.Distance(startPos, lineStart) < endPointThreshold ||
                                Vector3.Distance(startPos, lineEnd) < endPointThreshold ||
                                Vector3.Distance(endPos, lineStart) < endPointThreshold ||
                                Vector3.Distance(endPos, lineEnd) < endPointThreshold;

                if (!isEndPoint)
                    return true;
            }
        }
        return false;
    }

    public Color GetRoadColor(MapNode toNode)
    {
        if (GameManager.Instance == null) return Color.white;
        return GameManager.Instance.GetRoadColor(this, toNode);
    }

    private void CreateVisualConnection(MapNode otherNode, Color roadColor)
    {
        if (transform.parent == null) return;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;
        }

        var mapGen = transform.parent.GetComponent<MapGenerator>();
        if (mapGen == null || mapGen.linePrefab == null) return;

        var lineObj = Instantiate(mapGen.linePrefab, Vector3.zero, Quaternion.identity);
        var line = lineObj.GetComponent<LineRenderer>();
        if (line == null) return;

        // Configuration de la ligne
        line.useWorldSpace = true;
        lineObj.transform.parent = transform.parent;

        // Calculer la direction et appliquer un décalage aux extrémités
        Vector3 direction = (otherNode.transform.position - transform.position).normalized;
        float offset = 0.4f;
        
        Vector3 startPos = transform.position + (direction * offset);
        Vector3 endPos = otherNode.transform.position - (direction * offset);

        // Vérifier si la nouvelle ligne intersecte avec des lignes existantes
        if (WouldIntersectExistingLines(startPos, endPos))
        {
            Destroy(lineObj);
            return;
        }

        // Apply the road color
        line.material = new Material(Resources.Load<Material>("Materials/RoadSpriteMaterial"));
        line.startColor = roadColor;
        line.endColor = roadColor;

        // Set line parameters
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        line.positionCount = 2;
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
        line.sortingOrder = -1;
    }

    private void UpdateNodeConnections()
    {
        if (connections == null) return;
        
        foreach (var connection in connections.ToList())
        {
            if (GameManager.Instance != null)
            {
                // Use GameManager to get the road color
                Color roadColor = GameManager.Instance.GetRoadColor(this, connection);
                CreateVisualConnection(connection, roadColor);
            }
        }
    }
    
    private void OnMouseOver()
    {
        transform.localScale = originalScale * 1.2f; // Increase size by 20%
    }

    private void OnMouseExit()
    {
        transform.localScale = originalScale; // Reset to original size
    }

    private void OnMouseDown()
    {
        Debug.Log($"MapNode of type {nodeType} clicked!");
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.MoveToNode(this);
            Debug.Log($"Attempting to move to node of type {nodeType}");
        }
    }

    

    public NodeType GetNodeType() => nodeType;
}