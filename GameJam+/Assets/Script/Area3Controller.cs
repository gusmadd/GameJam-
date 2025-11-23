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
    public bool isClockSolved = false; // <--- WAJIB DITAMBAH!
    public GameObject fragmentPrefab; // assign prefab fragment di inspector
    public Transform fragmentSpawnPoint; // posisi muncul fragment

    void Start()
    {
        if (inventorySystem == null)
            inventorySystem = InventorySystem.Instance;

        if (uiInventory == null)
            uiInventory = UIInventory.Instance;

        if (dialogueVN == null)
            Debug.LogError("dialogueVN belum diassign di Inspector!");
        if (jamPanelController != null)
        {
            // Daftarkan fungsi untuk dipanggil saat panel jam tertutup
            jamPanelController.OnPanelClosed += OnClockPuzzleSolved;
        }
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

        Sprite jarumSprite = inventorySystem.GetItemSprite(jarumItemName);

        // 1. Pastikan panel terbuka (Jika belum dibuka, anggap jamPanelController mengurusnya)
        jamPanelController.OpenPanel(null); // Gunakan null karena sprite jam sudah diatur di tempat lain

        // 2. Panggil fungsi AKTIVASI jarum yang benar
        jamPanelController.ActivateJarum(jarumSprite); // <-- GANTI DENGAN ActivateJarum

        // Hapus jarum dari inventory
        inventorySystem.GetItems().Remove(jarumItemName);
        uiInventory.ClearAllSlots();
        uiInventory.RebuildUIFromData();
    }
    public void OnClockPuzzleSolved()
    {
        isClockSolved = true;

        // Aktifkan kembali interaksi NPC untuk dialog hadiah
        NPCMatHattler npcScript = matHattler.GetComponent<NPCMatHattler>();
        if (npcScript != null)
        {
            npcScript.canInteract = true;
            Debug.Log("Jam Solved: NPC Mat Hattler diaktifkan untuk dialog hadiah.");
        }
    }
    public void RewardFragment()
    {
        // ... (Logika dialog) ...
        Area3DialogueVN.DialogueLine[] rewardConversation = new Area3DialogueVN.DialogueLine[]
        {
        new Area3DialogueVN.DialogueLine { text = "Thank you! As a reward, I’ll give you this fragment. I don’t need it anymore.", isPlayer = false },
        new Area3DialogueVN.DialogueLine { text = "You should hurry to the Heart Palace. Please, free the White Rabbit!", isPlayer = false }
        };

        dialogueVN.StartDialogue(rewardConversation, SpawnFragment);
    }

    private void SpawnFragment()
    {
        if (fragmentPrefab != null && fragmentSpawnPoint != null)
        {
            GameObject fragment = Instantiate(fragmentPrefab, fragmentSpawnPoint.position, Quaternion.identity);

            // DAPATKAN SCRIPT Area3Fragment YANG BARU
            Area3Fragment area3FragmentScript = fragment.GetComponent<Area3Fragment>();

            if (area3FragmentScript != null)
            {
                area3FragmentScript.SetAreaController(this); // <-- KIRIM REFERENSI AREA3CONTROLLER
                area3FragmentScript.PlaySpawnAnimation();
            }
            else
            {
                Debug.LogError("ERROR: FragmentPrefab tidak memiliki script Area3Fragment!");
            }
        }
    }

    // ====================== FUNGSI BARU: Dipanggil oleh Area3Fragment Saat Diklik ======================
    public void CollectFragmentAndProceed(GameObject fragmentObject, string itemName, Sprite itemSprite)
    {
        // InventorySystem.Instance.AddItem(itemName, itemSprite); // FragmentFloating sudah melakukannya
        if (inventorySystem.AddItem(itemName, itemSprite)) // Panggil via InventorySystem milik Area3Controller
        {
            Destroy(fragmentObject); // Hapus objek fragment dari scene

            // Lanjutkan ke area berikutnya
            StartCoroutine(FinalTransition());
        }
    }

    private IEnumerator FinalTransition()
    {
        yield return new WaitForSeconds(0.5f); // Jeda opsional
        TransitionManager.Instance.FadeOutAndLoadScene("Area 5");
    }
}
