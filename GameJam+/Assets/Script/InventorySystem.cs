using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class InventorySaveData
{
    public List<string> items = new List<string>();
}

[System.Serializable]
public class ItemSpritePair
{
    public string itemName;   // nama item, misal: "Jam Rusak"
    public Sprite icon;       // sprite/icon item
}

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [Header("Max slot inventori")]
    public int maxSlots = 5;

    // Nama-nama item yang dimiliki player
    private List<string> items = new List<string>();

    [Header("Assign semua item dan spritenya di Inspector")]
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

    // Versi utama: cukup kirim nama item, sprite diambil dari database
    public bool AddItem(string itemName)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventory penuh!");
            return false;
        }

        items.Add(itemName);
        Debug.Log($"[InventorySystem] Tambah item: {itemName}");

        // Ambil sprite dari database berdasarkan nama
        Sprite icon = GetItemSprite(itemName);

        // Update UI kalau UIInventory sudah ada di scene
        if (UIInventory.Instance != null)
        {
            UIInventory.Instance.AddItemToUI(icon);
        }

        SaveInventory();
        return true;
    }

    // Versi lama (kalau masih ada script yang pakai AddItem(name, sprite))
    // Parameter sprite diabaikan dan tetap pakai database
    public bool AddItem(string itemName, Sprite icon)
    {
        return AddItem(itemName);
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    // Kembalikan copy list supaya aman
    public List<string> GetItems()
    {
        return new List<string>(items);
    }

    public Sprite GetItemSprite(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
            return null;

        foreach (var pair in itemSprites)
        {
            if (pair == null) continue;

            if (pair.itemName == itemName)
            {
                if (pair.icon == null)
                {
                    Debug.LogWarning($"[InventorySystem] Icon NULL untuk item '{itemName}' di itemSprites");
                }
                return pair.icon;
            }
        }

        Debug.LogWarning($"[InventorySystem] Tidak menemukan sprite untuk item '{itemName}' di itemSprites");
        return null;
    }

    public void SaveInventory()
    {
        if (string.IsNullOrEmpty(savePath))
            savePath = Application.persistentDataPath + "/inventory.json";

        InventorySaveData data = new InventorySaveData();
        data.items = new List<string>(items);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("[InventorySystem] Inventory disimpan ke: " + savePath);
    }

    public void LoadInventory()
    {
        if (string.IsNullOrEmpty(savePath))
            savePath = Application.persistentDataPath + "/inventory.json";

        if (!File.Exists(savePath))
        {
            Debug.Log("[InventorySystem] Tidak ada file inventory, mulai baru.");
            return;
        }

        string json = File.ReadAllText(savePath);
        InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

        if (data != null && data.items != null)
        {
            items = data.items;
            Debug.Log($"[InventorySystem] Inventory loaded. Jumlah item: {items.Count}");
        }
        else
        {
            items = new List<string>();
            Debug.LogWarning("[InventorySystem] Data inventory kosong / rusak, mulai baru.");
        }
    }

    public void ClearInventory()
    {
        items.Clear();
        SaveInventory();
        Debug.Log("[InventorySystem] Inventory dibersihkan.");
    }
}
