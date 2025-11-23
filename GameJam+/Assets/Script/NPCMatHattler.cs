using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMatHattler : MonoBehaviour
{
    public bool canInteract = true; // true saat NPC bisa diklik
    public Area3Controller areaController;
    // Tambahkan variabel ini untuk mencegah dialog hadiah berulang
    private bool rewardGiven = false;

    void OnMouseDown()
    {
        if (!canInteract || areaController == null) return;
        if (rewardGiven) return; // Jangan lakukan apa-apa jika hadiah sudah diberikan

        // 1. Cek status puzzle dari Area3Controller
        bool isSolved = areaController.isClockSolved; // <--- ASUMSIKAN VARIABEL INI ADA DI Area3Controller

        canInteract = false; // Matikan interaksi sementara

        if (isSolved)
        {
            // 2. JIKA PUZZLE SELESAI: Panggil dialog hadiah
            areaController.RewardFragment();
            rewardGiven = true; // Tandai hadiah sudah diproses
            // canInteract akan tetap false hingga dialog selesai, lalu tidak akan dipicu lagi karena rewardGiven=true
        }
        else
        {
            // 3. JIKA PUZZLE BELUM SELESAI: Panggil dialog awal
            areaController.ClickNPC();
            // canInteract akan dihidupkan lagi di OnNPCDialogueFinished() di Area3Controller
        }
    }
}