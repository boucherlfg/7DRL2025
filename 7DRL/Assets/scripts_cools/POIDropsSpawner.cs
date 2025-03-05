using UnityEngine;

public class POIDropsSpawner : MonoBehaviour
{
    public GameObject spawnerPrefab;
    public GameObject databaseObject;
    public ItemType ressourceFavorisee;
    public ItemType ressourceDefavorisee;
    public ItemType[] ressourcesDisponibles;
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
                    }
                break;

                case "Armure":
                    for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().armuresList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().armuresList[j].tier<=tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().armuresList[j]);
                        }
                    }
                break;

                case "Potion":
                    for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().potionsList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().potionsList[j].tier<= tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().potionsList[j]);
                        }
                    }
                break;

                case "Nourriture":
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().nourrituresList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().nourrituresList[j].tier<= tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().nourrituresList[j]);
                        }
                    }
                break;

                case "Minerai":
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().mineraisList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().mineraisList[j].tier<=tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().mineraisList[j]);
                        }
                    }
                break;

                case "Luxe":
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().luxesList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().luxesList[j].tier<= tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().luxesList[j]);
                        }
                    }
                break;

                case "Plante":
                     for(int j = 0 ; j < databaseObject.GetComponent<dataBaseV3>().plantesList.Count; j++){
                        if(databaseObject.GetComponent<dataBaseV3>().plantesList[j].tier<= tierPotentiel){
                            ressourcesSpawner.GetComponent<POIDrops>().dropsPossibles.Add(databaseObject.GetComponent<dataBaseV3>().plantesList[j]);
                        }
                    }

                break;
            }
        //ressourcesSpawner.GetComponent<POIDrops>.dropsPossibles










}

}

}
