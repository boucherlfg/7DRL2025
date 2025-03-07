using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
public class JeuAffichage : MonoBehaviour
{
   

    public List<Sprite> sprites = new();
    public List<TuileType> types = new();
    public GameObject tilePrefab;
    public int dimension;
    public float distance;
    public GameObject[,] grid;

    public bool jeuEnCours = false;


    public JeuAffichage script;

    public void Afficher(int dimension)
    {
        if (sprites.Count != 0 && tilePrefab != null)
        {
                grid = new GameObject[dimension, dimension];

                Vector3 positionOffset = /**new Vector3(0, 0, 0)*/transform.position - new Vector3(dimension * distance / 2.0f - distance / 2, dimension * distance / 2.0f - distance / 2, 0);

                for (int row = 0; row < dimension; row++)
                {
                    for (int column = 0; column < dimension; column++)
                    {


                        GameObject newTile = Instantiate(tilePrefab); 

                        Tuiles tile = newTile.AddComponent<Tuiles>(); 
                                                                      //a refaire pour etre proportionnel a inventaire;
                        tile.type = types[Random.Range(0, types.Count)]; //good

                        SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>(); 
                        renderer.sprite = tile.type.sprite; 

                        newTile.transform.parent = transform;
                        newTile.transform.position = new Vector3(column * distance, row * distance, 0) + positionOffset;


                        grid[column, row] = newTile;
                        jeuEnCours = true;

                    }
            }
        }
        else
        {
            Debug.Log("JeuAffichage : y manque de quoi dans les attributs");
        }
    }


    public void recommencer()
    {
        for (int row = 0; row < dimension; row++)
        {
            for (int column = 0; column < dimension; column++)
            {
                GameObject tuile = grid[column, row];
                Tuiles tuileScript = tuile.GetComponent<Tuiles>();

                tuileScript.type = types[Random.Range(0, types.Count)]; //good

                SpriteRenderer renderer = tuile.GetComponent<SpriteRenderer>();
                renderer.sprite = tuileScript.type.sprite;
                renderer.color = Color.white;
            }
        }
    }










    public void BougeTiles(int xInitial,int yInitial, int xDestination, int yDestination)
    {
        GameObject tile1 = grid[xInitial, yInitial];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();

        GameObject tile2 = grid[xDestination, yDestination];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();



        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;
        
    }


    public static JeuAffichage Instance { get; private set; }
    void Awake() { Instance = this; }


    private void Start()
    {
        Afficher(dimension);

    }
    
}