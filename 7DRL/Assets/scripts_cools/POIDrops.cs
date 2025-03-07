using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class POIDrops : MonoBehaviour
{
public GameObject itemPrefab;
public GameObject parent;

   public Image itemImage;
    public Image itemTypeImage;
    public TMP_Text itemName;
    public TMP_Text itemValue;
    public Sprite spriteUI;




public List<Item> dropsPossibles = new();
public void Start(){
    
}

public void addItem(){ 
  if(GestionRessourcesConcreteSingleton.Instance.nombreRecherchesRestantes>0){
  int randomChiffre = Random.Range(0, dropsPossibles.Count);
  GestionRessourcesConcreteSingleton.Instance.listSac.Add(dropsPossibles[randomChiffre]);
  ChangeUI(dropsPossibles[randomChiffre]);
 Debug.Log(GestionRessourcesConcreteSingleton.Instance.listSac[GestionRessourcesConcreteSingleton.Instance.listSac.Count-1]);

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
  GestionRessourcesConcreteSingleton.Instance.nombreRecherchesRestantes -= 1;
  }
  else{
    itemImage.gameObject.SetActive(false);
    itemTypeImage.gameObject.SetActive(false);
    itemName.text = "Partie terminée.";
  itemValue.gameObject.SetActive(false);
  }
}

     public void  ChangeUI(Item yippie){
    itemImage.sprite = yippie.sprite;
    itemTypeImage.sprite = spriteUI;
    itemName.text = yippie.itemName;
    itemValue.text = yippie.valeur.ToString();
    }

}
