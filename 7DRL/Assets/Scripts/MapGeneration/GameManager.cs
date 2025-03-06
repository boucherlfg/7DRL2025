using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<MapNode> MapNodes { get; private set; }
    public List<(MapNode, MapNode, Color)> MapLinks { get; private set; } = new List<(MapNode, MapNode, Color)>();
    public PlayerInfo Player { get; private set; }
    public GameObject playerPrefab;
    private PlayerController playerInstance;
    private readonly (Color color, int points)[] roadTypes = new[]
    {
        (new Color(0.5f, 0.5f, 0.5f, 1f), 1),  // Route en pierre (gris) = 1 point
        (new Color(0.15f, 0.15f, 0.15f, 1f), 2),  // Route en asphalte (noir) = 2 points
        (new Color(0.6f, 0.4f, 0.2f, 1f), 3)   // Route en bois (marron) = 3 points
    };


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeGameManager(); // S'assurer que Player est initialisé au démarrage
    }

    public void SpawnPlayer(MapNode startNode)
    {
        if (startNode == null)
        {
            return;
        }

        if (playerInstance == null && playerPrefab != null)
        {
            // Instantiate exactly at the node's transform position
            Vector3 spawnPosition = startNode.transform.position;
            GameObject playerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            playerInstance = playerObj.GetComponent<PlayerController>();
            
            if (playerInstance == null)
            {
                return;
            }

            // Make sure the player stays at the exact node position
            playerInstance.transform.position = spawnPosition;
            playerInstance.InitializePosition(startNode);
            SetPlayerPosition(startNode);
        }
    }

    public void InitializeGameManager()
    {
        MapNodes = new List<MapNode>();
        MapLinks = new List<(MapNode, MapNode, Color)>();
        Player = new PlayerInfo();
        playerInstance = null;
    }

    public void AddMapNode(MapNode node)
    {
        MapNodes.Add(node);
    }


    private bool IsColorSimilar(Color a, Color b, float tolerance = 0.1f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }

    private string ColorToString(Color color)
    {
        return $"R:{color.r:F3}, G:{color.g:F3}, B:{color.b:F3}";
    }

    private Color GetRandomRoadColor()
    {
        // Use UnityEngine.Random explicitly
        var roadType = roadTypes[UnityEngine.Random.Range(0, roadTypes.Length)];
        return roadType.color;
    }

    public void AddMapLink(MapNode node1, MapNode node2)
    {
        Color roadColor = GetRandomRoadColor();
        MapLinks.Add((node1, node2, roadColor));
        Debug.Log($"Added road with color: {ColorToString(roadColor)}");
    }

    // Method to get road color between two nodes
    public Color GetRoadColor(MapNode node1, MapNode node2)
    {
        var link = MapLinks.Find(l => 
            (l.Item1 == node1 && l.Item2 == node2) || 
            (l.Item1 == node2 && l.Item2 == node1));
        
        return link.Item3;
    }
    // Method to get points for a road color
    public int GetPointsForRoad(Color roadColor)
    {
        foreach (var roadType in roadTypes)
        {
            if (IsColorSimilar(roadColor, roadType.color))
            {
                return roadType.points;
            }
        }
        return 0;
    }

    public void SetPlayerInfo(PlayerInfo playerInfo)
    {
        Player = playerInfo;
    }

    public void SetPlayerPosition(MapNode node)
    {
        Player.CurrentNode = node;
    }

    public MapNode GetPlayerPosition()
    {
        return Player.CurrentNode;
    }

    internal void UpdateMapNode(MapNode mapNode)
    {
        //retrouver le node dans la liste
        var node = MapNodes.Find(n => n == mapNode);
        if (node != null)
        {
            node = mapNode;
        }
    }

    public void DrawMap(Vector3 offset)
    {
        foreach (var link in MapLinks)
        {
            Gizmos.color = link.Item3; // Utiliser la couleur stockée
            Gizmos.DrawLine(link.Item1.position + offset, link.Item2.position + offset);
        }

        foreach (var node in MapNodes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(node.position + offset, 0.2f);
        }
    }

    public void AddPoints(int points)
    {
        if (Player == null)
        {
            return;
        }
        
        Player.Jour += points;
        Debug.Log($"Points added: +{points}, Total Jour: {Player.Jour}");
    }

    private void OnGUI()
    {
        if (Player == null)
        {
            return;
        }

        // Créer une texture blanche pour le fond
        Texture2D backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, new Color(0, 0, 0, 0.7f));
        backgroundTexture.Apply();

        // Style pour le Jour
        GUIStyle JourStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 32,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(10, 10, 5, 5)
        };
        JourStyle.normal.textColor = Color.yellow;

        // Dessiner le fond
        GUI.DrawTexture(new Rect(10, 10, 200, 40), backgroundTexture);

        // Afficher le Jour
        string JourText = $"Jour: {Player.Jour}";
        GUI.Label(new Rect(15, 10, 190, 40), JourText, JourStyle);

        // Debug dans la console à chaque frame pour vérifier
        if (Time.frameCount % 60 == 0) // Log toutes les ~1 seconde
        {
            Debug.Log($"Current Jour Display: {JourText}");
        }

        // Nettoyage
        Destroy(backgroundTexture);
    }
}

public class PlayerInfo
{
    public int Jour { get; set; } = 0;
    public int Level { get; set; }
    public MapNode CurrentNode { get; set; } // Position actuelle du joueur
}
