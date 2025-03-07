using JetBrains.Annotations;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Tuiles : MonoBehaviour
{
    

    private static Tuiles tuile; 
    private SpriteRenderer Renderer;
    
    public TuileType type;

    public bool trouver = false;

    public class TuilePosition
    {
        private GameObject tuile;
        private int x;
        private int y;
        public TuilePosition(GameObject tuile, int x, int y)
        {
            this.tuile = tuile; 
            this.x = x;
            this.y = y;
        }
        public GameObject getTuile()
        {
            return tuile;
        }
        public int getX()
        {
            return x;
        }
        public int getY()
        {
            return y;
        }
    }

    private void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }

    public void Click()
    {
        Renderer.color = Color.green;
    }
    public void Lache() 
    {
        Renderer.color = Color.white;
    }



    /**
     * 
     * Permet de trouver les tuiles similaire 
     */
    public void TrouverTuiles(int xInit, int yInit, List<TuilePosition> list)
    {

        int xDest = xInit-1;
        int yDest = yInit-1;
        bool saut = false;
       for(int i = yDest; i < yDest + 3; i++)
        {
            for(int j = xDest; j < xDest + 3; j++)
            {
                if (saut == true)
                {     
                    saut = false;
                    if (j >= 0 && j < JeuAffichage.Instance.dimension && i >= 0 && i < JeuAffichage.Instance.dimension)
                    {
                        if (JeuAffichage.Instance.grid[j, i].GetComponent<Tuiles>().type.ToString() == type.ToString() && JeuAffichage.Instance.grid[j, i].GetComponent<Tuiles>().trouver != true)
                        {
                            GameObject itemSimilaire = JeuAffichage.Instance.grid[j, i];
                            TuilePosition nouvelleTuile = new(JeuAffichage.Instance.grid[j, i],i,j);
                          
                            trouver = true;
                            list.Add(nouvelleTuile);


                            nouvelleTuile.getTuile().GetComponent<SpriteRenderer>().color = Color.red;
                        } 
                    }
                }
                else
                {
                    saut = true;
                }
            }             
        }
    }
    int bug = 0;
    public void VerifieMatch(int xPos, int yPos)
    {
        List<GameObject> tuileARetirer = new List<GameObject>();
        List<TuilePosition> tuileTrouver = new List<TuilePosition>();

        TrouverTuiles(xPos, yPos, tuileTrouver);
        if(tuileTrouver.Count != 0)
        {
            int loopProof = 0;
            bool fin = false;
            while( fin == false && loopProof < 20)
            {
                List<TuilePosition> tuileTemporaire = new();

                for(int i = 0; i < tuileTrouver.Count; i++)
                {

                    TrouverTuiles(tuileTrouver[i].getX(), tuileTrouver[i].getY(), tuileTemporaire);
                    tuileARetirer.Add(tuileTrouver[i].getTuile());
                }
                tuileTrouver.Clear();
                for(int i = 0; i < tuileTemporaire.Count; i++)
                {
                    tuileTrouver.Add(tuileTemporaire[i]);
                }
                if (tuileTrouver.Count == 0)
                {
                    fin = true;
                }
                loopProof++;
            }
            for(int i =0; i < tuileARetirer.Count; i++)
            {
                tuileARetirer[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        else
        {
            Debug.Log("rien trouver");
        }
    }

    private void OnMouseDown()
    {

        int xInitial = 0;
        int yInitial = 0;
        int xDestination = 0;
        int yDestination = 0;

        bool tuileTrouver = false;
        int limite = 0;

        List<TuilePosition> whatever = new();
  
            for(int i = 0; i < JeuAffichage.Instance.dimension; i++)
            {
                for(int j = 0; j < JeuAffichage.Instance.dimension; j++)
                {
                    if (JeuAffichage.Instance.grid[i, j] == transform.gameObject)
                    {
                        xInitial = i;
                        yInitial = j;
                        xDestination = i;
                        yDestination = j - 1;
                    }
                }
            }


        //VerifieMatch(xInitial, yInitial);
        TrouverTuiles(xInitial, yInitial, whatever);

            while (tuileTrouver == false && limite < 5)
            {

                if (xDestination>=0 && xDestination < JeuAffichage.Instance.dimension && yDestination >= 0 && yDestination < JeuAffichage.Instance.dimension)
                {
                    if (JeuAffichage.Instance.grid[xDestination, yDestination] == gameObject)
                    {
                        tuileTrouver = true;
                    }    
                }
                if(tuileTrouver == false)
                {
                    xDestination++;

                    if(yDestination == yInitial-1 && xDestination == xInitial+1)
                    {
                        xDestination = xInitial-1;
                        yDestination++;
                    }
                    if(yDestination == yInitial && xDestination == xInitial+2)
                    {
                        xDestination = xInitial;
                        yDestination++;
                    }
                }
                limite++;
            }
        }

    }

