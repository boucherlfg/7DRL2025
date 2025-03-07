using UnityEngine;
using System.Collections.Generic;
using System;
public class Singleton<T> where T : Singleton<T>, new() {
    private static T _instance;
    public static T Instance => _instance ??= new(); 
}

public class GestionRessourcesConcreteSingleton : Singleton<GestionRessourcesConcreteSingleton> {
    public int quantiteRessourcesTotal = 0;
    public int quantiteRessourcesArme = 0;
    public int quantiteRessourcesArmure = 0;
    public int quantiteRessourcesNourriture = 0;
    public int quantiteRessourcesMinerai = 0;
    public int quantiteRessourcesPotion = 0;
    public int quantiteRessourcesPlante = 0;
    public int quantiteRessourcesLuxe = 0;
    public int tierActuel = 0;
    public  List<Item> listSac = new List<Item>();
    

}