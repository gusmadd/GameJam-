using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public string itemName = "Jam Rusak"; // jam rusak
    public Sprite icon;

    private bool picked = false;

    void OnMouseDown()
    {
        if (picked) return;
        if (!FindObjectOfType<Area1Controller>().CanPickWatch) return;

        bool added = InventorySystem.Instance.AddItem(itemName, icon);

        if (added)
        {
            picked = true;
            StartCoroutine(PickupAnimation());
        }
    }

    IEnumerator PickupAnimation()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + new Vector3(0, 0.4f, 0); // naik 0.4
        Vector3 originalScale = transform.localScale;

        float t = 0f;
        float duration = 0.8f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float progress = t / duration;

            // 1. Menjadi gelap saat diklik
            if (sr != null)
            {
                float dark = Mathf.Lerp(1f, 0.70f, progress * 1.2f);
                sr.color = new Color(dark, dark, dark, 1f);
            }

            // 2. Bergerak naik
            transform.position = Vector3.Lerp(startPos, targetPos, progress);

            // 3. Scale up → down
            if (progress < 0.5f)
            {
                // membesar di setengah pertama
                transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.25f, progress * 2f);
            }
            else
            {
                // mengecil di setengah kedua
                float p = (progress - 0.5f) * 2f;
                transform.localScale = Vector3.Lerp(originalScale * 1.25f, Vector3.zero, p);
            }

            yield return null;
        }

        // hilang
        gameObject.SetActive(false);
        FindObjectOfType<Area1Controller>().OnWatchCollected();
    }
}
