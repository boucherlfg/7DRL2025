using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;



public class JeuAffichage : MonoBehaviour
{
    
    public static JeuAffichage Instance { get; private set; }

    [Header("Zone de jeu")]
    public GameObject[,] grid;
    public int dimension;
    public float distance;
    public GameObject tilePrefab;
    public List<TuileType> types;
    private Vector3 positionOffset;

    public bool partieCommencer = false;

    public int quantiteRessourcesTotal = 0;
    public int quantiteRessourcesPlacer = 0;

    [Header("Gestion clients")]
    public int nbClients = 5;
    public Clients clientActuel;
    public TuileType typeActuel;
    public  List<Clients> clientsTotal;
    public Queue<Clients> clientsEnAttente = new();
    
    public GameObject clientWrapup;
    public GameObject clientVidePrefab;
    public List<GameObject> typeVisuel;
    public GameObject clientVisuel;

    public int argentGagner = 0;
    public TextMeshProUGUI textArgent;

    public GameObject gameOver;


    public void PartieTerminee()
    {
        partieCommencer = false;
        gameOver.SetActive(true);

    }


    public void ChoisirItemVedette()
    {
        if(quantiteRessourcesTotal !=0)
        {
            typeActuel = null;
            for (int i = 0; i < typeVisuel.Count; i++)
            {
                Destroy(typeVisuel[i]);
            }
            typeVisuel.Clear();
            GameObject itemDemande = Instantiate(tilePrefab);

            itemDemande.transform.parent = clientWrapup.transform;
            itemDemande.transform.position = new Vector3(clientWrapup.transform.position.x, clientWrapup.transform.position.y - 1, -2);

            bool itemChoisis = false;

            while (itemChoisis == false)
            {
                TuileType itemDemanderRandom = clientActuel.itemDemander[Random.Range(0, clientActuel.itemDemander.Count)];
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i].itemType.ToString() == itemDemanderRandom.itemType.ToString() && types[i].quantiteEnJeu > 0)
                    {
                        itemChoisis = true;
                        typeActuel = itemDemanderRandom;
                    }
                }
            }

            itemDemande.GetComponent<SpriteRenderer>().sprite = typeActuel.sprite;
            typeVisuel.Add(itemDemande);
        }
    }


    public void AfficherClient()
    {
        if (clientsTotal.Count != 0 && clientWrapup != null && clientVidePrefab != null)
        {
            for (int i = 0; i < clientsTotal.Count; i++)
            {
                clientsTotal[i].nbEssais = 0;
            }

            for (int i = 0; i<nbClients; i++)
            {
                clientsEnAttente.Enqueue(clientsTotal[Random.Range(0,clientsTotal.Count)]);
            }

            clientActuel = clientsEnAttente.Dequeue();

            clientVisuel = Instantiate(clientVidePrefab);
            clientVisuel.transform.parent = clientWrapup.transform;
            clientVisuel.transform.position = clientWrapup.transform.position;

            clientVisuel.GetComponent<SpriteRenderer>().sprite = clientActuel.sprite;
    
            ChoisirItemVedette();

        }
        else
        {
            Debug.LogError("AfficherClient() : y manque de quoi a remplir dans le script :PP");
        }
    }


    public void ProchainClient()
    {
        if(clientsEnAttente.Count != 0)
        {
            clientActuel.nbEssais = 0;
            clientActuel = clientsEnAttente.Dequeue();

            clientVisuel.GetComponent<SpriteRenderer>().sprite = clientActuel.sprite;
            ChoisirItemVedette();
        }
        else
        {
            clientVisuel.SetActive(false);
            for (int i = 0; typeVisuel.Count > i; i++)
            {
                typeVisuel[i].SetActive(false);
            }
            clientActuel = null;
            typeActuel = null;
            PartieTerminee();
        }
       
    }


    public void RecommenceClients()
    {
        clientsEnAttente.Clear();
        clientActuel = null;
        for(int i = 0; i < clientWrapup.transform.childCount; i++)
        {
           GameObject enfant = clientWrapup.transform.GetChild(i).gameObject;
            Destroy(enfant);
        }
        for(int i = 0; i< clientsTotal.Count; i++)
        {
            clientsTotal[i].nbEssais = 0;
        }
        
    }

    private void Awake() { Instance = this; }

    private void Start()
    {
        RemplirQuantiter();
    }


    public void Initialiser()
    {
        if (partieCommencer == false)
        {
            gameOver.SetActive(false);
            AfficherClient();
            RemplirQuantiter();
            positionOffset = transform.position - new Vector3(dimension * distance / 2.0f - distance / 2, dimension * distance / 2.0f - distance / 2, 0);
            GenererGrille();
            partieCommencer = true;
        }
        else
        {
            for(int i =0; i < dimension; i++)
            {
               for (int j =0; j < dimension; j++)
                {
                 Destroy(grid[i,j]);   
                }
            }
            RecommenceClients();
            partieCommencer = false;
        }
        
}
    

    public void RemplirQuantiter()
    {
        quantiteRessourcesTotal = 0;
        quantiteRessourcesPlacer = 0;
        for (int i = 0; i < types.Count; i++)
        {
            types[i].quantite = 0;
        }

        for (int i = 0; i < GestionRessourcesConcreteSingleton.Instance.listSac.Count; i++)
        {
            for(int j = 0; j<types.Count; j++)
            {
                if (GestionRessourcesConcreteSingleton.Instance.listSac[i].itemType.ToString() == types[j].itemType.ToString())
                {
                    types[j].quantite += GestionRessourcesConcreteSingleton.Instance.listSac[i].valeur;
                    types[j].quantiteEnJeu += GestionRessourcesConcreteSingleton.Instance.listSac[i].valeur;
                    types[j].valeur = GestionRessourcesConcreteSingleton.Instance.listSac[i].valeur;
                    quantiteRessourcesTotal += GestionRessourcesConcreteSingleton.Instance.listSac[i].valeur;
                }
            }
        }
    }


    public void GenererGrille()
    {
        int compteur = -1;
        grid = new GameObject[dimension, dimension];
        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {

                //debug.Log(compteur);
                compteur++;
                if (compteur < quantiteRessourcesTotal)
                {
                  
                    CreerTuile(x, y);

                    if(quantiteRessourcesTotal - compteur < (dimension * dimension) - compteur - x*dimension)
                    {

                        int nbAleatoire = Random.Range(0, 5);
                        if (nbAleatoire == 3)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }


    public void CreerTuile(int x, int y)
    {
        GameObject newTile = Instantiate(tilePrefab);
        Tuiles tile = newTile.AddComponent<Tuiles>();

        int bug = 0;
        bool typeTrouver = false;
        while (typeTrouver == false && bug < 100)
        {
            TuileType typeAleatoire = types[Random.Range(0, types.Count)];

            if (typeAleatoire.quantite! > 0)
            {
                typeTrouver = true;
                tile.type = typeAleatoire;
                typeAleatoire.quantite--;
                quantiteRessourcesPlacer++;
            }

        }
        

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
                if (grid[x, y] == null && quantiteRessourcesTotal - quantiteRessourcesPlacer > 0)
                {
                        CreerTuile(x, y);
                }
            }
        }
    }
}