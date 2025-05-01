using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SmartDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Drag Settings")]
    [SerializeField] private KeyCode multiDragKey = KeyCode.LeftControl;
    [SerializeField] private float dragThreshold = 5f; // Minimalny ruch aby rozpocząć przeciąganie
    
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;
    private bool isDragging;
    private Vector2 dragStartPosition;
    private static bool isMultiDragActive;

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
        isDragging = true;

        // Sprawdź czy wciśnięto klawisz modyfikujący
        isMultiDragActive = Input.GetKey(multiDragKey);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        ))
        {
            Vector2 newPosition = localPointerPosition + offset;
            Vector2 delta = newPosition - rectTransform.anchoredPosition;

            // Przeciągnij wszystkie obiekty jeśli multiDrag aktywny
            if (isMultiDragActive && Vector2.Distance(newPosition, dragStartPosition) > dragThreshold)
            {
                DragAllSelectedObjects(delta);
            }

            rectTransform.anchoredPosition = newPosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        isMultiDragActive = false;
    }

    private void DragAllSelectedObjects(Vector2 delta)
    {
        // Znajdź wszystkie obiekty z tym samym tagiem lub w tej samej grupie
        foreach (SmartDragHandler draggable in FindObjectsOfType<SmartDragHandler>())
        {
            if (draggable != this && draggable.transform.parent == this.transform.parent)
            {
                draggable.rectTransform.anchoredPosition += delta;
            }
        }
    }

    private void Update()
    {
        // Aktualizuj stan multiDrag jeśli klawisz został wciśnięty podczas przeciągania
        if (isDragging)
        {
            isMultiDragActive = Input.GetKey(multiDragKey);
        }
    }
}