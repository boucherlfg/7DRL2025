using System.Collections.Generic;
using UnityEngine;

public class Tuiles : MonoBehaviour
{
    public TuileType type;

    private void OnMouseDown()
    {
        List<Tuiles> tuilesASupprimer = TrouverTuilesAdjacentes();
        if (tuilesASupprimer.Count > 0)
        {
            SupprimerTuiles(tuilesASupprimer);
            JeuAffichage.Instance.AppliquerGravite();
            JeuAffichage.Instance.DecalerColonnes();
            JeuAffichage.Instance.RemplirGrille();
        }
    }

    private List<Tuiles> TrouverTuilesAdjacentes()
    {
        List<Tuiles> tuilesTrouvees = new List<Tuiles>();
        Queue<Tuiles> queue = new Queue<Tuiles>();
        HashSet<Tuiles> visited = new HashSet<Tuiles>();

        queue.Enqueue(this);
        visited.Add(this);

        while (queue.Count > 0)
        {
            Tuiles current = queue.Dequeue();
            tuilesTrouvees.Add(current);

            foreach (Tuiles voisin in JeuAffichage.Instance.ObtenirVoisins(current))
            {
                if (!visited.Contains(voisin) && voisin.type == this.type)
                {
                    queue.Enqueue(voisin);
                    visited.Add(voisin);
                }
            }
        }
        return tuilesTrouvees;
    }

    private void SupprimerTuiles(List<Tuiles> tuilesASupprimer)
    {
        JeuAffichage.Instance.clientActuel.nbEssais++;

        if(JeuAffichage.Instance.clientActuel.nbEssais == JeuAffichage.Instance.clientActuel.nbEssaiMax)
        {
            JeuAffichage.Instance.ProchainClient();
        }
        if (tuilesASupprimer[0].type == JeuAffichage.Instance.typeActuel)
        {
            int taille = tuilesASupprimer.Count;
            
            JeuAffichage.Instance.argentGagner += taille - 1 + taille;
            JeuAffichage.Instance.textArgent.text = $"{JeuAffichage.Instance.argentGagner.ToString()}g";
            
        }
        foreach (Tuiles tuile in tuilesASupprimer)
        { 
            Vector2Int pos = JeuAffichage.Instance.ObtenirPositionTuile(tuile);
            JeuAffichage.Instance.grid[pos.x, pos.y] = null;
            tuile.type.quantiteEnJeu -= tuile.type.valeur;
            Destroy(tuile.gameObject);
        }
        //Debug.Log($"{tuilesASupprimer.Count} tuiles de type {type} supprimées.");
    }
}