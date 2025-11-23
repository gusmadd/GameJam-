using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Area1Controller : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public Light2D globalLight; // global light warna hitam
    public float lightFadeDuration = 1f;

    [Header("Interactables")]
    public GameObject windowObject;
    public PickupItem watchItem;

    private bool canPickWatch = false;
    private bool canClickWindow = false;
    public bool CanPickWatch => canPickWatch;
    public bool CanClickWindow => canClickWindow;

    [TextArea(2, 6)]
    public float startDelay = 2.2f;

    private string[] openingDialog = new string[]
    {
        "Dimana ini...?",
        "Kepalaku... terasa berat.",
        "Aku harus mencari tahu apa yang terjadi.",
        "Kamu ambilah jam itu, nadbfuhabff...."
    };

    void Start()
    {
        // Disable interaksi
        watchItem.enabled = false;
        windowObject.GetComponent<WindowInteract>().enabled = false;

        // Pastikan Global Light aktif, tapi intensitas awal rendah (terang)
        if (globalLight != null)
        {
            globalLight.intensity = 10f; // terang awal
            globalLight.gameObject.SetActive(true);
        }

        StartCoroutine(StartOpeningDialogueWithDelay());
    }

    private IEnumerator StartOpeningDialogueWithDelay()
    {
        yield return new WaitForSeconds(startDelay);

        // Fade in = terang → gelap (intensity naik)
        if (globalLight != null) yield return StartCoroutine(FadeLight(globalLight, 10f, 1f, lightFadeDuration));

        dialogueManager.StartDialogue(openingDialog, OnOpeningDialogueFinished);
    }

    private void OnOpeningDialogueFinished()
    {
        canPickWatch = true;
        watchItem.enabled = true;

        // Fade out = gelap → terang (intensity turun)
        if (globalLight != null) StartCoroutine(FadeLight(globalLight, 1f, 10f, lightFadeDuration));
    }

    public void OnWatchCollected()
    {
        watchItem.enabled = false;
        UIInventory.Instance.AddItemToUI(Resources.Load<Sprite>("Sprites/Items/broken_watch"));

        string[] afterWatch = {
            "Wah jam ini rusak... gimana ya.",
            "Sepertinya aku harus keluar dari ruangan ini."
        };

        // Fade in = terang → gelap
        if (globalLight != null) StartCoroutine(FadeLight(globalLight, 10f, 1f, lightFadeDuration));

        dialogueManager.StartDialogue(afterWatch, OnAfterWatchFinished);
    }

    private void OnAfterWatchFinished()
    {
        canClickWindow = true;
        windowObject.GetComponent<WindowInteract>().enabled = true;

        // Fade out = gelap → terang
        if (globalLight != null) StartCoroutine(FadeLight(globalLight, 1f, 10f, lightFadeDuration));
    }

    private IEnumerator FadeLight(Light2D light, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            light.intensity = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        light.intensity = end;
    }
}