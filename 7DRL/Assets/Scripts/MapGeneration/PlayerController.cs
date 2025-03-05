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

    private IEnumerator MoveAlongPath(MapNode targetNode)
    {
        if (targetNode == null || targetNode.transform == null) yield break;
        
        isMoving = true;
        targetPosition = targetNode.transform.position;
        
        while (isMoving)
        {
            yield return null;
        }

        currentNode = targetNode;
        GameManager.Instance?.SetPlayerPosition(currentNode);
    }
}