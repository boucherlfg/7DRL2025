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
    private NodeType nodeType;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        AssignRandomNodeType();
        UpdateNodeColor();
    }

    private void AssignRandomNodeType()
    {
        nodeType = (NodeType)Random.Range(0, System.Enum.GetValues(typeof(NodeType)).Length);
    }

    private void UpdateNodeColor()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = GetColorForNodeType(nodeType);
    }

    private Color GetColorForNodeType(NodeType type)
    {
        return type switch
        {
            NodeType.Ruins => new Color(0.7f, 0.0f, 0.0f, 1f),    // Rouge foncé
            NodeType.City => new Color(0.0f, 0.0f, 0.7f, 1f),     // Bleu foncé
            NodeType.Farm => new Color(0.0f, 0.7f, 0.0f, 1f),     // Vert foncé
            NodeType.Dungeon => new Color(0.5f, 0.0f, 0.5f, 1f),  // Violet
            _ => Color.black
        };
    }


    public void ConnectTo(MapNode otherNode)
    {
        if (connections.Contains(otherNode)) return;
    
        connections.Add(otherNode);
        otherNode.connections.Add(this);
        CreateVisualConnection(otherNode);
    }

    public void SetVisibility(int currentLevel)
    {
        if (spriteRenderer is not null)
        {
            spriteRenderer.enabled = level <= currentLevel;
        }

        if (transform.parent is null) return;
        
        foreach (Transform child in transform.parent)
        {
            var line = child.GetComponent<LineRenderer>();
            if (line is null && IsConnectedToLine(line)) continue;
            
            // Only show lines connecting nodes of visible levels
            var start = line.GetPosition(0);
            var end = line.GetPosition(1);
            
            var startNode = GetNodeAtPosition(start);
            var endNode = GetNodeAtPosition(end);
            
            if (startNode is not null && endNode is not null)
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

    private void CreateVisualConnection(MapNode otherNode)
    {
        if (transform.parent is null) return;
        
        var mapGen = transform.parent.GetComponent<MapGenerator>();
        var lineObj = Instantiate(mapGen.linePrefab, Vector3.zero, Quaternion.identity);
        var line = lineObj.GetComponent<LineRenderer>();
        
        var lineMaterial = new Material(Shader.Find("Sprites/Default"));
        line.material = lineMaterial;
        
        // Définir les 3 couleurs de route
        Color greyRoad = new Color(0.5f, 0.5f, 0.5f, 1f);    // Gris
        Color brownRoad = new Color(0.6f, 0.4f, 0.2f, 1f);   // Marron
        Color whiteRoad = Color.white;                        // Blanc

        // Choisir aléatoirement une des trois couleurs
        Color lineColor = Random.Range(0, 3) switch
        {
            0 => greyRoad,
            1 => brownRoad,
            _ => whiteRoad
        };
        
        line.startColor = lineColor;
        line.endColor = lineColor;
        
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        
        line.SetPosition(0, transform.position);
        line.SetPosition(1, otherNode.transform.position);
        lineObj.transform.parent = transform.parent;
    }

    // private Color GetColorForLevel(int level)
    // {
    //     // Niveau 1 reste toujours blanc
    //     if (level == 1) return Color.white;
        
    //     // Utiliser le niveau comme seed pour avoir une couleur consistante par niveau
    //     Random.InitState(level);
        
    //     return new Color(
    //         Random.Range(0.2f, 1f),  // Red - minimum 0.2 pour éviter les couleurs trop sombres
    //         Random.Range(0.2f, 1f),  // Green
    //         Random.Range(0.2f, 1f),  // Blue
    //         1f                       // Alpha
    //     );
    // }
    
    private NodeType GetNodeType() => nodeType;
}