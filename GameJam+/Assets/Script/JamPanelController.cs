using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JamPanelController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;          // panel jam
    public Image jarumImage;          // jarum pendek
    public CanvasGroup canvasGroup;   // untuk fade out panel
    public Button clickButton;        // button transparan di tengah jam

    [Header("Settings")]
    public float stepAngle = 5f;      // derajat per klik
    public float targetAngle = 30f;   // jam 7
    public float tolerance = 2f;      // toleransi ±2°

    private bool jarumActive = false;

    [Header("Inventory Reference")]
    public InventorySystem inventorySystem;
    public UIInventory uiInventory;

    // Callback untuk memberi tahu controller (misal Area3Controller) setelah panel ditutup
    public System.Action OnPanelClosed;

    // ================= Start =================
    void Start()
    {
        panel.SetActive(false);
        jarumImage.gameObject.SetActive(false);

        // Assign listener klik button
        if (clickButton != null)
            clickButton.onClick.AddListener(OnJarumClick);

        // Safety check inventory
        if (inventorySystem == null) inventorySystem = InventorySystem.Instance;
        if (uiInventory == null) uiInventory = UIInventory.Instance;
    }

    // ================= Buka Panel =================
    public void OpenPanel(Sprite jamSprite)
    {
        panel.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // HAPUS AKTIVASI JARUM DI SINI:
        // jarumImage.gameObject.SetActive(true); 

        // Pastikan jarum TIDAK aktif saat panel pertama dibuka
        jarumImage.gameObject.SetActive(false);
        jarumActive = false; // Jarum belum dipasang

        // Posisi awal jarum (Hanya mengatur posisi jika sudah aktif, tapi sebaiknya di ActivateJarum)
        // jarumImage.rectTransform.localEulerAngles = new Vector3(0, 0, 180f); 
    }
    // ================= Klik-klik jarum =================
    public void OnJarumClick()
    {
        if (!jarumActive) return;

        float currentAngle = jarumImage.rectTransform.localEulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360f;

        currentAngle -= stepAngle; // arah searah jarum jam
        currentAngle = Mathf.Clamp(currentAngle, targetAngle, 180f); // 180° = jam 6

        jarumImage.rectTransform.localEulerAngles = new Vector3(0, 0, currentAngle);

        // cek target jam 7
        if (Mathf.Abs(currentAngle - targetAngle) <= tolerance)
        {
            jarumImage.rectTransform.localEulerAngles = new Vector3(0, 0, targetAngle);
            jarumActive = false;
            StartCoroutine(FadeOutPanelAndRemoveItems());
        }
    }

    // ================= Fade out panel dan hapus item =================
    private IEnumerator FadeOutPanelAndRemoveItems()
    {
        float t = 0f;
        float duration = 0.5f;

        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        panel.SetActive(false);
        jarumImage.gameObject.SetActive(false);
        canvasGroup.blocksRaycasts = false;

        // Hapus item inventory
        if (inventorySystem != null)
        {
            inventorySystem.GetItems().Remove("Jam Rusak");
            inventorySystem.GetItems().Remove("Jarum Pendek");
            inventorySystem.SaveInventory();

            if (uiInventory != null)
            {
                uiInventory.ClearAllSlots();
                uiInventory.RebuildUIFromData();
            }
        }

        // Panggil callback kalau ada
        OnPanelClosed?.Invoke();
    }

    // ================= Pasang jarum (dipanggil dari Area3Controller) =================
    public void PlaceJarum(Sprite jarumSprite)
    {
        // Buka panel jam
        OpenPanel(jarumSprite);

        // jarum sudah aktif di OpenPanel
        // klik-klik tetap via button transparan
    }
    public void ActivateJarum(Sprite jarumSprite)
    {
        jarumImage.sprite = jarumSprite;
        jarumImage.gameObject.SetActive(true); // <-- BARU AKTIF DI SINI
        jarumImage.rectTransform.localEulerAngles = new Vector3(0, 0, 180f); // posisi awal jam 6
        jarumActive = true;

        // Pastikan panel terbuka jika belum
        panel.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
