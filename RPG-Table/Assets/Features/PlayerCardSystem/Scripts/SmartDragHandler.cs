using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Handles advanced drag functionality for UI elements, including multi-select and global drag operations.
/// Implements IDragHandler, IPointerDownHandler, and IPointerUpHandler to respond to Unity's drag events.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SmartDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Drag Settings")]
    [SerializeField] 
    private KeyCode multiDragKey = KeyCode.LeftControl;
    
    [SerializeField] 
    private float dragThreshold = 5f;
    
    /// <summary>
    /// Determines if the object is in edit mode (editor-only functionality).
    /// </summary>
    public bool Edit = false;
    
    /// <summary>
    /// Determines if the object is in game mode (runtime functionality).
    /// </summary>
    public bool Game = true;
    
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 offset;
    private Vector3 dragStartPosition;
    private static bool isMultiDragActive;
    private static List<SmartDragHandler> selectedObjects = new List<SmartDragHandler>();
    private static SmartDragHandler currentDragLeader;
    
    /// <summary>
    /// Indicates whether any SmartDragHandler is currently being dragged.
    /// </summary>
    public static bool isDragging => currentDragLeader != null;

    private static bool isGlobalRightDrag = false;
    private static Vector3 globalDragStartPos;

    /// <summary>
    /// Initializes required components and sets up initial state.
    /// </summary>
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (!Game) Edit = true;
    }

    /// <summary>
    /// Handles per-frame updates including global drag operations and scaling controls.
    /// </summary>
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
            if (Input.GetKeyDown("w"))
            {
                rectTransform.localScale += Vector3.one * 0.1f;
            }
            if (Input.GetKeyDown("s"))
            {
                rectTransform.localScale -= Vector3.one * 0.1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete) && isDragging && Game)
        {
            Destroy(currentDragLeader.gameObject);
        }
    }

    /// <summary>
    /// Handles pointer down events to initiate drag operations.
    /// </summary>
    /// <param name="eventData">Pointer event data containing position and button information.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint
        );

        offset = transform.position - worldPoint;
        dragStartPosition = transform.position;

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
    /// Handles drag events to move the object and any selected objects.
    /// </summary>
    /// <param name="eventData">Pointer event data containing current position information.</param>
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || currentDragLeader != this) return;

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint
        );

        Vector3 newPosition = worldPoint + offset;
        Vector3 delta = newPosition - transform.position;

        if (Vector3.Distance(newPosition, dragStartPosition) > dragThreshold)
        {
            MoveAllSelectedObjects(delta);
        }
    }

    /// <summary>
    /// Moves all currently selected objects by the specified delta.
    /// </summary>
    /// <param name="delta">The amount to move each selected object.</param>
    private void MoveAllSelectedObjects(Vector3 delta)
    {
        foreach (var draggable in selectedObjects)
        {
            if (draggable != null)
            {
                draggable.transform.position += delta;
            }
        }
    }

    /// <summary>
    /// Handles pointer up events to complete drag operations.
    /// </summary>
    /// <param name="eventData">Pointer event data containing button information.</param>
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
    /// Initiates a global drag operation that affects all SmartDragHandler objects.
    /// </summary>
    private void StartGlobalDrag()
    {
        isGlobalRightDrag = true;
        globalDragStartPos = Input.mousePosition;
        selectedObjects.Clear();
        selectedObjects.AddRange(FindObjectsOfType<SmartDragHandler>());
    }

    /// <summary>
    /// Updates the position of all objects during a global drag operation.
    /// </summary>
    private void UpdateGlobalDrag()
    {
        Vector3 currentMousePos = Input.mousePosition;
        Vector3 delta = Camera.main.ScreenToWorldPoint(currentMousePos) - 
                       Camera.main.ScreenToWorldPoint(globalDragStartPos);

        foreach (var draggable in selectedObjects)
        {
            if (draggable != null)
            {
                draggable.transform.position += delta;
            }
        }

        globalDragStartPos = currentMousePos;
    }

    /// <summary>
    /// Stops the global drag operation and clears the selection.
    /// </summary>
    private void StopGlobalDrag()
    {
        isGlobalRightDrag = false;
        selectedObjects.Clear();
    }

    /// <summary>
    /// Cleans up references when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (selectedObjects.Contains(this))
            selectedObjects.Remove(this);

        if (currentDragLeader == this)
            currentDragLeader = null;
    }
}