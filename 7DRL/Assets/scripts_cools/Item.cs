using UnityEngine;
using System.Collections.Generic;
using System;
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]

public class Item : ScriptableObject
{

    public int id;
    public string itemName;
    public ItemType itemType;
    public enum ItemType{Arme, Armure, Potion, Nourriture, Minerai, Plante, Luxe};
    public  List<ItemType> typesItems = new List<ItemType>();

    public int valeurArme;
    public int valeurArmure;
    public int valeurPotion;
    public int valeurNourriture;
     public int valeurMinerai;
    public int valeurPlante;
    public int valeurLuxe;
    public Sprite sprite;
    public Sprite icon;
}
