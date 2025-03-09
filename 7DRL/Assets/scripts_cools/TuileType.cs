using UnityEngine;

[CreateAssetMenu(fileName = "TuileType", menuName = "Scriptable Objects/TuileType")]
public class TuileType : ScriptableObject
{


    public ItemType itemType;
    public enum ItemType { Arme, Armure, Potion, Nourriture, Minerai, Plante, Luxe };
    public int quantite;
    public int quantiteEnJeu;
    public int valeur;
    public Sprite sprite;
}
