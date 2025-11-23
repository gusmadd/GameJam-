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
        "Where am I? What kind of place is this? Huh... these clothes... What’s happening in Wonderland?",
        "A clock? Whose clock is just lying there?"
    };

    void Start()
    {
        InventorySystem.Instance.ClearInventory();
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
            "It’s broken... but maybe I should keep it. It could be a clue in this strange world.",
            "First, I need to find a way out. There’s a window over there. Maybe looking through it will give me some answers."
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