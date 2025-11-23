using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentFrame : MonoBehaviour
{
    public Area8Controller area8; // assign di inspector

    private void OnMouseDown()
    {
        if (area8 == null) return;

        area8.OnFrameClicked();
    }
}
