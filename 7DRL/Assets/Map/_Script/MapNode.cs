using UnityEngine;
using System.Collections.Generic;

public class MapNode : MonoBehaviour
{
    public int level;
    public Vector3 position;
    public List<MapNode> connections;

    private void Awake()
    {
        connections = new List<MapNode>();
    }

    public void AddConnection(MapNode otherNode)
    {
        if (!connections.Contains(otherNode))
        {
            connections.Add(otherNode);
        }
    }

    public void ConnectTo(MapNode otherNode)
    {
        AddConnection(otherNode);
        otherNode.AddConnection(this);
        
        if (transform.parent != null)
        {
            GameObject lineObj = Instantiate(
                transform.parent.GetComponent<MapGenerator>().linePrefab, 
                Vector3.zero, 
                Quaternion.identity
            );
            
            LineRenderer line = lineObj.GetComponent<LineRenderer>();
            line.SetPosition(0, transform.position);
            line.SetPosition(1, otherNode.transform.position);
            lineObj.transform.parent = transform.parent;
        }
    }

    public void SetVisibility(int currentLevel)
    {

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if (transform.parent != null)
        {
            foreach (Transform child in transform.parent)
            {
                LineRenderer line = child.GetComponent<LineRenderer>();
                if (line != null)
                {
                    Vector3 start = line.GetPosition(0);
                    Vector3 end = line.GetPosition(1);

                    if (Vector3.Distance(start, transform.position) < 0.1f || 
                        Vector3.Distance(end, transform.position) < 0.1f)
                    {
                        line.enabled = true;
                    }
                }
            }
        }
    }
}