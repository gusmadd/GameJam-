using UnityEngine;

public class HintButton : MonoBehaviour
{
    public HintPanelController hintPanel;
    public Sprite hintSprite;

    public void OnHintClicked()
    {
        if (hintPanel != null)
            hintPanel.ShowHint(hintSprite);
    }
}
