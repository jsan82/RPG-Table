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
    
        if (cardArea == null)
        {
            GameObject area = GameObject.Find("CardArea");
            if (area != null) cardArea = area.transform;
            else Debug.LogError("CardArea not in scene!");
        }

        if (availableObjects == null)
            Debug.LogError("availableObjects is null!");
        else if (availableObjects.Count == 0)
            Debug.LogError("availableObjects is empty!");

        if (objectsPanel == null)
            Debug.LogError("objectsPanel is not asigned!");

        if (objectButtonPrefab == null)
            Debug.LogError("objectButtonPrefab is not asigned!");

        foreach (var obj in availableObjects) //initialize buttons
        {
            GameObject button = Instantiate(objectButtonPrefab, objectsPanel);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = obj.name;
            button.GetComponent<Button>().onClick.AddListener(() => SelectObject(obj.prefab));
        }
    }

   void Update()
    {
        if (pendingObject != null) //checkign if we are placing an object
        {
            if (Input.GetMouseButtonDown(0)) 
                {
                    if(selectedPrefab.name == "InputField") //checking for input field
                    {
                        inputTypeDropdown.SetActive(true);
                    }
                    else
                    {
                        inputTypeDropdown.SetActive(false);
                    }
                    confirmationDialog.SetActive(true);
                }

            if(!confirmationDialog.activeSelf){ //if we are not placing an object, we can move it
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
                
            }
        }
    }

    // Method to select an object prefab from the list and instantiate it in the card area
    public void SelectObject(GameObject prefab)
    {
        selectedPrefab = prefab;
        if (pendingObject != null) Destroy(pendingObject);
        
        pendingObject = Instantiate(prefab, cardArea);
        //pendingObject.transform.localScale = Vector3.one;
        
        SetObjectComponentsEnabled(pendingObject, false);
    }

    // Method to confirm the placement of the object and set its ID
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
            //Checking if the object has an ObjectID component, if not, add it
            ObjectID objId = pendingObject.GetComponent<ObjectID>();
            if (objId == null) objId = pendingObject.AddComponent<ObjectID>();

            objId.SetID(idInputField.text, pendingObject, selectedPrefab.name);
            pendingObject.name = idInputField.text;

            SetObjectComponentsEnabled(pendingObject, true);
            pendingObject = null;
            selectedPrefab = null;
        }
        else
        {
            Debug.LogWarning("ID not assigned!");
            Destroy(pendingObject);
            pendingObject = null;
        }

        ResetPlacement();
    }

    //Method for printing the dictionary of objects
    public void printDictionary()
    {
        ObjectID.printDictionary();
    }

    // Method to cancel the placement of the object and reset the state
    public void CancelPlacement()
    {
        if (pendingObject != null) Destroy(pendingObject);
        ResetPlacement();
    }

    // Method to reset the placement state and hide the confirmation dialog
    private void ResetPlacement()
    {
        pendingObject = null;
        selectedPrefab = null;
        idInputField.text = "";
        confirmationDialog.SetActive(false);
    }

    // Method to enable or disable all components of the object (except for the ObjectID component)
   public static void SetObjectComponentsEnabled(GameObject obj, bool enabled)
    {
        // Collidery 2D
        foreach (var collider in obj.GetComponents<Collider2D>())
            collider.enabled = enabled;
        
        
        foreach (var behaviour in obj.GetComponents<MonoBehaviour>())
            behaviour.enabled = enabled;
    }

}