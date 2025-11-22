using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPuzzleItem : MonoBehaviour
{
    public int mushroomID; // 0 = tengah, 1 = kanan, 2 = kiri

    private SpriteRenderer sr;
    private bool solved = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnMouseDown()
    {
        if (solved) return;

        MushroomPuzzleManager.Instance.OnMushroomClicked(mushroomID, this);
    }

    // ========= ANIMASI BENAR =========
    public void PlayCorrectAnimation()
    {
        solved = true;
        StartCoroutine(CorrectAnim());
    }

    IEnumerator CorrectAnim()
    {
        Vector3 startPos = transform.position;
        Vector3 upPos = startPos + new Vector3(0, 0.3f, 0);

        Vector3 originalScale = transform.localScale;
        Vector3 bigScale = originalScale * 1.3f;

        float t = 0f;
        float duration = 0.6f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            // naik
            transform.position = Vector3.Lerp(startPos, upPos, p);

            // membesar lalu sedikit goyang
            float shake = Mathf.Sin(p * 20f) * 0.03f;
            transform.localScale = Vector3.Lerp(originalScale, bigScale, p) + new Vector3(shake, shake, 0);

            yield return null;
        }

        // mengecil
        t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(bigScale, Vector3.zero, t / 0.3f);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    // ========= ANIMASI SALAH =========
    public void PlayWrongAnimation()
    {
        StartCoroutine(WrongAnim());
    }

    IEnumerator WrongAnim()
    {
        Color originalColor = sr.color;

        sr.color = new Color(1f, 0.4f, 0.4f, originalColor.a);

        Vector3 originalScale = transform.localScale;
        Vector3 bigScale = originalScale * 1.15f;
        Vector3 originalPos = transform.localPosition;

        float t = 0f;
        float duration = 0.25f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float shake = Mathf.Sin(t * 60f) * 0.03f;

            // posisi tidak bergeser tetap di originalPos
            transform.localPosition = originalPos + new Vector3(shake, 0, 0);

            transform.localScale = Vector3.Lerp(originalScale, bigScale, t / duration);
            yield return null;
        }

        t = 0f;
        while (t < 0.15f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(bigScale, originalScale, t / 0.15f);
            yield return null;
        }

        transform.localScale = originalScale;
        transform.localPosition = originalPos;
        sr.color = originalColor;
    }
}
