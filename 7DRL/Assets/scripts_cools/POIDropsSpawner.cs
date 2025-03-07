using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class POIDropsSpawner : MonoBehaviour
{
    public GameObject spawnerPrefab;
    public GameObject databaseObject;
    public ItemType ressourceFavorisee;
    public ItemType ressourceDefavorisee;
    public ItemType[] ressourcesDisponibles;

    public GameObject[] arrayGameObjectSpawnersArmes;
    public GameObject[] arrayGameObjectSpawnersArmures;
    public GameObject[] arrayGameObjectSpawnersNourritures;
    public GameObject[] arrayGameObjectSpawnersPotions;    
    public GameObject[] arrayGameObjectSpawnersPlantes;
    public GameObject[] arrayGameObjectSpawnersMinerais;
    public GameObject[] arrayGameObjectSpawnersLuxes;


    public Sprite spriteTypeArme;
    public Sprite spriteTypeArmure;
    public Sprite spriteTypeMinerai;
    public Sprite spriteTypePlante;
    public Sprite spriteTypeLuxe;
    public Sprite spriteTypePotion;
    public Sprite spriteTypeNourriture;

    public Sprite[] spritesArmes;
    public Sprite[] spritesArmures;

    public Sprite[] spritesNourritures;

    public Sprite[] spritesPotions;

    public Sprite[] spritesPlantes;

    public Sprite[] spritesLuxes;

    public Sprite[] spritesMinerais;
    public Image itemImage;
    public Image itemTypeImage;
    public TMP_Text itemName;
    public TMP_Text itemValue;



    public int tierPotentiel;
    public GameObject[] spawnPoints;
    public GameObject ressourcesSpawner;
    public enum ItemType{Arme, Armure, Potion, Nourriture, Minerai, Plante, Luxe};

    void Start()
    {
        GestionRessourcesConcreteSingleton.Instance.nombreRecherchesRestantes = 5;
       // RessourcesDeFavSetup();
        //RessourcesSpawn();
      //  public GameObject[] arrayArraySpawners = new GameObject[]{arrayGameObjectSpawnersArmes, arrayGameObjectSpawnersArmures};
        AjoutRessources(arrayGameObjectSpawnersArmes,"Arme");
        AjoutRessources(arrayGameObjectSpawnersNourritures,"Nourriture");
        AjoutRessources(arrayGameObjectSpawnersPotions,"Potion");
        AjoutRessources(arrayGameObjectSpawnersPlantes,"Plante");
        AjoutRessources(arrayGameObjectSpawnersMinerais,"Minerai");
        AjoutRessources(arrayGameObjectSpawnersLuxes,"Luxe");
        AjoutRessources(arrayGameObjectSpawnersArmures,"Armure");


    }

    void RessourcesDeFavSetup(){
        while(ressourceFavorisee == ressourceDefavorisee){
            RandomiserRessources();
        }
    }

    void RandomiserRessources(){
        ressourceFavorisee = ressourcesDisponibles[Random.Range(0, ressourcesDisponibles.Length)];
        ressourceDefavorisee = ressourcesDisponibles[Random.Range(0, ressourcesDisponibles.Length)];
    }

   void RessourcesSpawn(){
    tierPotentiel = GestionRessourcesConcreteSingleton.Instance.tierActuel;
        for (int i = 0; i < ressourcesDisponibles.Length ; i++){
            tierPotentiel = GestionRessourcesConcreteSingleton.Instance.tierActuel;

            if(ressourcesDisponibles[i].ToString() == ressourceFavorisee.ToString()){
                tierPotentiel++;
            }
            else if(ressourcesDisponibles[i].ToString()== ressourceDefavorisee.ToString()){
                tierPotentiel--;
            }

          ressourcesSpawner = Instantiate (spawnerPrefab, spawnPoints[i].transform);



            switch (ressourcesDisponibles[i].ToString())
            {
                 case "Arme":
                    for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().armesList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().armesList[j].tier<=tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().armesList[j]);
                        }
                        ressourcesSpawner.GetComponent<Image>().sprite = spritesArmes[tierPotentiel];
                    }
                break;

                case "Armure":
                    for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().armuresList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().armuresList[j].tier<=tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().armuresList[j]);
                        }
                        ressourcesSpawner.GetComponent<Image>().sprite = spritesArmures[tierPotentiel];

                    }
                break;

                case "Potion":
                    for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().potionsList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().potionsList[j].tier<= tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().potionsList[j]);
                        }
                    ressourcesSpawner.GetComponent<Image>().sprite = spritesPotions[tierPotentiel];

                    }
                break;

                case "Nourriture":
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().nourrituresList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().nourrituresList[j].tier<= tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().nourrituresList[j]);
                        }
                        ressourcesSpawner.GetComponent<Image>().sprite = spritesNourritures[tierPotentiel];
                        
                    }
                break;

                case "Minerai":
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().mineraisList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().mineraisList[j].tier<=tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().mineraisList[j]);
                        }
                        ressourcesSpawner.GetComponent<Image>().sprite = spritesMinerais[tierPotentiel];

                    }
                break;

                case "Luxe":
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().luxesList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().luxesList[j].tier<= tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().luxesList[j]);
                        }
                    ressourcesSpawner.GetComponent<Image>().sprite = spritesLuxes[tierPotentiel];

                    }
                break;

                case "Plante":
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().plantesList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().plantesList[j].tier<= tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().plantesList[j]);
                        }
                    ressourcesSpawner.GetComponent<Image>().sprite = spritesPlantes[tierPotentiel];

                    }

                break;
            }
        //ressourcesSpawner.GetComponent<POIDrops>.dropsPossibles










}

}
        public void AjoutRessources(GameObject[] arrayGameObject,string leType ){

            for (int k = 0; k< arrayGameObject.Length; k++){
                arrayGameObject[k].GetComponent<POIDrops>().itemImage = this.itemImage;
                arrayGameObject[k].GetComponent<POIDrops>().itemTypeImage = this.itemTypeImage;
                arrayGameObject[k].GetComponent<POIDrops>().itemName = this.itemName;
                arrayGameObject[k].GetComponent<POIDrops>().itemValue = this.itemValue;
                Debug.Log("yippie");
            }
switch (leType)
            {
                 case "Arme":
                 for (int i = 0 ; i< arrayGameObject.Length; i++){

                         arrayGameObject[i].GetComponent<POIDrops>().spriteUI = spriteTypeArme;

                    for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().armesList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().armesList[j].tier<=tierPotentiel){
                            arrayGameObject[i].GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().armesList[j]);
                        }
                    }
                    }
                break;

                case "Armure":
                    for (int i = 0 ; i< arrayGameObject.Length; i++){
                                                 arrayGameObject[i].GetComponent<POIDrops>().spriteUI = spriteTypeArmure;

                    for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().armuresList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().armuresList[j].tier<=tierPotentiel){
                            arrayGameObject[i].GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().armuresList[j]);
                        }

                    }  }
                break;

                case "Potion":
                for (int i = 0 ; i< arrayGameObject.Length; i++){
                                             arrayGameObject[i].GetComponent<POIDrops>().spriteUI = spriteTypePotion;

                    for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().potionsList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().potionsList[j].tier<= tierPotentiel){
                            arrayGameObject[i].GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().potionsList[j]);
                        }
                    }  
                    
                    
                    }
                break;

                case "Nourriture":
                for (int i = 0 ; i< arrayGameObject.Length; i++){
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().nourrituresList.Count; j++){
                                                                     arrayGameObject[i].GetComponent<POIDrops>().spriteUI = spriteTypeNourriture;

                        if(databaseObject.GetComponent<dataBaseV3>().nourrituresList[j].tier<= tierPotentiel){
                            arrayGameObject[i].GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().nourrituresList[j]);
                        }
                        
                    }  }
                break;

                case "Minerai":
                for (int i = 0 ; i< arrayGameObject.Length; i++){
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().mineraisList.Count; j++){
                                                                                             arrayGameObject[i].GetComponent<POIDrops>().spriteUI = spriteTypeMinerai;

                        if(databaseObject.GetComponent<dataBaseV3>().mineraisList[j].tier<=tierPotentiel){
                            arrayGameObject[i].GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().mineraisList[j]);
                        }

                    }  }
                break;

                case "Luxe":
                for (int i = 0 ; i< arrayGameObject.Length; i++){
                                                        arrayGameObject[i].GetComponent<POIDrops>().spriteUI = spriteTypeLuxe;

                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().luxesList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().luxesList[j].tier<= tierPotentiel){
                            arrayGameObject[i].GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().luxesList[j]);
                        }

                    }  }
                break;

                case "Plante":
                for (int i = 0 ; i< arrayGameObject.Length; i++){
                                    arrayGameObject[i].GetComponent<POIDrops>().spriteUI = spriteTypePlante;
    
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().plantesList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().plantesList[j].tier<= tierPotentiel){
                            arrayGameObject[i].GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().plantesList[j]);
                        }

                    }  }

                break;
            }

        }
}
