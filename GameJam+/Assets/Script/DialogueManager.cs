using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public Image dialogueBox;
    public TextMeshProUGUI dialogueText;

    public float typingSpeed = 0.03f;
    public float eraseSpeed = 0.015f;

    private string[] currentDialog;
    private int index;
    private bool isTyping = false;
    private bool isErasing = false;
    private Coroutine typingRoutine;
    private Coroutine eraseRoutine;

    public DialogueBoxAnimator dialogueAnimator;
    private System.Action finishCallback;

    void Start()
    {
        dialogueText.text = "";
    }

    // VERSION WITH CALLBACK
    public void StartDialogue(string[] dialog, System.Action onFinished)
    {
        GameManager.Instance.SetOpeningDialogueFinished(false);

        finishCallback = onFinished;
        currentDialog = dialog;
        index = 0;

        StartCoroutine(StartDialogueFlow());
    }

    private IEnumerator StartDialogueFlow()
    {
        // animasi textbox muncul
        yield return dialogueAnimator.PlayShowAnimation();

        // tampilkan line pertama
        yield return StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        string line = currentDialog[index];
        typingRoutine = StartCoroutine(TypingProcess(line));

        yield return typingRoutine;

        isTyping = false;
    }

    IEnumerator TypingProcess(string line)
    {
        dialogueText.text = "";
        foreach (char c in line.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator EraseLine()
    {
        isErasing = true;

        eraseRoutine = StartCoroutine(EraseProcess());
        yield return eraseRoutine;

        isErasing = false;
    }

    IEnumerator EraseProcess()
    {
        while (dialogueText.text.Length > 0)
        {
            dialogueText.text = dialogueText.text.Substring(0, dialogueText.text.Length - 1);
            yield return new WaitForSeconds(eraseSpeed);
        }
    }

    void Update()
    {
        {
            if (currentDialog == null) return;

            // SPACE
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HandleInput();
            }

        }

    }
    private void HandleInput()
    {
        if (isTyping)
        {
            if (typingRoutine != null)
                StopCoroutine(typingRoutine);

            dialogueText.text = currentDialog[index];
            isTyping = false;
        }
        else if (!isErasing)
        {
            StartCoroutine(NextDialogue());
        }
    }

    IEnumerator NextDialogue()
    {
        // hapus huruf huruf
        yield return StartCoroutine(EraseLine());

        index++;

        if (index >= currentDialog.Length)
        {
            yield return dialogueAnimator.PlayHideAnimation();

            GameManager.Instance.SetOpeningDialogueFinished(true);

            finishCallback?.Invoke();

            currentDialog = null;
            yield break;
        }

        yield return StartCoroutine(TypeLine());
    }
}