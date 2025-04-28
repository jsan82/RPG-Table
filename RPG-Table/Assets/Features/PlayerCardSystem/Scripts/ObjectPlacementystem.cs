using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class ObjectPlacementSystem : MonoBehaviour
{
    [System.Serializable]
    public class PlaceableObject
    {
        public string name;
        public GameObject prefab;
        public Sprite icon;
    }

    public List<PlaceableObject> availableObjects;
    public Transform objectsPanel;
    public Transform cardArea;
    public GameObject objectButtonPrefab;
    public GameObject confirmationDialog;
    public GameObject inputTypeDropdown;
    public InputField idInputField;
    private GameObject selectedPrefab;
    private GameObject pendingObject;

    void Start()
    {
        Debug.Log("Rozpoczynam inicjalizację...");
        Debug.Log(Application.persistentDataPath);
    
        if (cardArea == null)
        {
            GameObject area = GameObject.Find("CardArea");
            if (area != null) cardArea = area.transform;
            else Debug.LogError("Nie znaleziono obiektu CardArea w scenie!");
        }

        if (availableObjects == null)
            Debug.LogError("availableObjects jest null!");
        else if (availableObjects.Count == 0)
            Debug.LogError("availableObjects jest pusty!");

        if (objectsPanel == null)
            Debug.LogError("objectsPanel nie jest przypisany!");

        if (objectButtonPrefab == null)
            Debug.LogError("objectButtonPrefab nie jest przypisany!");

        foreach (var obj in availableObjects)
        {
            GameObject button = Instantiate(objectButtonPrefab, objectsPanel);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = obj.name;
            button.GetComponent<Button>().onClick.AddListener(() => SelectObject(obj.prefab));
        }
    }

   void Update()
{
    if (pendingObject != null)
    {
        if (Input.GetMouseButtonDown(0))
            {
                if(selectedPrefab.name == "InputField")
                {
                    inputTypeDropdown.SetActive(true);
                }
                else
                {
                    inputTypeDropdown.SetActive(false);
                }
                confirmationDialog.SetActive(true);
            }

        if(!confirmationDialog.activeSelf){
            Vector2 mousePos = Input.mousePosition;
            // Dla obiektów UI
            if (pendingObject.GetComponent<RectTransform>() != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    cardArea as RectTransform, 
                    mousePos, 
                    null, // Dla UI używamy null zamiast Camera.main
                    out Vector2 localPoint);
                    
                pendingObject.GetComponent<RectTransform>().anchoredPosition = localPoint;
            }
            // Dla obiektów 3D
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    cardArea as RectTransform, 
                    mousePos, 
                    Camera.main, 
                    out Vector2 localPoint);
                    
                pendingObject.transform.localPosition = localPoint;
            }
        }
    }
}

    public void SelectObject(GameObject prefab)
    {
        selectedPrefab = prefab;
        if (pendingObject != null) Destroy(pendingObject);
        
        pendingObject = Instantiate(prefab, cardArea);
        //pendingObject.transform.localScale = Vector3.one;
        
        SetObjectComponentsEnabled(pendingObject, false);
    }
    public void ConfirmPlacement()
    {
        if (pendingObject == null) return;

        if (!string.IsNullOrEmpty(idInputField.text))
        {
            if(selectedPrefab.name == "InputField")
            {
                switch (GameObject.Find("InputFieldType").GetComponent<TMP_Dropdown>().value)
                {
                    case 0:
                        pendingObject.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Standard;
                        break;
                    case 1:
                        pendingObject.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.IntegerNumber;
                        break;
                    case 2:
                        pendingObject.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.DecimalNumber;
                        break;
                    default:
                        pendingObject.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Standard;
                        break;
                }
            }

            ObjectID objId = pendingObject.GetComponent<ObjectID>();
            if (objId == null) objId = pendingObject.AddComponent<ObjectID>();
            Debug.Log("Dodano " + selectedPrefab.name);

            objId.SetID(idInputField.text, pendingObject, selectedPrefab.name);
            pendingObject.name = idInputField.text;

            SetObjectComponentsEnabled(pendingObject, true);
            pendingObject = null;
            selectedPrefab = null;
        }
        else
        {
            Debug.LogWarning("Nie podano ID - niszczę obiekt");
            Destroy(pendingObject);
            pendingObject = null;
        }

        ResetPlacement();
    }

    public void printDictionary()
    {
        ObjectID.printDictionary();
    }

    public void CancelPlacement()
    {
        if (pendingObject != null) Destroy(pendingObject);
        ResetPlacement();
    }

    private void ResetPlacement()
    {
        pendingObject = null;
        selectedPrefab = null;
        idInputField.text = "";
        confirmationDialog.SetActive(false);
    }

   public static void SetObjectComponentsEnabled(GameObject obj, bool enabled)
{
    // Collidery 2D
    foreach (var collider in obj.GetComponents<Collider2D>())
        collider.enabled = enabled;
    
    // Collidery 3D
    foreach (var collider in obj.GetComponents<Collider>())
        collider.enabled = enabled;
    
    foreach (var behaviour in obj.GetComponents<MonoBehaviour>())
        behaviour.enabled = enabled;
}

}