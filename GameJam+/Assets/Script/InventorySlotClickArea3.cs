using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryClickDetectorArea3 : MonoBehaviour
{
    public UIInventory uiInventory;
    public Area3Controller area3Controller;

    // InventoryClickDetectorArea3.cs

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;

            for (int i = 0; i < uiInventory.slots.Count; i++)
            {
                var slot = uiInventory.slots[i];
                if (slot.sprite == null) continue;

                if (RectTransformUtility.RectangleContainsScreenPoint(slot.rectTransform, mousePos))
                {
                    string itemName = uiInventory.GetItemName(i);

                    // Pastikan nama item di sini sesuai dengan Area3Controller.cs
                    if (itemName == "Jam Rusak")
                    {
                        // Daripada memanggil OpenPanel langsung, panggil Area3Controller.OpenJamPanel() 
                        // karena fungsi itu yang berisi logika penghapusan
                        // area3Controller.OpenJamPanel(); <--- Jika Anda menggunakan fungsi ini

                        // Jika Anda harus membuka panel langsung, hapus item di sini:

                        // =========================================================
                        // 1. Panggil OpenPanel
                        area3Controller.jamPanelController.OpenPanel(slot.sprite);

                        // 2. Hapus item
                        InventorySystem.Instance.GetItems().Remove(itemName);

                        // 3. Refresh UI
                        uiInventory.ClearAllSlots();
                        uiInventory.RebuildUIFromData();
                        // =========================================================

                    }
                    else if (itemName == "Jarum Pendek") // Perbaiki: Jarum Pendek HARUS dihapus di fungsi PlaceJarumInJam
                    {
                        area3Controller.PlaceJarumInJam();
                    }
                }
            }
        }
    }
}
