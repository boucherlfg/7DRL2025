using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public class MiniJeu : MonoBehaviour
{

    public List<Sprite> sprites = new();

    public GameObject tilePrefab;   
    public int gridDimension = 8;
    public float distance = 1.0f;
    public GameObject[,] grid;

    public void commencer()
    {

        Vector3 positionOffset = transform.position - new Vector3(gridDimension * distance / 2.0f, 0);
        for(int row = 0; row < gridDimension; row++)
        {

        }

    }

}
