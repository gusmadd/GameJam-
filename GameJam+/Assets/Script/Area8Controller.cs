using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Area8Controller : MonoBehaviour
{
    public Area3DialogueVN dialogueVN;
    public FragmentFrame fragmentFrame;

    [Header("Effects")]
    public Light2D frameLight;
    public Camera mainCam;

    private bool dialogueCompleted = false;
    private bool frameActivated = false;

    void Start()
    {
        StartCoroutine(StartSceneSequence());
    }

    // ====================== DIALOG AWAL ======================
    IEnumerator StartSceneSequence()
    {
        yield return new WaitForSeconds(0.5f);

        Area3DialogueVN.DialogueLine[] intro = new Area3DialogueVN.DialogueLine[]
        {
            new Area3DialogueVN.DialogueLine{ text="Selamat datang, Timekeeper baru kami.", speaker="???" },
            new Area3DialogueVN.DialogueLine{ text="Hah? Apa maksudnya?", speaker="Alice", isPlayer=true },
            new Area3DialogueVN.DialogueLine{ text="Ah, yang penting aku harus selamatkan White Rabbit dulu. Tanpa dia Wonderland akan hancur.", speaker="Alice", isPlayer=true }
        };

        dialogueVN.StartDialogue(intro, OnIntroDialogueFinished);
    }

    private void OnIntroDialogueFinished()
    {
        dialogueCompleted = true;
        Debug.Log("Dialog selesai. Bingkai bisa diklik.");
    }

    // ====================== KLIK BINGKAI ======================
    public void OnFrameClicked()
    {
        if (!dialogueCompleted) return;
        if (frameActivated) return;

        frameActivated = true;

        Debug.Log("Bingkai diklik — zooming + light ON");

        // Nyalakan light
        if (frameLight != null)
        {
            frameLight.gameObject.SetActive(true);
        }

        // Kamera zoom
        StartCoroutine(ZoomCamera());
    }

    IEnumerator ZoomCamera()
    {
        if (mainCam == null) mainCam = Camera.main;

        float startSize = mainCam.orthographicSize;
        float targetSize = startSize * 0.8f; // zoom 20%
        float t = 0f;
        float duration = 0.5f;

        while (t < duration)
        {
            t += Time.deltaTime;
            mainCam.orthographicSize = Mathf.Lerp(startSize, targetSize, t / duration);
            yield return null;
        }
    }
}
