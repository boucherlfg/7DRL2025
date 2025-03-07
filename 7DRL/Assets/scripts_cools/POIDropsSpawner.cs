using UnityEngine;
using UnityEngine.UI;
public class POIDropsSpawner : MonoBehaviour
{
    public GameObject spawnerPrefab;
    public GameObject databaseObject;
    public ItemType ressourceFavorisee;
    public ItemType ressourceDefavorisee;
    public ItemType[] ressourcesDisponibles;
    public Sprite[] spritesArmes;
    public Sprite[] spritesArmures;

    public Sprite[] spritesNourritures;

    public Sprite[] spritesPotions;

    public Sprite[] spritesPlantes;

    public Sprite[] spritesLuxes;

    public Sprite[] spritesMinerais;


    public int tierPotentiel;
    public GameObject[] spawnPoints;
    public GameObject ressourcesSpawner;
    public enum ItemType{Arme, Armure, Potion, Nourriture, Minerai, Plante, Luxe};

    void Start()
    {
        RessourcesDeFavSetup();
        RessourcesSpawn();
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

}
