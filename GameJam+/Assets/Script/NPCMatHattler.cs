using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMatHattler : MonoBehaviour
{
    public bool canInteract = true; // true saat NPC bisa diklik
    public Area3Controller areaController;

    void OnMouseDown()
    {
        if (!canInteract) return;

        canInteract = false; // matikan sementara agar dialog tidak double
        areaController.StartNPCDialogue();
    }
}
