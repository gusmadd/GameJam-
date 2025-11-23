using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area3Fragment : MonoBehaviour
{
    // ===============================================
    // VARIABEL UNTUK ITEM DAN KONEKSI KE AREA3CONTROLLER
    public string fragmentItemName = "Fragment2"; // Nama item di inventory
    public Sprite fragmentItemIcon;              // Icon item di inventory
    private Area3Controller areaController;      // Referensi ke Area3Controller

    // ===============================================
    // VARIABEL ANIMASI (SAMA PERSIS DENGAN FragmentFloating.cs)
    public float floatHeight = 0.15f;    // Ketinggian naik-turun
    public float rotationSpeed = 40f;    // Kecepatan rotasi Y (jika ada rotasi di Y)
    // Untuk rotasi 2D, mungkin Anda ingin menggunakan Z
    public float zRotationSpeed = 360f; // Kecepatan rotasi Z untuk 2D

    private bool picked = false;
    private Vector3 startPos;
    private Vector3 initialScale;
    private SpriteRenderer sr; // Komponen SpriteRenderer untuk kontrol visual

    // ===============================================
    // AWAKE & START
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // Pastikan skala awal tersimpan sebelum animasi spawn mengubahnya
        initialScale = transform.localScale;

        // Atur posisi awal di Awake, karena Start dipanggil belakangan
        startPos = transform.position;
    }

    // ===============================================
    // FUNGSI KOMUNIKASI (Dipanggil oleh Area3Controller saat Instantiate)
    public void SetAreaController(Area3Controller controller)
    {
        areaController = controller;
    }

    // ===============================================
    // ANIMASI SPAWN DAN FLOATING (SAMA PERSIS DENGAN FragmentFloating.cs)
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
            
            // Rotasi Z (untuk 2D)
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime); // <-- Gunakan rotationSpeed untuk Z

            yield return null;
        }
    }

    // ===============================================
    // DETEKSI KLIK (Dipanggil saat pemain klik objek)
    private void OnMouseDown()
    {
        if (picked) return;
        picked = true;

        // Pastikan AreaController sudah di-assign
        if (areaController == null)
        {
            Debug.LogError("ERROR: Area3Controller belum di-assign ke Area3Fragment!");
            // Jika tidak ada controller, mungkin kita bisa langsung tambahkan ke inventory
            // InventorySystem.Instance.AddItem(fragmentItemName, fragmentItemIcon);
            Destroy(gameObject); // Hapus objek
            return;
        }

        StopCoroutine(FloatingIdle()); // Hentikan animasi idle
        StartCoroutine(PickupSequence());
    }

    // ===============================================
    // ANIMASI PICKUP DAN KOMUNIKASI (SAMA PERSIS DENGAN FragmentFloating.cs)
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

            float smoothStep = Mathf.SmoothStep(0.0f, 1.0f, p);

            transform.position = Vector3.Lerp(start, targetCenter, smoothStep);
            transform.localScale = Vector3.Lerp(initialScale, peakScale, smoothStep);

            transform.Rotate(0, 0, zRotationSpeed * Time.deltaTime); // <-- Rotasi Z
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

            transform.position = targetCenter + new Vector3(0, Mathf.Lerp(0, -0.5f, p), 0);
            transform.localScale = Vector3.Lerp(peakScale, Vector3.zero, p);
            transform.Rotate(0, 0, zRotationSpeed * 0.5f * Time.deltaTime); // Rotasi lebih pelan
            yield return null;
        }

        // 4. Masuk ke Inventory dan Beritahu Area3Controller
        // Panggil fungsi di Area3Controller untuk pickup dan transisi
        areaController.CollectFragmentAndProceed(gameObject, fragmentItemName, fragmentItemIcon); 
        
        // Objek akan di-Destroy oleh Area3Controller setelah diproses
        yield break; 
    }
}