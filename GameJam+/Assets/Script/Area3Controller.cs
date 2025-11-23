using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Area3Controller : MonoBehaviour
{
    [Header("Dialogue VN")]
    public Area3DialogueVN dialogueVN;

    [Header("Mat Hattler NPC")]
    public GameObject matHattler;

    [Header("Teko & Panel")]
    public KettleInteract kettleInteract;
    public GameObject kettleObject;
    public GameObject kettlePanel;
    public GameObject jarumJam;
    public GameObject jamPanel;

    [Header("Inventory Item Names")]
    public string jarumItemName = "JarumJam";
    public string jamRusakItemName = "JamRusak";

    [Header("Inventory reference")]
    public InventorySystem inventorySystem;
    public UIInventory uiInventory;

    [Header("Jam Panel reference")]
    public JamPanelController jamPanelController;
    private bool hasTalkedToNPC = false;

    void Start()
    {
        if (inventorySystem == null)
            inventorySystem = InventorySystem.Instance;

        if (uiInventory == null)
            uiInventory = UIInventory.Instance;

        if (dialogueVN == null)
            Debug.LogError("dialogueVN belum diassign di Inspector!");
    }

    // ====================== CLICK NPC ======================
    public void ClickNPC()
    {
        if (hasTalkedToNPC) return;
        hasTalkedToNPC = true;

        Area3DialogueVN.DialogueLine[] conversation = new Area3DialogueVN.DialogueLine[]
        {
            new Area3DialogueVN.DialogueLine{ text="Help me, I’m always stuck at 6:00 PM.", isPlayer=false },
            new Area3DialogueVN.DialogueLine{ text="Free me—make time move, even if just by one hour.", isPlayer=false },
        };

        dialogueVN.StartDialogue(conversation, OnNPCDialogueFinished);
    }

    private void OnNPCDialogueFinished()
    {
        // aktifkan interaksi teko
        if (kettleInteract != null)
            kettleInteract.CanInteract = true;

        Debug.Log("NPC dialogue selesai, teko bisa diklik");
    }

    // ====================== PICKUP JARUM ======================
    public void PickupJarum()
    {
        if (inventorySystem == null)
        {
            Debug.LogError("InventorySystem.Instance belum siap!");
            return;
        }

        Sprite jarumSprite = inventorySystem.GetItemSprite(jarumItemName);
        if (jarumSprite == null)
        {
            Debug.LogError("Sprite jarum belum diassign di InventorySystem!");
            return;
        }

        bool added = inventorySystem.AddItem(jarumItemName, jarumSprite);

        if (added)
        {
            jarumJam.SetActive(false);
            kettlePanel.SetActive(false);
            kettleInteract.ClosePanel();
            Debug.Log("Jarum diambil, masuk inventory");
        }
    }

    // Area3Controller.cs (Perlu dipastikan kode ini ada dan benar)
    public void PlaceJarumInJam()
    {
        if (!inventorySystem.HasItem(jarumItemName))
        {
            Debug.LogWarning("Jarum tidak ada di inventory!");
            return;
        }

        // Panggil klik jarum tanpa parameter
        jamPanelController.OnJarumClick();

        // Hapus jarum dari inventory
        inventorySystem.GetItems().Remove(jarumItemName);

        // Refresh UI
        uiInventory.ClearAllSlots();
        uiInventory.RebuildUIFromData();
    }
}
