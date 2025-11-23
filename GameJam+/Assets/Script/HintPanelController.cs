using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HintPanelController : MonoBehaviour
{
    [Header("References")]
    public GameObject panelRoot;   // HintPanel (full screen)
    public Image HintImage;        // Gambar hint di tengah
    public CanvasGroup canvasGroup;

    [Header("Animation")]
    public float animDuration = 0.3f;
    public Vector3 hiddenScale = new Vector3(0.8f, 0.8f, 1f);
    public Vector3 shownScale = Vector3.one;

    private bool isShowing = false;

    void Awake()
    {
        if (panelRoot == null)
            panelRoot = gameObject;

        if (canvasGroup == null)
            canvasGroup = panelRoot.GetComponent<CanvasGroup>();

        // PANEL SELALU AKTIF, tapi:
        // - alpha 0 (tak terlihat)
        // - tidak menerima klik
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (HintImage != null)
            HintImage.rectTransform.localScale = hiddenScale;
    }

    // dipanggil dari tombol Hint
    public void ShowHint(Sprite sprite)
    {
        if (isShowing) return;
        isShowing = true;

        if (HintImage != null && sprite != null)
            HintImage.sprite = sprite;

        // pause game
        Time.timeScale = 0f;

        // aktifkan interaksi panel
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        StartCoroutine(PlayShowAnimation());
    }

    // dipanggil dari klik area gelap
    public void HideHint()
    {
        if (!isShowing) return;
        StartCoroutine(PlayHideAnimation());
    }

    private IEnumerator PlayShowAnimation()
    {
        float t = 0f;
        if (HintImage != null)
            HintImage.rectTransform.localScale = hiddenScale;

        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / animDuration);

            canvasGroup.alpha = p;

            if (HintImage != null)
                HintImage.rectTransform.localScale = Vector3.Lerp(hiddenScale, shownScale, p);

            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator PlayHideAnimation()
    {
        float t = 0f;
        Vector3 startScale = (HintImage != null) ? HintImage.rectTransform.localScale : shownScale;

        while (t < animDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / animDuration);

            canvasGroup.alpha = 1f - p;

            if (HintImage != null)
                HintImage.rectTransform.localScale = Vector3.Lerp(startScale, hiddenScale, p);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Time.timeScale = 1f; // resume game
        isShowing = false;
    }
}
