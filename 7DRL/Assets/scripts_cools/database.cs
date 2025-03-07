using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class dataBaseV3 : MonoBehaviour
{

    public List<Item> itemsList = new();
    public List<Item> armesList = new();
    public List<Item> armuresList = new();
    public List<Item> nourrituresList = new();
    public List<Item> mineraisList = new();
    public List<Item> potionsList = new();
    public List<Item> plantesList = new();
    public List<Item> luxesList = new();


    private void Awake()
    {   
        for(int i = 0; i < itemsList.Count; i++) {
            itemsList[i].id = i;
            switch(itemsList[i].itemType.ToString()){
                case "Arme":
                    armesList.Add(itemsList[i]);
                break;

                case "Armure":
                    armuresList.Add(itemsList[i]);
                break;

                case "Potion":
                    potionsList.Add(itemsList[i]);
 
                break;

                case "Nourriture":
                    nourrituresList.Add(itemsList[i]);  
                break;

                case "Minerai":
                    mineraisList.Add(itemsList[i]);
                break;

                case "Luxe":
                    luxesList.Add(itemsList[i]);
                break;

                case "Plante":
                  plantesList.Add(itemsList[i]);
                break;
                default:
                    Debug.Log("NOOOOON");
                    break;
            }
        }
    }
}
    


