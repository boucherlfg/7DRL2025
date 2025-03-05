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

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        
        // Forcer l'utilisation du shader Sprites/Default
        if (spriteRenderer != null && spriteRenderer.material.shader.name != "Sprites/Default")
        {
            spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
        
        AssignRandomNodeType();
        UpdateNodeConnections(); // Ajouter cette ligne

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMapNode(this);
        }
        else
        {
            Debug.LogError("GameManager instance is null.");
        }
    }

    private void UpdateNodeConnections()
    {
        if (connections == null) return;
        
        foreach (var connection in connections.ToList())
        {
            // Recréer la connexion visuelle
            CreateVisualConnection(connection);
        }
    }

    private void AssignRandomNodeType()
    {
        nodeType = (NodeType)Random.Range(0, System.Enum.GetValues(typeof(NodeType)).Length);
        UpdateNodeSprite();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateMapNode(this);
        }
        else
        {
            Debug.LogError("GameManager instance is null.");
        }
    }

    private void UpdateNodeSprite()
    {
        if (spriteRenderer == null) return;
        
        var mapGen = GetComponentInParent<MapGenerator>();
        if (mapGen == null) return;

        // Définit le sprite et la couleur avec une intensité plus forte
        (Sprite sprite, Color color) = nodeType switch
        {
            NodeType.Ruins => (mapGen.ruinsSprite, new Color(1f, 0.2f, 0.2f, 1f)),    // Rouge plus vif
            NodeType.City => (mapGen.citySprite, new Color(0.2f, 0.2f, 1f, 1f)),      // Bleu plus vif
            NodeType.Farm => (mapGen.farmSprite, new Color(0.2f, 1f, 0.2f, 1f)),      // Vert plus vif
            NodeType.Dungeon => (mapGen.dungeonSprite, new Color(0.8f, 0.2f, 0.8f, 1f)), // Violet plus vif
            _ => (mapGen.ruinsSprite, Color.white)
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

        // Supprimer les anciennes connexions visuelles si elles existent
        if (transform.parent != null)
        {
            var existingLines = transform.parent.GetComponentsInChildren<LineRenderer>();
            foreach (var line in existingLines)
            {
                if (IsConnectedToLine(line) && IsNodeConnectedToLine(otherNode, line))
                {
                    Destroy(line.gameObject);
                }
            }
        }

        connections.Add(otherNode);
        otherNode.connections.Add(this);
        CreateVisualConnection(otherNode);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMapLink(this, otherNode);
        }
        else
        {
            Debug.LogError("GameManager instance is null.");
        }
    }

    // Ajouter cette méthode helper
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
            spriteRenderer.enabled = level <= currentLevel;
        }

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

    private void CreateVisualConnection(MapNode otherNode)
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
            // Si il y a intersection, on peut soit:
            // 1. Ne pas créer la connexion
            // 2. Essayer de trouver un chemin alternatif
            // 3. Ajuster légèrement les points de départ/fin
            // Pour cet exemple, on ne crée pas la connexion
            Debug.Log("Connection ignorée : intersection détectée");
            Destroy(lineObj);
            return;
        }

        // Définir une seule couleur uniforme pour toute la ligne
        Color lineColor = Random.Range(0, 3) switch
        {
            0 => new Color(0.6f, 0.4f, 0.2f, 1f), // Marron
            1 => new Color(0.5f, 0.5f, 0.5f, 1f), // Gris
            2 => new Color(0.3f, 0.3f, 0.3f, 1f)  // Gris foncé
        };

        // Appliquer la couleur uniformément
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = lineColor;
        line.endColor = lineColor;

        // Paramètres de la ligne
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 2;
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
        line.sortingOrder = -1;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMapLink(this, otherNode);
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
    }

    private NodeType GetNodeType() => nodeType;
}