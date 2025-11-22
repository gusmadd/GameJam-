using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBoxAnimator : MonoBehaviour
{
    public RectTransform dialogueBox;
    public CanvasGroup canvasGroup;

    public float animDuration = 0.4f;
    public float slideDistance = 80f;

    Vector2 originalPos;

    void Awake()
    {
        originalPos = dialogueBox.anchoredPosition;
        canvasGroup.alpha = 0;
        dialogueBox.anchoredPosition = originalPos - new Vector2(0, slideDistance);
    }

    public IEnumerator PlayShowAnimation()
    {
        float t = 0;
        Vector2 startPos = originalPos - new Vector2(0, slideDistance);

        while (t < animDuration)
        {
            t += Time.deltaTime;

            float p = t / animDuration;

            dialogueBox.anchoredPosition = Vector2.Lerp(startPos, originalPos, p);
            canvasGroup.alpha = p;

            yield return null;
        }

        dialogueBox.anchoredPosition = originalPos;
        canvasGroup.alpha = 1;
    }

    public IEnumerator PlayHideAnimation()
    {
        float t = 0;
        Vector2 endPos = originalPos - new Vector2(0, slideDistance);

        while (t < animDuration)
        {
            t += Time.deltaTime;

            float p = t / animDuration;

            dialogueBox.anchoredPosition = Vector2.Lerp(originalPos, endPos, p);
            canvasGroup.alpha = 1 - p;

            yield return null;
        }

        canvasGroup.alpha = 0;
        dialogueBox.anchoredPosition = endPos;
    }
}
