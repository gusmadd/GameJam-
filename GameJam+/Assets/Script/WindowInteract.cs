using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowInteract : MonoBehaviour
{
    public string nextSceneName = "Area2";
    [HideInInspector]
    public bool canClick = true;

    public SpriteRenderer playerSprite;   // <-- tambahkan reference ke player

    private void OnMouseDown()
    {
        var area1 = FindObjectOfType<Area1Controller>();
        if (area1 != null && !area1.CanClickWindow) return;
        if (!canClick) return;

        canClick = false;
        StartCoroutine(ClickAnimation());
    }

    private IEnumerator ClickAnimation()
    {
        Vector3 originalScale = transform.localScale;

        // animasi jendela membesar
        transform.localScale = originalScale * 1.1f;
        yield return new WaitForSeconds(0.08f);

        transform.localScale = originalScale;
        yield return new WaitForSeconds(0.1f);

        // setelah animasi jendela → fade out player
        yield return StartCoroutine(FadeOutPlayer());
        if (MushroomPuzzleManager.Instance != null)
            MushroomPuzzleManager.Instance.StopAllCoroutines();

        TransitionManager.Instance.FadeOutAndLoadScene(nextSceneName);
    }

    private IEnumerator FadeOutPlayer()
    {
        float duration = 1.0f;
        float t = 0f;

        Color originalColor = playerSprite.color;

        while (t < duration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            playerSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }
    }
}