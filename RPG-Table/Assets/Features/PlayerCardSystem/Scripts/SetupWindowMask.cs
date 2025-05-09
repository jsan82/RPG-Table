using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Dodane dla obsługi przeciągania

public class SetupWindowMask : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] private RectTransform window; // Panel z maską
    [SerializeField] private RectTransform cardArea; // Teraz też RectTransform (ważne dla pozycji UI)
    [SerializeField] private float dragSpeed = 1f; // Prędkość przeciągania

    private Vector2 initialCardAreaPos;
    private Vector2 initialDragPos;

    void Start()
    {
        // Dodaj RectMask2D jeśli nie istnieje
        if (!window.TryGetComponent<RectMask2D>(out _))
        {
            window.gameObject.AddComponent<RectMask2D>();
        }

        // Upewnij się, że CardArea jest dzieckiem okna
        cardArea.SetParent(window);
        cardArea.SetAsFirstSibling();

        // Zapisz początkową pozycję cardArea (opcjonalne, np. do resetu)
        initialCardAreaPos = cardArea.anchoredPosition;
    }

    // Rozpoczęcie przeciągania
    public void OnBeginDrag(PointerEventData eventData)
    {
        initialDragPos = eventData.position;
        Debug.Log("Dragging");
    }

    // Przeciąganie
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragDelta = (eventData.position - initialDragPos) * dragSpeed;
        cardArea.anchoredPosition += dragDelta;
        initialDragPos = eventData.position; // Aktualizuj pozycję początkową
    }
}