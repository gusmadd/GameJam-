using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    

public class Area3Controller : MonoBehaviour
{
    [Header("Dialogue UI")]
    public GameObject playerBubble;
    public TextMeshProUGUI playerTMP;

    public GameObject npcBubble;
    public TextMeshProUGUI npcTMP;

    private bool hasTalkedToNPC = false;

    [System.Serializable]
    public class DialogueLine
    {
        public string text;
        public bool isPlayer; // true = player, false = NPC
    }

    public void StartNPCDialogue()
    {
        if (hasTalkedToNPC) return;
        hasTalkedToNPC = true;

        DialogueLine[] conversation = new DialogueLine[]
        {
            new DialogueLine{ text = "Hei, lihat teko itu di sudut ruangan.", isPlayer = false },
            new DialogueLine{ text = "Hmm… petunjuk apa ya ini?", isPlayer = true },
            new DialogueLine{ text = "Sepertinya ada sesuatu yang menarik di dalamnya...", isPlayer = false },
            new DialogueLine{ text = "Oke, aku akan periksa.", isPlayer = true }
        };

        StartCoroutine(PlayConversation(conversation));
    }

    private IEnumerator PlayConversation(DialogueLine[] conversation)
    {
        foreach (var line in conversation)
        {
            if (line.isPlayer)
            {
                playerBubble.SetActive(true);
                npcBubble.SetActive(false);

                playerTMP.text = line.text;
            }
            else
            {
                npcBubble.SetActive(true);
                playerBubble.SetActive(false);

                npcTMP.text = line.text;
            }

            // Tunggu klik / durasi
            while (!Input.GetMouseButtonDown(0)) // misal klik kiri untuk lanjut
                yield return null;
        }

        // Dialog selesai → sembunyikan semua
        playerBubble.SetActive(false);
        npcBubble.SetActive(false);

        OnNPCDialogueFinished();
    }

    private void OnNPCDialogueFinished()
    {
        Debug.Log("Dialog selesai. Bisa lanjut interaksi objek berikutnya.");
        // Misal enable klik teko
    }
}

