using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Area3DialogueVN : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup dialogueBox; // panel utama
    public RectTransform dialogueBoxRect;
    public TextMeshProUGUI dialogueText;

    public float fadeDuration = 0.4f;
    public float slideDistance = 50f;
    public float typingSpeed = 0.03f;

    [Header("Bubbles")]
    public GameObject playerBubble;
    public GameObject npcBubble;
    public TextMeshProUGUI playerTMP;
    public TextMeshProUGUI npcTMP;

    private bool isTyping = false;

    private Vector2 originalPos;

    void Awake()
    {
        originalPos = dialogueBoxRect.anchoredPosition;
        dialogueBox.alpha = 0;
        dialogueBoxRect.anchoredPosition = originalPos + new Vector2(0, slideDistance);
    }

    [System.Serializable] // <-- CS0579: Hapus duplikat dari file
    public class DialogueLine
    {
        public string text;
        public bool isPlayer; // true = player, false = NPC
        public string speaker; // <-- Tambahan dari error CS0117 sebelumnya
    }

    public void StartDialogue(DialogueLine[] conversation, System.Action onFinished)
    {
        StartCoroutine(PlayConversation(conversation, onFinished));
    }

    private IEnumerator PlayConversation(DialogueLine[] conversation, System.Action onFinished)
    {
        foreach (var line in conversation)
        {
            if (line.isPlayer)
            {
                playerBubble.SetActive(true);
                npcBubble.SetActive(false);
                playerTMP.text = "";
            }
            else
            {
                npcBubble.SetActive(true);
                playerBubble.SetActive(false);
                npcTMP.text = "";
            }

            // fade in box
            yield return StartCoroutine(FadeInBox());

            TextMeshProUGUI tmp = line.isPlayer ? playerTMP : npcTMP;
            yield return StartCoroutine(TypeLine(tmp, line.text));

            // tunggu input
            bool waiting = true;
            while (waiting)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                    waiting = false;
                yield return null;
            }

            yield return StartCoroutine(FadeOutBox());
        }

        // hide bubbles
        playerBubble.SetActive(false);
        npcBubble.SetActive(false);

        onFinished?.Invoke();
    }

    private IEnumerator FadeInBox()
    {
        float t = 0f;
        Vector2 startPos = originalPos + new Vector2(0, slideDistance);
        Vector2 endPos = originalPos;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = t / fadeDuration;
            dialogueBox.alpha = Mathf.Lerp(0, 1, p);
            dialogueBoxRect.anchoredPosition = Vector2.Lerp(startPos, endPos, p);
            yield return null;
        }
        dialogueBox.alpha = 1;
        dialogueBoxRect.anchoredPosition = endPos;
    }

    private IEnumerator FadeOutBox()
    {
        float t = 0f;
        Vector2 startPos = dialogueBoxRect.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, slideDistance);

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = t / fadeDuration;
            dialogueBox.alpha = Mathf.Lerp(1, 0, p);
            dialogueBoxRect.anchoredPosition = Vector2.Lerp(startPos, endPos, p);
            yield return null;
        }
        dialogueBox.alpha = 0;
        dialogueBoxRect.anchoredPosition = endPos;
    }

    private IEnumerator TypeLine(TextMeshProUGUI tmp, string line)
    {
        isTyping = true;
        tmp.text = "";
        foreach (char c in line)
        {
            tmp.text += c;

            float timer = 0f;
            while (timer < typingSpeed)
            {
                timer += Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    tmp.text = line; // langsung selesai
                    break;
                }
                yield return null;
            }
        }
        isTyping = false;
    }
}
