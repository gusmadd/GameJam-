using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Area1Controller : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public Light2D globalLight; 
    public float lightFadeDuration = 1f;

    [Header("Interactables")]
    public GameObject windowObject;
    public PickupItem watchItem;

    private bool canPickWatch = false;
    private bool canClickWindow = false;
    public bool CanPickWatch => canPickWatch;
    public bool CanClickWindow => canClickWindow;

    void Awake()
    {
        // kalau belum di-assign di Inspector, cari otomatis di scene
        if (dialogueManager == null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();

            if (dialogueManager == null)
            {
                Debug.LogError("[Area1] Tidak menemukan DialogueManager di scene!");
            }
        }
    }

    [TextArea(2, 6)]
    public float startDelay = 2.2f;

    private string[] openingDialog = new string[]
    {
        "Where am I? What kind of place is this? Huh... these clothes... What’s happening in Wonderland?",
        "A clock? Whose clock is just lying there?"
    };

    void Start()
    {
        // 🔹 PAKSA UNPAUSE SETIAP KALI MASUK AREA 1
        Time.timeScale = 1f;

        Debug.Log("[Area1] Start() dipanggil");

        // Reset inventory untuk awal Area 1
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.ClearInventory();
        }

        // Disable interaksi awal
        if (watchItem != null)
        {
            watchItem.enabled = false;
            Debug.Log("[Area1] watchItem disabled di awal");
        }

        if (windowObject != null)
        {
            var wi = windowObject.GetComponent<WindowInteract>();
            if (wi != null)
            {
                wi.enabled = false;
                Debug.Log("[Area1] windowObject WindowInteract disabled di awal");
            }
        }

        // Set lampu global
        if (globalLight != null)
        {
            globalLight.intensity = 10f; // terang
            globalLight.gameObject.SetActive(true);
            Debug.Log("[Area1] globalLight aktif, intensity = 10");
        }

        StartCoroutine(StartOpeningDialogueWithDelay());
    }

    private IEnumerator StartOpeningDialogueWithDelay()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        if (globalLight != null)
            yield return StartCoroutine(FadeLight(globalLight, 10f, 1f, lightFadeDuration));

        if (dialogueManager == null)
            yield break;  // sudah di-log error di Awake

        dialogueManager.StartDialogue(openingDialog, OnOpeningDialogueFinished);
    }

    private void OnOpeningDialogueFinished()
    {
        Debug.Log("[Area1] Opening dialogue selesai, watch boleh diambil");

        canPickWatch = true;
        if (watchItem != null) watchItem.enabled = true;

        // Fade out = gelap → terang
        if (globalLight != null)
            StartCoroutine(FadeLight(globalLight, 1f, 10f, lightFadeDuration));
    }

    public void OnWatchCollected()
    {
        Debug.Log("[Area1] OnWatchCollected terpanggil");

        if (watchItem != null) watchItem.enabled = false;

        // Tambah ke UI inventory (ini hanya visual, logic inventory asli di InventorySystem)
        UIInventory.Instance.AddItemToUI(Resources.Load<Sprite>("Sprites/Items/broken_watch"));

        string[] afterWatch =
        {
            "It’s broken... but maybe I should keep it. It could be a clue in this strange world.",
            "First, I need to find a way out. There’s a window over there. Maybe looking through it will give me some answers."
        };

        // Fade in = terang → gelap
        if (globalLight != null)
            StartCoroutine(FadeLight(globalLight, 10f, 1f, lightFadeDuration));

        if (dialogueManager != null)
            dialogueManager.StartDialogue(afterWatch, OnAfterWatchFinished);
    }

    private void OnAfterWatchFinished()
    {
        Debug.Log("[Area1] AfterWatch dialogue selesai, window bisa diklik");

        canClickWindow = true;

        if (windowObject != null)
        {
            var wi = windowObject.GetComponent<WindowInteract>();
            if (wi != null) wi.enabled = true;
        }

        // Fade out = gelap → terang
        if (globalLight != null)
            StartCoroutine(FadeLight(globalLight, 1f, 10f, lightFadeDuration));
    }

    private IEnumerator FadeLight(Light2D light, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;   // pakai waktu yang TIDAK kena timeScale
            light.intensity = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        light.intensity = end;
    }
}
