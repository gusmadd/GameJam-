// REVISED SCRIPT: BlackPortal.cs
using UnityEngine;
using System.Collections;

public class BlackPortal : MonoBehaviour
{
    // Ganti Image dengan SpriteRenderer
    public SpriteRenderer portalRenderer;

    private Vector3 initialScale;

    void Start()
    {
        // Pastikan SpriteRenderer terpasang, jika tidak, dapatkan secara otomatis
        if (portalRenderer == null)
        {
            portalRenderer = GetComponent<SpriteRenderer>();
        }

        if (portalRenderer != null)
        {
            // Set warna awal (transparan) dan simpan skala awal
            portalRenderer.color = new Color(1, 1, 1, 0);
            initialScale = transform.localScale;
            transform.localScale = Vector3.zero;
        }

        // Nonaktifkan objek secara default (akan diaktifkan oleh Area5Controller)
        gameObject.SetActive(false);
    }

    public void OpenPortal()
    {
        // Aktifkan GameObject induk
        gameObject.SetActive(true);
        StartCoroutine(PortalAppear());
    }

    IEnumerator PortalAppear()
    {
        if (portalRenderer == null) yield break; // Safety check

        float duration = 1f;
        float t = 0f;

        // Skala akhir yang diinginkan, menggunakan initialScale yang sudah disimpan
        Vector3 startScale = Vector3.zero;

        // Gunakan initialScale yang sudah diset di editor.
        Vector3 endScale = initialScale * 1.2f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            // Fade in alpha (Asumsi warna Sprite adalah putih/warna terang)
            float a = Mathf.Lerp(0f, 1f, p);
            portalRenderer.color = new Color(1, 1, 1, a);

            // Animasi skala
            transform.localScale = Vector3.Lerp(startScale, endScale, p);

            yield return null;
        }
    }
}