using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class SmartDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Drag Settings")]
    [SerializeField] private KeyCode multiDragKey = KeyCode.LeftControl;
    [SerializeField] private float dragThreshold = 5f;
    
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;
    private Vector2 dragStartPosition;
    private static bool isMultiDragActive;
    private static List<SmartDragHandler> selectedObjects = new List<SmartDragHandler>();
    private static SmartDragHandler currentDragLeader;
    public static bool isDragging => currentDragLeader != null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        )) return;

        offset = rectTransform.anchoredPosition - localPointerPosition;
        dragStartPosition = rectTransform.anchoredPosition;

        // Ustaw ten obiekt jako lidera przeciągania
        currentDragLeader = this;
        
        // Zarządzanie zaznaczeniem
        isMultiDragActive = Input.GetKey(multiDragKey);
        if (isMultiDragActive)
        {
            if (!selectedObjects.Contains(this))
                selectedObjects.Add(this);
        }
        else
        {
            selectedObjects.Clear();
            selectedObjects.Add(this);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentDragLeader != this) return;


        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        ))
        {

            Vector2 newPosition = localPointerPosition + offset;
            Vector2 delta = newPosition - rectTransform.anchoredPosition;

            // Przesuń wszystkie zaznaczone obiekty
            if (Vector2.Distance(newPosition, dragStartPosition) > dragThreshold)
            {
                MoveAllSelectedObjects(delta);
            }
        }
    }

    private void MoveAllSelectedObjects(Vector2 delta)
    {
        foreach (var draggable in selectedObjects)
        {
            if (draggable != null && draggable.transform.parent == this.transform.parent)
            {
                draggable.rectTransform.anchoredPosition += delta;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentDragLeader == this)
        {
            currentDragLeader = null;
            
            // Jeśli nie trzymamy klawisza multiDrag, wyczyść zaznaczenie
            if (!Input.GetKey(multiDragKey))
            {
                selectedObjects.Clear();
            }
        }
    }

    private void Update()
    {


        if (currentDragLeader == this)
        {
            //up and down in hierarchy
            if (Input.GetKeyDown("s")|| Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (this.transform.GetSiblingIndex() > 0)
                {
                    int currentIndex = this.transform.GetSiblingIndex();
                    this.transform.SetSiblingIndex(currentIndex - 1);
                }
            }
            if (Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (this.transform.GetSiblingIndex() < this.transform.parent.childCount - 1)
                {
                    int currentIndex = this.transform.GetSiblingIndex();
                    this.transform.SetSiblingIndex(currentIndex + 1);
                }
            }
        }


        // Aktualizacja stanu multiDrag
        if (Input.GetKeyDown(multiDragKey) && currentDragLeader == null)
        {
            isMultiDragActive = true;
            if (!selectedObjects.Contains(this))
                selectedObjects.Add(this);
        }
        else if (Input.GetKeyUp(multiDragKey) && currentDragLeader == null)
        {
            isMultiDragActive = false;
            selectedObjects.Clear();
        }

    }

    private void OnDestroy()
    {
        // Usuń ten obiekt z listy jeśli istnieje
        if (selectedObjects.Contains(this))
            selectedObjects.Remove(this);
            
        if (currentDragLeader == this)
            currentDragLeader = null;
    }
}