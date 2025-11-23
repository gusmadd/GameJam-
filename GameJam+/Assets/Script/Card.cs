using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public int cardIndex;
    public PuzzleManager manager;

    private RectTransform rect;
    private Vector2 startPos;
    private Vector2 raisedPos;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
        raisedPos = startPos + new Vector2(0, 40f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.ClickCard(cardIndex);

    }

    public void Raise()
    {
        rect.anchoredPosition = raisedPos;
    }

    public void Lower()
    {
        rect.anchoredPosition = startPos;
    }
}