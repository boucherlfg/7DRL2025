using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

[System.Serializable]
public class ItemData
{
    public string name;
    public string type;
    public int tier;
    public int value;
}

[System.Serializable]
public class ItemList
{
    public List<ItemData> items;
}

public class JsonToScriptable : MonoBehaviour
{
    private static string jsonPath = "Assets/SO/items.json"; // Emplacement du fichier JSON
    private static string savePath = "Assets/SO/Items/"; // Dossier de sauvegarde des ScriptableObjects

    [MenuItem("Tools/Import JSON to ScriptableObjects")]
    public static void ImportJson()
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("Fichier JSON non trouvé : " + jsonPath);
            return;
        }

        string jsonText = File.ReadAllText(jsonPath);
        ItemList itemList = JsonUtility.FromJson<ItemList>(jsonText);

        if (itemList == null || itemList.items == null)
        {
            Debug.LogError("Erreur lors de la lecture du JSON !");
            return;
        }

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        foreach (ItemData itemData in itemList.items)
        {
            CreateScriptableObject(itemData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Importation terminée avec succès !");
    }

    private static void CreateScriptableObject(ItemData data)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.itemName = data.name;
        newItem.itemType = ConvertItemType(data.type);
        newItem.tier = data.tier;
        newItem.valeur = data.value;

        string fileName = $"{savePath}{data.name.Replace(" ", "_")}.asset";
        AssetDatabase.CreateAsset(newItem, fileName);
        Debug.Log("Créé : " + fileName);
    }

    private static Item.ItemType ConvertItemType(string type)
    {
        return type.ToLower() switch
        {
            "arme" => Item.ItemType.Arme,
            "armure" => Item.ItemType.Armure,
            "potion" => Item.ItemType.Potion,
            "nourriture" => Item.ItemType.Nourriture,
            "minerai" => Item.ItemType.Minerai,
            "plante" => Item.ItemType.Plante,
            "luxe" => Item.ItemType.Luxe,
            _ => throw new System.Exception("Type inconnu: " + type),
        };
    }
}
