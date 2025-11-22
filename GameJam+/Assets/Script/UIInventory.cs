using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public static UIInventory Instance;

    public List<Image> slots = new List<Image>();

    void Awake()
    {
        Instance = this;
    }

    // Panggil ini ketika item ditambah
    public void AddItemToUI(Sprite icon)
    {
        foreach (Image slot in slots)
        {
            if (slot.sprite == null)
            {
                slot.sprite = icon;
                slot.color = new Color(1, 1, 1, 1); // visible

                // animasi pop
                StartCoroutine(InventoryPopEffect(slot.transform));
                return;
            }
        }
    }

    IEnumerator InventoryPopEffect(Transform slot)
    {
        Vector3 originalScale = slot.localScale;
        slot.localScale = Vector3.zero;

        float t = 0;
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
            slot.color = new Color(1, 1, 1, 0); // invisible
        }
    }
}
