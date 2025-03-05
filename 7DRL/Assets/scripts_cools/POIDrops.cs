using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class POIDrops : MonoBehaviour
{
public GameObject itemPrefab;
public GameObject parent;
public Item[] dropsPossibles;
public void Start(){
    
}

public void addItem(){ 
  


  int randomChiffre = Random.Range(0, dropsPossibles.Length);
  GestionRessourcesConcreteSingleton.Instance.listSac.Add(dropsPossibles[randomChiffre]);
 Debug.Log(GestionRessourcesConcreteSingleton.Instance.listSac.Count);

//  GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesTotal += dropsPossibles[randomChiffre].valeur;

 /* switch(dropsPossibles[randomChiffre].itemType.ToString()) 
  {
    case "Arme":

     GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesArme += dropsPossibles[randomChiffre].valeurArme;
     GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesTotal += dropsPossibles[randomChiffre].valeurArme; 
      GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesArme += dropsPossibles[randomChiffre].valeur;
    break;

    case "Armure":
      GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesArmure += dropsPossibles[randomChiffre].valeur;
    break;

    case "Potion":
      GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesPotion += dropsPossibles[randomChiffre].valeur;
    break;

    case "Nourriture":
      GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesNourriture += dropsPossibles[randomChiffre].valeur;
    break;

    case "Minerai":
      GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesMinerai += dropsPossibles[randomChiffre].valeur;
    break;

    case "Luxe":
      GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesLuxe += dropsPossibles[randomChiffre].valeur;
    break;

    case "Plante":
      GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesPlante += dropsPossibles[randomChiffre].valeur;
    break;
  }*/
}



}
