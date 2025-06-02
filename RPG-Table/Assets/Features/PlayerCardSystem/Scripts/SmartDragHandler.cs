using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


/// <summary>
/// Advanced drag handler for UI elements with multi-select and global drag capabilities
/// </summary>
/// <remarks>
/// Features:
/// - Single object dragging
/// - Global right-click drag for all objects
/// - Drag threshold to prevent accidental movements
/// - Hierarchy manipulation shortcuts
/// - Edit/Game mode differentiation
/// </remarks>
[RequireComponent(typeof(RectTransform))]
public class SmartDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Drag Settings")]
    [SerializeField] private KeyCode multiDragKey = KeyCode.LeftControl;
    [SerializeField] private float dragThreshold = 5f;
    
    public bool Edit = false;
    public bool Game = true;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;
    private Vector2 dragStartPosition;
    private static bool isMultiDragActive;
    private static List<SmartDragHandler> selectedObjects = new List<SmartDragHandler>();
    private static SmartDragHandler currentDragLeader;
    public static bool isDragging => currentDragLeader != null;

    private static bool isGlobalRightDrag = false;
    private static Vector2 globalDragStartPos;


    /// <summary>
    /// Initializes drag handler components
    /// </summary>
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (!Game)
        {
            Edit = true;
        }
    }

    /// <summary>
    /// Handles per-frame input processing
    /// </summary>
    /// <remarks>
    /// Manages:
    /// - Global right-drag operations
    /// - Scale modification shortcuts (W/S keys)
    /// - Multi-drag key states
    /// </remarks>
    private void Update()
    {

        if (Input.GetKeyDown(multiDragKey))
        {
            StartGlobalDrag();
        }
        else if (Input.GetKeyUp(multiDragKey))
        {
            StopGlobalDrag();
        }
        else if (isGlobalRightDrag && Input.GetKey(multiDragKey))
        {
            UpdateGlobalDrag();
        }


        if (currentDragLeader == this)
        {

            // Debug.Log($"shift down:  {Input.GetKeyDown(KeyCode.LeftShift)}");
            if (Input.GetKeyDown("w"))
            {
                Debug.Log("Scale Up");
                this.rectTransform.localScale = new Vector3(this.rectTransform.localScale.x + 0.5f, this.rectTransform.localScale.y + 0.5f, 1f);
            }
            if (Input.GetKeyDown("s"))
            {
                Debug.Log("Scale Down");
                this.rectTransform.localScale = new Vector3(this.rectTransform.localScale.x - 0.5f, this.rectTransform.localScale.y - 0.5f, 1f);
            }

        }
        if (Input.GetKeyDown(KeyCode.Delete) && isDragging && Game)
        {
            Destroy(currentDragLeader.gameObject);
        }
    }

    
    /// <summary>
    /// Initiates global drag for all objects
    /// </summary>
    private void StartGlobalDrag()
    {
        isGlobalRightDrag = true;
        globalDragStartPos = Input.mousePosition;
        selectedObjects.Clear();
        selectedObjects.AddRange(FindObjectsOfType<SmartDragHandler>());
    }

    
    /// <summary>
    /// Updates positions during global drag
    /// </summary>
    private void UpdateGlobalDrag()
    {
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 delta = (currentMousePos - globalDragStartPos) / canvas.scaleFactor; 

        foreach (var draggable in selectedObjects)
        {
            if (draggable != null)
            {
                draggable.rectTransform.anchoredPosition += delta;
            }
        }

        globalDragStartPos = currentMousePos;
    }

    /// <summary>
    /// Terminates global drag operation
    /// </summary>
    private void StopGlobalDrag()
    {
        isGlobalRightDrag = false;
        selectedObjects.Clear();
    }

    /// <summary>
    /// Handles pointer down event (drag start)
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
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

    /// <summary>
    /// Handles drag movement
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
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


    /// <summary>
    /// Moves all selected objects by delta amount
    /// </summary>
    /// <param name="delta">Movement vector</param>
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

    /// <summary>
    /// Handles pointer release (drag end)
    /// </summary>
    /// <param name="eventData">Pointer event data</param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || currentDragLeader != this) return;

        currentDragLeader = null;
        if (!Input.GetKey(multiDragKey))
        {
            selectedObjects.Clear();
        }
    }

    /// <summary>
    /// Cleans up static references when destroyed
    /// </summary>
    private void OnDestroy()
    {
        if (selectedObjects.Contains(this))
            selectedObjects.Remove(this);

        if (currentDragLeader == this)
            currentDragLeader = null;
    }
}