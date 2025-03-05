using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewMonoBehaviourScript : MonoBehaviour
{
public GameObject itemPrefab;
public GameObject parent;
public Item[] dropsPossibles;
public void Start(){
    
}

public void addArme(){ 
  
 int randomChiffre = Random.Range(0, dropsPossibles.Length);
 GestionRessourcesConcreteSingleton.Instance.listSac.Add(dropsPossibles[randomChiffre]);
       GameObject itemInstance = Instantiate(itemPrefab,parent.transform );
       itemInstance.GetComponent<AfficherMagasin>().item = dropsPossibles[randomChiffre];
       itemInstance.GetComponent<AfficherMagasin>().changeImage();

 foreach(Item affaire in GestionRessourcesConcreteSingleton.Instance.listSac){
    Debug.Log(affaire);
 }
for (int i = 0; i < dropsPossibles[randomChiffre].typesItems.Count; i++){
    switch(dropsPossibles[randomChiffre].typesItems[i].ToString()) 
{
  case "Arme":
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesArme += dropsPossibles[randomChiffre].valeurArme;
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesTotal += dropsPossibles[randomChiffre].valeurArme;

    break;
  case "Armure":
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesArmure += dropsPossibles[randomChiffre].valeurArmure;
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesTotal += dropsPossibles[randomChiffre].valeurArmure;

    break;
  case "Potion":
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesPotion += dropsPossibles[randomChiffre].valeurPotion;
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesTotal += dropsPossibles[randomChiffre].valeurPotion;

    break;
  case "Nourriture":
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesNourriture += dropsPossibles[randomChiffre].valeurNourriture;
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesTotal += dropsPossibles[randomChiffre].valeurNourriture;

    break;
  case "Minerai":
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesMinerai += dropsPossibles[randomChiffre].valeurMinerai;
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesTotal += dropsPossibles[randomChiffre].valeurMinerai;

    break;
  case "Luxe":
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesLuxe += dropsPossibles[randomChiffre].valeurLuxe;
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesTotal += dropsPossibles[randomChiffre].valeurLuxe;

    break;
      case "Plante":
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesPlante += dropsPossibles[randomChiffre].valeurPlante;
GestionRessourcesConcreteSingleton.Instance.quantiteRessourcesTotal += dropsPossibles[randomChiffre].valeurPlante;

    break;
}
}


}


}
