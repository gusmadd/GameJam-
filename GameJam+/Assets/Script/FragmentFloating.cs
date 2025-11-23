using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentFloating : MonoBehaviour
{
    public string itemName = "Fragment1";
    public Sprite itemIcon;
    public float floatHeight = 0.15f; // Ketinggian naik-turun
    public float rotationSpeed = 40f; // Kecepatan rotasi Y

    private bool picked = false;
    private Vector3 startPos;
    private Vector3 initialScale;
    private SpriteRenderer sr; // Tambahkan SpriteRenderer

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // Pastikan skala awal tersimpan sebelum animasi spawn mengubahnya
        initialScale = transform.localScale;

        // Atur posisi awal di Awake, karena Start dipanggil belakangan
        startPos = transform.position;
    }

    // Metode ini dipanggil dari PuzzleManager setelah Instantiate
    public void PlaySpawnAnimation()
    {
        StartCoroutine(SpawnAndFloat());
    }

    IEnumerator SpawnAndFloat()
    {
        float duration = 0.5f;
        float t = 0f;

        // 1. Animasi muncul (Skala dari nol ke normal, kedip)
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            // Skala dari nol ke normal
            transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, p);

            // Kedip-kedip (Flicker/Pulse Warna)
            float flicker = Mathf.Sin(t * 20f) * 0.5f + 0.5f; // Nilai 0 sampai 1
            sr.color = new Color(flicker, flicker, flicker, 1f); // Warna berkedip

            yield return null;
        }

        transform.localScale = initialScale; // Pastikan skala tepat
        sr.color = Color.white; // Kembalikan warna normal setelah kedip

        // Langsung masuk ke floating idle
        StartCoroutine(FloatingIdle());
    }

    IEnumerator FloatingIdle()
    {
        float t = 0;

        while (!picked)
        {
            t += Time.deltaTime;

            // 1. Naik turun pelan
            transform.position = startPos + new Vector3(0, Mathf.Sin(t * 2f) * floatHeight, 0);

            // 2. Agak membesar/mengecil (Pulse Scale Kecil)
            float scalePulse = 1f + Mathf.Sin(t * 4f) * 0.05f;
            transform.localScale = initialScale * scalePulse;

            // 3. Kedip-kedip warna
            float flicker = Mathf.Sin(t * 10f) * 0.5f + 0.5f;
            sr.color = new Color(flicker, flicker, flicker, 1f);

            yield return null;
        }
    }

    private void OnMouseDown()
    {
        if (picked) return;
        picked = true;

        // Hentikan coroutine idle (agar tidak bertabrakan)
        StopCoroutine(FloatingIdle());

        StartCoroutine(PickupSequence());
    }

    IEnumerator PickupSequence()
    {
        float duration = 0.8f;
        float t = 0;

        // Target di tengah layar (gunakan Z = 0 atau 1 untuk 2D)
        Vector3 targetCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 1));
        Vector3 start = transform.position;
        Vector3 peakScale = initialScale * 2f; // Puncak pembesaran saat di tengah

        // 1. Terbang ke tengah layar (Membesar dan Berputar)
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            // Menggunakan kurva yang lebih halus (Ease-in-out)
            float smoothStep = Mathf.SmoothStep(0.0f, 1.0f, p);

            transform.position = Vector3.Lerp(start, targetCenter, smoothStep);
            transform.localScale = Vector3.Lerp(initialScale, peakScale, smoothStep);

            // Rotasi cepat
            transform.Rotate(0, 0, 360 * Time.deltaTime);

            yield return null;
        }

        // 2. Jeda di tengah
        yield return new WaitForSeconds(0.2f);

        // 3. Turun Mengecil dan Menghilang (Animasi Masuk ke Inventory)
        float hideDuration = 0.4f;
        t = 0;
        while (t < hideDuration)
        {
            t += Time.deltaTime;
            float p = t / hideDuration;

            // Bergerak sedikit ke bawah (meniru "masuk" ke UI/Inventory)
            transform.position = targetCenter + new Vector3(0, Mathf.Lerp(0, -0.5f, p), 0);

            // Mengecil menuju nol
            transform.localScale = Vector3.Lerp(peakScale, Vector3.zero, p);

            // Putar lagi sedikit
            transform.Rotate(0, 0, 180 * Time.deltaTime);

            yield return null;
        }

        // 4. Masuk ke Inventory dan Beritahu Manager
        // Pastikan InventorySystem Anda ada dan fungsinya benar
        InventorySystem.Instance.AddItem(itemName, itemIcon);
        MushroomPuzzleManager.Instance.OnFragmentCollected();

        Destroy(gameObject);
    }
}