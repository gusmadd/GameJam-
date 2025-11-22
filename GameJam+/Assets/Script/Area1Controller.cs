using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area1Controller : MonoBehaviour
{
    public DialogueManager dialogueManager;

    [Header("Interactables")]
    public GameObject windowObject;
    public PickupItem watchItem;

    private bool canPickWatch = false;
    private bool canClickWindow = false;
    public bool CanPickWatch => canPickWatch;
    public bool CanClickWindow => canClickWindow;
    [TextArea(2, 6)]
    public float startDelay = 2.2f;
    private string[] openingDialog = new string[]
    {
        "Dimana ini...?",
        "Kepalaku... terasa berat.",
        "Aku harus mencari tahu apa yang terjadi.",
        "Kamu ambilah jam itu, nadbfuhabff...."
    };

    void Start()
    {
        // Disable semua interaksi dulu
        watchItem.enabled = false;
        windowObject.GetComponent<WindowInteract>().enabled = false;

        // Mulai dialog 1
        StartCoroutine(StartOpeningDialogueWithDelay());
    }
    private IEnumerator StartOpeningDialogueWithDelay()
    {
        yield return new WaitForSeconds(2f);   // JEDA 2 DETIK

        dialogueManager.StartDialogue(openingDialog, OnOpeningDialogueFinished);
    }

    private void OnOpeningDialogueFinished()
    {
        // Setelah dialog pertama selesai → boleh ambil jam saja
        canPickWatch = true;
        watchItem.enabled = true;
    }

    public void OnWatchCollected()
    {
        // Disable agar tidak double click
        watchItem.enabled = false;

        // Tambah ke inventory
        UIInventory.Instance.AddItemToUI(Resources.Load<Sprite>("Sprites/Items/broken_watch"));

        // Mulai dialog kedua
        string[] afterWatch = {
            "Wah jam ini rusak... gimana ya.",
            "Sepertinya aku harus keluar dari ruangan ini."
        };

        dialogueManager.StartDialogue(afterWatch, OnAfterWatchFinished);
    }

    private void OnAfterWatchFinished()
    {
        canClickWindow = true; // <-- WAJIB DITAMBAHKAN
        // Baru jendela boleh diklik
        windowObject.GetComponent<WindowInteract>().enabled = true;
    }
}
