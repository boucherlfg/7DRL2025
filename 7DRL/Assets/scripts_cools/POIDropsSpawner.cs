using UnityEngine;

public class POIDropsSpawner : MonoBehaviour
{
    public GameObject spawnerPrefab;
    public GameObject databaseObject;
    public ItemType ressourceFavorisee;
    public ItemType ressourceDefavorisee;
    public ItemType[] ressourcesDisponibles;
    public GameObject[] spawnPoints;

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
        for (int i = 0; i < ressourcesDisponibles.Length ; i++){
            public GameObject ressourcesSpawner = Instantiate (spawnerPrefab, spawnPoints[i].transform);
            switch (ressourcesDisponibles[i].ToString())
            {
                 case "Arme":
                 
                    for(int j =0 ; j<    databaseObject.GetComponent<database>.armesList.Count; j++){
                        if(databaseObject.GetComponent<database>.armesList[j].tier<=databaseObject.GetComponent<database>.tierActuel){
                            ressourcesSpawner.GetComponent<POIDrops>.dropsPossibles.Add(databaseObject.GetComponent<database>.armesList[j]);
                        }
                    }
                    Debug.Log(ressourcesSpawner.GetComponent<POIDrops>.dropsPossibles.Count)
                break;

                case "Armure":
                break;

                case "Potion":
                break;

                case "Nourriture":
                break;

                case "Minerai":
                break;

                case "Luxe":
                break;

                case "Plante":

                break;
            }
        //ressourcesSpawner.GetComponent<POIDrops>.dropsPossibles










}

}

}
