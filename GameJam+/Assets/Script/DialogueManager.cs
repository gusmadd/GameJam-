using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public Image dialogueBox;                // akan diisi otomatis dari Image di Text Box
    public TextMeshProUGUI dialogueText;     // akan diisi otomatis dari child TMP Text

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;
    public float eraseSpeed = 0.015f;

    private string[] currentDialog;
    private int index;
    private bool isTyping = false;
    private bool isErasing = false;
    private Coroutine typingRoutine;
    private Coroutine eraseRoutine;

    [Header("Animation")]
    public DialogueBoxAnimator dialogueAnimator;   // diambil otomatis dari komponen di Text Box

    private System.Action finishCallback;

    void Awake()
    {
        // Ambil komponen otomatis supaya tidak tergantung drag Inspector
        if (dialogueBox == null)
            dialogueBox = GetComponent<Image>();

        if (dialogueText == null)
            dialogueText = GetComponentInChildren<TextMeshProUGUI>(true);

        if (dialogueAnimator == null)
            dialogueAnimator = GetComponent<DialogueBoxAnimator>();

        if (dialogueText == null)
            Debug.LogError("[DialogueManager] Tidak menemukan TextMeshProUGUI di child Text Box!");
    }

    void Start()
    {
        if (dialogueText != null)
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
        if (dialogueAnimator != null)
            yield return dialogueAnimator.PlayShowAnimation();

        // tampilkan line pertama
        yield return StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        if (dialogueText == null) yield break;

        dialogueText.text = "";

        string line = currentDialog[index];
        typingRoutine = StartCoroutine(TypingProcess(line));

        yield return typingRoutine;

        isTyping = false;
    }

    IEnumerator TypingProcess(string line)
    {
        if (dialogueText == null) yield break;
        if (line == null) yield break;

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
        if (dialogueText == null) yield break;

        while (dialogueText.text.Length > 0)
        {
            dialogueText.text = dialogueText.text.Substring(0, dialogueText.text.Length - 1);
            yield return new WaitForSeconds(eraseSpeed);
        }
    }

    void Update()
    {
        if (currentDialog == null) return;

        // SPACE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (isTyping)
        {
            if (typingRoutine != null)
                StopCoroutine(typingRoutine);

            if (dialogueText != null)
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
            if (dialogueAnimator != null)
                yield return dialogueAnimator.PlayHideAnimation();

            GameManager.Instance.SetOpeningDialogueFinished(true);

            finishCallback?.Invoke();

            currentDialog = null;
            yield break;
        }

        yield return StartCoroutine(TypeLine());
    }
}
