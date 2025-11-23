using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public static UIInventory Instance;

    [Header("Slot gambar item di UI")]
    public List<Image> slots = new List<Image>();

    private bool hasBuilt = false; // Flag untuk build UI sekali per scene

    void Awake()
    {
        Instance = this;
        ClearAllSlots();

        // Build UI dari InventorySystem kalau sudah ada
        RebuildUIFromData();
    }

    public void RebuildUIFromData()
    {
        if (hasBuilt) return; // Hanya sekali per scene
        if (InventorySystem.Instance == null) return;

        hasBuilt = true;

        if (InventorySystem.Instance.GetItems().Count > 0)
        {
            StartCoroutine(LoadIconsAndBuild());
        }
    }

    IEnumerator LoadIconsAndBuild()
    {
        // Tunggu 1 frame supaya semua komponen UI siap
        yield return null;

        if (InventorySystem.Instance == null) yield break;

        List<string> itemsToLoad = InventorySystem.Instance.GetItems();

        foreach (string item in itemsToLoad)
        {
            Sprite icon = InventorySystem.Instance.GetItemSprite(item);

            if (icon != null)
            {
                foreach (Image slot in slots)
                {
                    if (slot.sprite == null)
                    {
                        slot.sprite = icon;
                        slot.color = new Color(1, 1, 1, 1); // visible
                        break;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"UIInventory: Sprite null untuk item {item}");
            }
        }
    }

    // Dipanggil oleh InventorySystem saat item baru ditambah
    public void AddItemToUI(Sprite icon)
    {
        if (icon == null) return; // Safety check

        foreach (Image slot in slots)
        {
            if (slot.sprite == null)
            {
                slot.sprite = icon;
                slot.color = new Color(1, 1, 1, 1);

                StartCoroutine(InventoryPopEffect(slot.transform));
                return;
            }
        }
    }

    IEnumerator InventoryPopEffect(Transform slot)
    {
        Vector3 originalScale = slot.localScale;
        slot.localScale = Vector3.zero;

        float t = 0f;
        float duration = 0.3f;

        while (t < duration)
        {
            t += Time.deltaTime;
            slot.localScale = Vector3.Lerp(Vector3.zero, originalScale, t / duration);
            yield return null;
        }
    }

    public void ClearAllSlots()
    {
        foreach (var slot in slots)
        {
            if (slot == null) continue;
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0); // transparan
        }
        hasBuilt = false;
    }

    // == Helper buat akses berdasarkan index slot ==

    public string GetItemName(int index)
    {
        if (InventorySystem.Instance == null) return null;

        if (index >= 0 && index < slots.Count)
        {
            List<string> items = InventorySystem.Instance.GetItems();
            if (index < items.Count)
                return items[index];
        }
        return null;
    }

    public Sprite GetItemSprite(int index)
    {
        string itemName = GetItemName(index);
        if (string.IsNullOrEmpty(itemName)) return null;
        return InventorySystem.Instance.GetItemSprite(itemName);
    }
}
