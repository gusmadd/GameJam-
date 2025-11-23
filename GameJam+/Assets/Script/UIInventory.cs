using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public static UIInventory Instance;

    public List<Image> slots = new List<Image>();
    private bool hasBuilt = false; // Flag untuk rebuild UI sekali per scene

    void Awake()
    {
        Instance = this;
        ClearAllSlots();

        // Build UI dari InventorySystem jika ada
        RebuildUIFromData();
    }

    public void RebuildUIFromData()
    {
        if (hasBuilt) return; // Hanya sekali
        if (InventorySystem.Instance == null) return;

        hasBuilt = true;

        if (InventorySystem.Instance.GetItems().Count > 0)
        {
            StartCoroutine(LoadIconsAndBuild());
        }
    }

    IEnumerator LoadIconsAndBuild()
    {
        yield return null; // tunggu frame supaya semua komponen siap

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

    // Tambahkan item baru ke UI (dipanggil oleh InventorySystem)
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
            slot.sprite = null;
            slot.color = new Color(1, 1, 1, 0);
        }
        hasBuilt = false;
    }
}
