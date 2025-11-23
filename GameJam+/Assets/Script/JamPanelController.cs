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
    public float stepAngle = 5f;
    public float targetAngle = 150f;  // Jam 7 (150 derajat)
    public float startAngle = 180f;   // Jam 6 (180 derajat)
    public float tolerance = 2f;

    private bool jarumActive = false;

    void Start()
    {
        // Pastikan jarum dan panel aktif sesuai kebutuhan
        panel.SetActive(false);
        jarumImage.gameObject.SetActive(false);

        // Assign listener klik ke button transparan
        clickButton.onClick.AddListener(OnJarumClick);
    }

    // ================= Buka panel jam =================
    public void OpenPanel(Sprite jamSprite)
    {
        panel.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Set jarum di posisi awal jam 6
        jarumImage.gameObject.SetActive(true);
        jarumImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90f);
        jarumActive = true;
    }

    // ================= Klik-klik jarum =================
    public void OnJarumClick()
    {
        if (!jarumActive) return;

        float currentAngle = jarumImage.rectTransform.localEulerAngles.z;

        // Putar jarum mundur (dari 180 ke 150)
        currentAngle -= stepAngle;

        // Pastikan jarum tidak melewati target 150 derajat
        // Batas bawah (Jam 7) adalah 150, batas atas (Jam 6) adalah 180.
        currentAngle = Mathf.Clamp(currentAngle, targetAngle, startAngle);

        jarumImage.rectTransform.localEulerAngles = new Vector3(0, 0, currentAngle);

        // Cek apakah sudah mencapai target (150 derajat)
        if (Mathf.Abs(currentAngle - targetAngle) <= tolerance)
        {
            jarumImage.rectTransform.localEulerAngles = new Vector3(0, 0, targetAngle); // snap
            Debug.Log("Puzzle Selesai! Jam diatur ke Jam 7.");
            StartCoroutine(FadeOutPanel());
            jarumActive = false;

            // Tambahkan fungsi untuk melanjutkan game/memberi hadiah di sini
            // Misalnya: FindObjectOfType<Area3Controller>().GiveFinalItem(); 
        }
    }

    // ================= Fade out panel =================
    private IEnumerator FadeOutPanel()
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
    }
}