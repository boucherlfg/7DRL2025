using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using System.Collections.Generic;


public class JeuAffichage : MonoBehaviour
{
    public static JeuAffichage Instance { get; private set; }
    public GameObject[,] grid;
    public int dimension;
    public float distance;
    public GameObject tilePrefab;
    public List<TuileType> types;
    private Vector3 positionOffset;

    private void Awake() { Instance = this; }

    private void Start()
    {
        positionOffset = transform.position - new Vector3(dimension * distance / 2.0f - distance / 2, dimension * distance / 2.0f - distance / 2, 0);
        GenererGrille();
    }

    public void GenererGrille()
    {
        grid = new GameObject[dimension, dimension];
        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                CreerTuile(x, y);
            }
        }
    }

    public void CreerTuile(int x, int y)
    {
        GameObject newTile = Instantiate(tilePrefab);
        Tuiles tile = newTile.AddComponent<Tuiles>();
        tile.type = types[Random.Range(0, types.Count)];

        SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();
        renderer.sprite = tile.type.sprite;

        newTile.transform.parent = transform;
        newTile.transform.position = new Vector3(x * distance, y * distance, 0) + positionOffset;

        grid[x, y] = newTile;
    }

    public List<Tuiles> ObtenirVoisins(Tuiles tuile)
    {
        List<Tuiles> voisins = new List<Tuiles>();
        Vector2Int pos = ObtenirPositionTuile(tuile);
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int dir in directions)
        {
            int x = pos.x + dir.x;
            int y = pos.y + dir.y;
            if (x >= 0 && x < dimension && y >= 0 && y < dimension && grid[x, y] != null)
            {
                voisins.Add(grid[x, y].GetComponent<Tuiles>());
            }
        }
        return voisins;
    }

    public Vector2Int ObtenirPositionTuile(Tuiles tuile)
    {
        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                if (grid[x, y] == tuile.gameObject)
                    return new Vector2Int(x, y);
            }
        }
        return Vector2Int.one * -1;
    }

    public void AppliquerGravite()
    {
        for (int x = 0; x < dimension; x++)
        {
            for (int y = 1; y < dimension; y++)
            {
                if (grid[x, y] != null && grid[x, y - 1] == null)
                {
                    for (int k = y; k > 0 && grid[x, k - 1] == null; k--)
                    {
                        grid[x, k - 1] = grid[x, k];
                        grid[x, k] = null;
                        grid[x, k - 1].transform.position = new Vector3(x * distance, (k - 1) * distance, 0) + positionOffset;
                    }
                }
            }
        }
    }

    public void DecalerColonnes()
    {
        for (int x = 0; x < dimension - 1; x++)
        {
            if (grid[x, 0] == null)
            {
                for (int k = x; k < dimension - 1; k++)
                {
                    for (int y = 0; y < dimension; y++)
                    {
                        grid[k, y] = grid[k + 1, y];
                        if (grid[k, y] != null)
                            grid[k, y].transform.position = new Vector3(k * distance, y * distance, 0) + positionOffset;
                    }
                }
                for (int y = 0; y < dimension; y++)
                    grid[dimension - 1, y] = null;
            }
        }
    }

    public void RemplirGrille()
    {
        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                if (grid[x, y] == null)
                {
                    CreerTuile(x, y);
                }
            }
        }
    }
}