using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentFloating : MonoBehaviour
{
     public string itemName = "Fragment1";
    public Sprite itemIcon;

    private bool picked = false;

    private void OnMouseDown()
    {
        if (picked) return;
        picked = true;

        // masuk inventory
        InventorySystem.Instance.AddItem(itemName, itemIcon);

        // kabari puzzle manager
        MushroomPuzzleManager.Instance.OnFragmentCollected();

        Destroy(gameObject);
    }
}
