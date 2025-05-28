using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class SmartDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Drag Settings")]
    [SerializeField] private KeyCode multiDragKey = KeyCode.LeftControl;
    [SerializeField] private float dragThreshold = 5f;
    
    public bool Edit = true;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;
    private Vector2 dragStartPosition;
    private static bool isMultiDragActive;
    private static List<SmartDragHandler> selectedObjects = new List<SmartDragHandler>();
    private static SmartDragHandler currentDragLeader;
    public static bool isDragging => currentDragLeader != null;

    // Nowe zmienne dla globalnego przeciągania prawym przyciskiem
    private static bool isGlobalRightDrag = false;
    private static Vector2 globalDragStartPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        // Globalne przeciąganie prawym przyciskiem (nawet poza UI)
        if (Input.GetMouseButtonDown(1)) // Prawy przycisk myszy wciśnięty
        {
            StartGlobalDrag();
        }
        else if (Input.GetMouseButtonUp(1)) // Prawy przycisk myszy puszczony
        {
            StopGlobalDrag();
        }
        else if (isGlobalRightDrag && Input.GetMouseButton(1)) // Przeciąganie w trakcie
        {
            UpdateGlobalDrag();
        }

        // Reszta logiki (hierarchia, multi-drag itp.)
        if (currentDragLeader == this)
        {

           // Debug.Log($"shift down:  {Input.GetKeyDown(KeyCode.LeftShift)}");
            if (Input.GetKeyDown("w"))
            {
                Debug.Log("Scale Up");
                this.rectTransform.localScale = new Vector3(this.rectTransform.localScale.x + 0.2f, this.rectTransform.localScale.y + 0.2f, 1f);
            }
            if (Input.GetKeyDown("s"))
            {
                Debug.Log("Scale Down");
                this.rectTransform.localScale = new Vector3(this.rectTransform.localScale.x - 0.2f, this.rectTransform.localScale.y - 0.2f, 1f);
            }
            // if (Input.GetKeyDown("s") || Input.GetKeyDown(KeyCode.DownArrow))
            // {
            //     Debug.Log("Move Up");
            //     if (!Edit) return;
            //     if (this.transform.GetSiblingIndex() > 0)
            //     {
            //         int currentIndex = this.transform.GetSiblingIndex();
            //         this.transform.SetSiblingIndex(currentIndex - 1);
            //     }
            // }
            // if (Input.GetKeyDown("w") || Input.GetKeyDown(KeyCode.UpArrow))
            // {
            //     Debug.Log("Move Down");
            //     if (!Edit) return;
            //     if (this.transform.GetSiblingIndex() < this.transform.parent.childCount - 1)
            //     {
            //         int currentIndex = this.transform.GetSiblingIndex();
            //         this.transform.SetSiblingIndex(currentIndex + 1);
            //     }
            // }
        }

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

    // Rozpoczęcie globalnego przeciągania prawym przyciskiem
    private void StartGlobalDrag()
    {
        isGlobalRightDrag = true;
        globalDragStartPos = Input.mousePosition;
        selectedObjects.Clear();
        selectedObjects.AddRange(FindObjectsOfType<SmartDragHandler>());
    }

    // Aktualizacja pozycji podczas globalnego przeciągania
    private void UpdateGlobalDrag()
    {
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 delta = (currentMousePos - globalDragStartPos) / canvas.scaleFactor; // Uwzględniamy skalę canvasa

        foreach (var draggable in selectedObjects)
        {
            if (draggable != null)
            {
                draggable.rectTransform.anchoredPosition += delta;
            }
        }

        globalDragStartPos = currentMousePos; // Aktualizujemy pozycję startową
    }

    // Zakończenie globalnego przeciągania
    private void StopGlobalDrag()
    {
        isGlobalRightDrag = false;
        selectedObjects.Clear();
    }

    // Standardowe przeciąganie (lewy przycisk myszy)
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        )) return;

        offset = rectTransform.anchoredPosition - localPointerPosition;
        dragStartPosition = rectTransform.anchoredPosition;

        currentDragLeader = this;
        
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
        if (eventData.button != PointerEventData.InputButton.Left || currentDragLeader != this) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        ))
        {
            Vector2 newPosition = localPointerPosition + offset;
            Vector2 delta = newPosition - rectTransform.anchoredPosition;

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
            if (draggable != null)
            {
                draggable.rectTransform.anchoredPosition += delta;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || currentDragLeader != this) return;

        currentDragLeader = null;
        if (!Input.GetKey(multiDragKey))
        {
            selectedObjects.Clear();
        }
    }

    private void OnDestroy()
    {
        if (selectedObjects.Contains(this))
            selectedObjects.Remove(this);
            
        if (currentDragLeader == this)
            currentDragLeader = null;
    }
}