using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Clients", menuName = "Scriptable Objects/Clients")]
public class Clients : ScriptableObject
{
    public enum clientType { villageois, Chevalier, Sorcier, Forgeron, Agriculteur};
    public clientType clienType;

    public Sprite sprite;
    public List<TuileType> itemDemander;

    public int nbEssais = 0;
    public int nbEssaiMax = 3;
}
