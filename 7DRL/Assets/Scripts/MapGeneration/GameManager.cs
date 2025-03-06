using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public Sprite ruinsSprite;
    public Sprite citySprite;
    public Sprite farmSprite;
    public Sprite dungeonSprite;
    public static GameManager Instance { get; private set; }

    public List<MapNode> MapNodes { get; private set; }
    public List<(MapNode, MapNode, Color)> MapLinks { get; private set; } = new List<(MapNode, MapNode, Color)>();
    public PlayerInfo Player { get; private set; }
    public GameObject playerPrefab;
    private PlayerController playerInstance;
    private readonly (Color color, int points)[] roadTypes = new[]
    {
        (new Color(0.6f, 0.4f, 0.2f, 1f), 3),  // Route de terre (marron) = 3 points
        (new Color(0.5f, 0.5f, 0.5f, 1f), 2),  // Route pavée (gris) = 2 points
        (new Color(0.15f, 0.15f, 0.15f, 1f), 1) // Grande route (noir) = 1 point
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
        InitializeGameManager();
    }

    public void SpawnPlayer(MapNode startNode)
    {
        if (startNode == null)
        {
            return;
        }

        if (playerInstance == null && playerPrefab != null)
        {
            // Call OnPlayerEnter before instantiating the player
            startNode.OnPlayerEnter();

            // Instantiate exactly at the node's transform position
            Vector3 spawnPosition = startNode.transform.position;
            GameObject playerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            playerInstance = playerObj.GetComponent<PlayerController>();
            
            if (playerInstance == null)
            {
                return;
            }

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
        if (Player == null) return;

        // Créer une texture pour le fond
        Texture2D backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, new Color(0, 0, 0, 0f));
        backgroundTexture.Apply();

        // Style pour le texte
        GUIStyle textStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16, // Réduction de la taille de police
            fontStyle = FontStyle.Normal,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(15, 15, 5, 5)
        };
        textStyle.normal.textColor = Color.white;

        // Style pour le titre
        GUIStyle titleStyle = new GUIStyle(textStyle)
        {
            fontSize = 20, // Réduction de la taille de police du titre
            fontStyle = FontStyle.Bold
        };
        titleStyle.normal.textColor = Color.yellow;

        // Dimensions et positions
        const float padding = 10f;
        const float elementHeight = 30f;
        const float legendWidth = 350f;
        float totalHeight = 380f;
        
        // Position du score (en haut à gauche) - garde son opacité d'origine
        Texture2D scoreBackgroundTexture = new Texture2D(1, 1);
        scoreBackgroundTexture.SetPixel(0, 0, new Color(0, 0, 0, 0.7f));
        scoreBackgroundTexture.Apply();
        
        GUI.DrawTexture(new Rect(padding, padding, legendWidth * 0.5f, elementHeight * 1.5f), scoreBackgroundTexture);
        GUI.Label(new Rect(padding, padding, legendWidth * 0.5f, elementHeight), 
            $"Jour: {Player.Jour}", titleStyle);

        // Position de la légende (à droite) - avec le fond plus transparent
        float rightPadding = Screen.width - legendWidth - padding;
        GUI.DrawTexture(new Rect(rightPadding, padding, legendWidth, totalHeight), backgroundTexture);

        float currentY = padding + 10f; // Ajout d'un peu d'espace en haut

        // Section Nodes
        GUI.Label(new Rect(rightPadding, currentY, legendWidth, elementHeight), 
            "Types de Lieux:", titleStyle);
        currentY += elementHeight + 5f;

        float iconSize = elementHeight - 4f;
        float iconPadding = 15f;

        if (ruinsSprite != null)
        {
            GUI.DrawTexture(new Rect(rightPadding + iconPadding, currentY + 2f, iconSize, iconSize), 
                ruinsSprite.texture);
            GUI.Label(new Rect(rightPadding + iconSize + iconPadding * 2, currentY, legendWidth - iconSize, elementHeight), 
                "Ruines - Site abandonné", textStyle);
        }
        currentY += elementHeight;

        if (citySprite != null)
        {
            GUI.DrawTexture(new Rect(rightPadding + iconPadding, currentY + 2f, iconSize, iconSize), 
                citySprite.texture);
            GUI.Label(new Rect(rightPadding + iconSize + iconPadding * 2, currentY, legendWidth - iconSize, elementHeight), 
                "Ville - Centre habité", textStyle);
        }
        currentY += elementHeight;

        if (farmSprite != null)
        {
            GUI.DrawTexture(new Rect(rightPadding + iconPadding, currentY + 2f, iconSize, iconSize), 
                farmSprite.texture);
            GUI.Label(new Rect(rightPadding + iconSize + iconPadding * 2, currentY, legendWidth - iconSize, elementHeight), 
                "Ferme - Zone agricole", textStyle);
        }
        currentY += elementHeight;

        if (dungeonSprite != null)
        {
            GUI.DrawTexture(new Rect(rightPadding + iconPadding, currentY + 2f, iconSize, iconSize), 
                dungeonSprite.texture);
            GUI.Label(new Rect(rightPadding + iconSize + iconPadding * 2, currentY, legendWidth - iconSize, elementHeight), 
                "Donjon - Zone dangereuse", textStyle);
        }
        currentY += elementHeight * 1.5f;

        // Section Routes
        GUI.Label(new Rect(rightPadding, currentY, legendWidth, elementHeight), 
            "Types de Routes:", titleStyle);
        currentY += elementHeight + 5f;

        textStyle.normal.textColor = new Color(0.6f, 0.4f, 0.2f);
        GUI.Label(new Rect(rightPadding, currentY, legendWidth, elementHeight), 
            "■ Route de terre - 3 jours par déplacement", textStyle);
        currentY += elementHeight;

        textStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
        GUI.Label(new Rect(rightPadding, currentY, legendWidth, elementHeight), 
            "■ Route pavée - 2 jours par déplacement", textStyle);
        currentY += elementHeight;

        textStyle.normal.textColor = new Color(0.15f, 0.15f, 0.15f);
        GUI.Label(new Rect(rightPadding, currentY, legendWidth, elementHeight), 
            "■ Grande route - 1 jour par déplacement", textStyle);

        // Nettoyage
        Destroy(backgroundTexture);
        Destroy(scoreBackgroundTexture);
    }
}

public class PlayerInfo
{
    public int Jour { get; set; } = 0;
    public int Level { get; set; }
    public MapNode CurrentNode { get; set; } // Position actuelle du joueur
}
