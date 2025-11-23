using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public Area5Controller controller;
    public Image[] cardImages; // UI cards
    public int[] correctCards = { 1, 2, 6, 3 };

    private bool[] selected;
    private int correctCount = 0;

    void Start()
    {
        selected = new bool[cardImages.Length];

        foreach (var img in cardImages)
        {
            img.color = Color.white;
        }
    }

    public void EnablePuzzle()
    {
        correctCount = 0;
        for (int i = 0; i < selected.Length; i++)
            selected[i] = false;
    }

    public void ClickCard(int index)
    {
        if (selected[index]) return;

        selected[index] = true;

        // animasi benar/salah
        bool isCorrect = false;
        foreach (int id in correctCards)
        {
            if (id == index) isCorrect = true;
        }

        if (isCorrect)
        {
            StartCoroutine(CorrectCardEffect(cardImages[index]));
            correctCount++;

            if (correctCount == correctCards.Length)
            {
                controller.ClosePuzzleAndTalk();
            }
        }
        else
        {
            StartCoroutine(WrongCardEffect(cardImages[index]));
            StartCoroutine(ResetPuzzle());
        }
    }

    // =========================
    // ANIMASI KARTU BENAR
    // =========================
    IEnumerator CorrectCardEffect(Image img)
    {
        Color startColor = img.color;
        Color highlight = new Color(0.2f, 1f, 0.2f); // hijau terang

        Vector3 startScale = img.transform.localScale;
        Vector3 bigScale = startScale * 1.25f;

        float t = 0f;

        // Glow hijau + membesar
        while (t < 0.2f)
        {
            t += Time.deltaTime * 5f;
            img.transform.localScale = Vector3.Lerp(startScale, bigScale, t);
            img.color = Color.Lerp(startColor, highlight, t);
            yield return null;
        }

        // Bounce balik sedikit
        t = 0f;
        Vector3 smallBack = startScale * 1.1f;

        while (t < 0.15f)
        {
            t += Time.deltaTime * 4f;
            img.transform.localScale = Vector3.Lerp(bigScale, smallBack, t);
            yield return null;
        }

        // selesai → tetap hijau & sedikit membesar
    }

    // =========================
    // ANIMASI KARTU SALAH
    // =========================
    IEnumerator WrongCardEffect(Image img)
    {
        Color original = img.color;
        Color wrong = new Color(1f, 0.3f, 0.3f);

        // Blink merah 2x
        for (int i = 0; i < 2; i++)
        {
            img.color = wrong;
            yield return new WaitForSeconds(0.1f);
            img.color = original;
            yield return new WaitForSeconds(0.1f);
        }

        // Shake
        RectTransform rt = img.rectTransform;
        Vector3 originalPos = rt.anchoredPosition;

        float shakeAmt = 10f;
        float t = 0f;

        while (t < 0.2f)
        {
            t += Time.deltaTime * 10f;
            rt.anchoredPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeAmt;
            yield return null;
        }

        rt.anchoredPosition = originalPos;
    }

    // =========================
    // RESET PUZZLE SETELAH SALAH
    // =========================
    IEnumerator ResetPuzzle()
    {
        yield return new WaitForSeconds(0.4f);

        for (int i = 0; i < cardImages.Length; i++)
        {
            selected[i] = false;
            cardImages[i].transform.localScale = Vector3.one;
            cardImages[i].color = Color.white;
        }

        correctCount = 0;
    }
}