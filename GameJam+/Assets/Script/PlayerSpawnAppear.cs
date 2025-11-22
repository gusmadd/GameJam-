using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnAppear : MonoBehaviour
{
    public float fadeDuration = 0.8f;
    public float popScale = 1.15f;

    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // mulai dengan player invisible
        Color c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, 0f);

        // mulai animasi muncul
        StartCoroutine(AppearAnimation());
    }

    IEnumerator AppearAnimation()
    {
        float t = 0f;

        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 0.85f; // sedikit mengecil dulu

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float progress = t / fadeDuration;

            // fade-in
            float alpha = Mathf.Lerp(0f, 1f, progress);
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, alpha);

            // scale pop (membesar lalu kembali)
            if (progress < 0.5f)
            {
                transform.localScale = Vector3.Lerp(originalScale * 0.85f, originalScale * popScale, progress * 2f);
            }
            else
            {
                transform.localScale = Vector3.Lerp(originalScale * popScale, originalScale, (progress - 0.5f) * 2f);
            }

            yield return null;
        }

        // pastikan final fix
        transform.localScale = originalScale;
    }
}
