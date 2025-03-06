using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private MapNode currentNode;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitializePosition(MapNode startNode)
    {
        if (startNode != null)
        {
            currentNode = startNode;
            transform.position = startNode.transform.position;
        }
    }

    public void MoveToNode(MapNode targetNode)
    {
        if (targetNode == null || currentNode == null) return;
        if (targetNode == currentNode) return;
        
        if (!currentNode.connections.Contains(targetNode))
        {
            return;
        }

        StartCoroutine(MoveAlongPath(targetNode));
    }

    private void Update()
    {
        if (isMoving && targetPosition != null)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                isMoving = false;
            }
        }
    }

    private void AddPointsForRoad(MapNode fromNode, MapNode toNode)
    {
        if (fromNode == null || toNode == null) return;

        Debug.Log($"Checking road between {fromNode.name} and {toNode.name}");
        var lines = fromNode.transform.parent.GetComponentsInChildren<LineRenderer>();
        
        foreach (var line in lines)
        {
            Vector3 pos0 = line.GetPosition(0);
            Vector3 pos1 = line.GetPosition(1);

            float detectionThreshold = 1.5f;
            bool isConnecting = 
                (Vector3.Distance(pos0, fromNode.transform.position) < detectionThreshold && 
                Vector3.Distance(pos1, toNode.transform.position) < detectionThreshold) ||
                (Vector3.Distance(pos1, fromNode.transform.position) < detectionThreshold && 
                Vector3.Distance(pos0, toNode.transform.position) < detectionThreshold);

            if (isConnecting)
            {
                Color roadColor = line.startColor;
                Debug.Log($"Found connecting road with color: R:{roadColor.r:F3}, G:{roadColor.g:F3}, B:{roadColor.b:F3}");

                // Routes grises = toujours 1 point
                if (IsColorSimilar(roadColor, new Color(0.5f, 0.5f, 0.5f, 1f), 0.2f))
                {
                    GameManager.Instance?.AddPoints(1);
                    Debug.Log("Stone road (gray) = 1 point");
                }
                // Routes noires = toujours 2 points
                else if (IsColorSimilar(roadColor, new Color(0.0f, 0.0f, 0.0f, 1f), 0.2f))
                {
                    GameManager.Instance?.AddPoints(2);
                    Debug.Log("Asphalt road (black) = 2 points");
                }
                // Routes marron = toujours 3 points
                else if (IsColorSimilar(roadColor, new Color(0.6f, 0.4f, 0.2f, 1f), 0.2f))
                {
                    GameManager.Instance?.AddPoints(3);
                    Debug.Log("Wood road (brown) = 3 points");
                }
                else 
                {
                    Debug.LogWarning($"Unrecognized road color: {ColorToString(roadColor)}");
                }
                return;
            }
        }
        Debug.LogWarning("No connecting road found!");
    }

    private bool IsColorSimilar(Color a, Color b, float tolerance = 0.1f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
            Mathf.Abs(a.g - b.g) < tolerance &&
            Mathf.Abs(a.b - b.b) < tolerance;
    }

    private string ColorToString(Color c)
    {
        return $"R: {c.r:F3}, G: {c.g:F3}, B: {c.b:F3}";
    }

    private bool Approximately(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) && 
            Mathf.Approximately(a.g, b.g) && 
            Mathf.Approximately(a.b, b.b);
    }

    private IEnumerator MoveAlongPath(MapNode targetNode)
    {
        if (targetNode == null || targetNode.transform == null) yield break;
        
        isMoving = true;
        targetPosition = targetNode.transform.position;
        
        while (isMoving)
        {
            yield return null;
        }

        // Ajouter les points après le déplacement
        AddPointsForRoad(currentNode, targetNode);
        currentNode = targetNode;
        GameManager.Instance?.SetPlayerPosition(currentNode);
    }
}