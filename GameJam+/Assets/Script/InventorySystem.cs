using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class InventorySaveData
{
    public List<string> items = new List<string>();
}

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    public int maxSlots = 5;
    private List<string> items = new List<string>();

    [Header("Assign all item sprites here in Inspector")]
    public List<ItemSpritePair> itemSprites = new List<ItemSpritePair>();

    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            savePath = Application.persistentDataPath + "/inventory.json";
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddItem(string itemName, Sprite icon)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventory penuh!");
            return false;
        }

        items.Add(itemName);

        if (UIInventory.Instance != null)
            UIInventory.Instance.AddItemToUI(icon);

        SaveInventory();
        return true;
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public List<string> GetItems()
    {
        return items;
    }

    public Sprite GetItemSprite(string itemName)
    {
        foreach (var pair in itemSprites)
        {
            if (pair.itemName == itemName) return pair.icon;
        }
        return null;
    }

    public void SaveInventory()
    {
        if (string.IsNullOrEmpty(savePath))
            savePath = Application.persistentDataPath + "/inventory.json";

        InventorySaveData data = new InventorySaveData();
        data.items = items;
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void LoadInventory()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("Tidak ada inventory, mulai baru.");
            return;
        }

        string json = File.ReadAllText(savePath);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

        items = data.items;
    }

    public void ClearInventory()
    {
        items.Clear();
        SaveInventory();
    }
}

[System.Serializable]
public class ItemSpritePair
{
    public string itemName;
    public Sprite icon;
}