using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class JarumUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Controller Reference")]
    public Area3Controller areaController; // assign di Inspector

    public void OnPointerClick(PointerEventData eventData)
    {
        if (areaController != null)
        {
            areaController.PickupJarum();
            Debug.Log("Jarum di UI diklik dan masuk inventory!");
        }
        else
        {
            Debug.LogWarning("Area3Controller belum diassign di JarumUI!");
        }
    }
}
