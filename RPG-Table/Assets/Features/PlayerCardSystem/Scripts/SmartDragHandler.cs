using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
    private Vector3 offset;
    private Vector3 dragStartPosition;
    private static bool isMultiDragActive;
    private static List<SmartDragHandler> selectedObjects = new List<SmartDragHandler>();
    private static SmartDragHandler currentDragLeader;
    public static bool isDragging => currentDragLeader != null;

    private static bool isGlobalRightDrag = false;
    private static Vector3 globalDragStartPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (!Game) Edit = true;
    }

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

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || currentDragLeader != this) return;

        currentDragLeader = null;
        if (!Input.GetKey(multiDragKey))
        {
            selectedObjects.Clear();
        }
    }

    private void StartGlobalDrag()
    {
        isGlobalRightDrag = true;
        globalDragStartPos = Input.mousePosition;
        selectedObjects.Clear();
        selectedObjects.AddRange(FindObjectsOfType<SmartDragHandler>());
    }

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

    private void StopGlobalDrag()
    {
        isGlobalRightDrag = false;
        selectedObjects.Clear();
    }

    private void OnDestroy()
    {
        if (selectedObjects.Contains(this))
            selectedObjects.Remove(this);

        if (currentDragLeader == this)
            currentDragLeader = null;
    }
}