using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class JeuAffichage : MonoBehaviour
{
    

    public List<Sprite> sprites = new();
    public GameObject tilePrefab;
    public int dimension;
    public float distance;
    public GameObject[,] grid;




    public void afficher(int dimension)
    {
        if(sprites.Count != 0 && tilePrefab != null)
        {

        
            grid = new GameObject[dimension, dimension];

            Vector3 positionOffset = /**new Vector3(0, 0, 0)*/transform.position - new Vector3(dimension * distance  / 2.0f - distance / 2, dimension * distance / 2.0f-distance/2, 0);

            for(int row = 0; row < dimension; row++)
            {
                for(int column = 0; column < dimension; column++)
                {
                    GameObject newTile = Instantiate(tilePrefab);
                    SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>();

                    renderer.sprite = sprites[Random.Range(0, sprites.Count)];
                    newTile.transform.parent = transform;
                    newTile.transform.position = new Vector3(column * distance, row * distance, 0) + positionOffset;

                    grid[column, row] = newTile;
                }
            }
        }
        else
        {
            Debug.Log("JeuAffichage : y manque de quoi dans les attributs");
        }
    }





    private void Start()
    {
        afficher(dimension);
    }
    
}