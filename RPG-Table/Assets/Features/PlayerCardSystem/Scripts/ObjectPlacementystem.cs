using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ObjectPlacementSystem : MonoBehaviour
{
    
    private string PATH_TO_FILES;
    private string FILE_NAME = "PlayerPrefab.json";
    public List<PlaceableObject> availableObjects;
    public List<savingCustomObjects> userObjects;
    public Transform objectsPanel;
    public Transform cardArea;
    public GameObject objectButtonPrefab;
    public GameObject confirmationDialog;
    public GameObject CustomPrefabCreator;
    public GameObject inputTypeDropdown;
    public InputField idInputField;
    private GameObject selectedPrefab;
    private GameObject pendingObject;

    [System.Serializable]
    public class PlaceableObject
    {
        public string name;
        public GameObject prefab;
        public string imageName;
    }
    void Start()
    {
        PATH_TO_FILES = Path.Combine(Application.persistentDataPath, "PlayerPrefab/");
        if (!Directory.Exists(PATH_TO_FILES))
        {
            Directory.CreateDirectory(PATH_TO_FILES);
        }
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
       if (File.Exists(Path.Combine(PATH_TO_FILES, FILE_NAME)))
       {
            string json = File.ReadAllText(Path.Combine(PATH_TO_FILES, FILE_NAME));
            SavePrefabData savePrefabData = JsonUtility.FromJson<SavePrefabData>(json);
            userObjects = savePrefabData.prefabs;
            Debug.Log("Loaded " + userObjects.Count + " prefabs from file.");
            foreach (var obj in userObjects) //initialize buttons
            {
                GameObject button = Instantiate(objectButtonPrefab, objectsPanel);
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                buttonText.text = obj.name;
                GameObject prefab = Resources.Load<GameObject>(obj.prefab);
                button.GetComponent<Button>().onClick.AddListener(() => SelectObject(prefab));
            }
       }
    }

   void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape) && confirmationDialog.activeSelf) //checking if we are placing an object and pressing escape
        {
            CancelPlacement();
        }
        if (pendingObject != null && !CustomPrefabCreator.activeSelf) //checkign if we are placing an object
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

    public void onCustomPrefabCreatorButtonClick()
    {
        CustomPrefabCreator.SetActive(true);
    }

    public void onCustomPrefabCreatorCloseButtonClick()
    {
        CustomPrefabCreator.SetActive(false);
        CustomPrefabCreator.transform.Find("PrefabName").GetComponent<TMP_InputField>().text = "";
        CustomPrefabCreator.transform.Find("PrefabType").GetComponent<TMP_Dropdown>().value = 0;
        CustomPrefabCreator.transform.Find("ImageName").GetComponent<TMP_InputField>().text = "";
    }
    public void onCustomPrefabSaveButtonClick()
    {
        string prefabName = CustomPrefabCreator.transform.Find("PrefabName").GetComponent<TMP_InputField>().text;
        string prefabType = CustomPrefabCreator.transform.Find("PrefabType").GetComponent<TMP_Dropdown>().options[
            CustomPrefabCreator.transform.Find("PrefabType").GetComponent<TMP_Dropdown>().value].text;
        Debug.Log("Saving prefab as: " + prefabName + " of type: " + prefabType);
    
        // creating the prefab
        PlaceableObject newObject = new PlaceableObject();
        savingCustomObjects customObject = new savingCustomObjects();
        customObject.name = prefabName;
        customObject.imageName = CustomPrefabCreator.transform.Find("ImageName").GetComponent<TMP_InputField>().text;
        customObject.prefab = prefabType;


        newObject.name = prefabName;
        newObject.imageName = CustomPrefabCreator.transform.Find("ImageName").GetComponent<TMP_InputField>().text;
        newObject.prefab = Resources.Load<GameObject>(prefabType);

        // Saving the prefab to the list
        userObjects.Add(customObject);
       
        // adding the prefab to the list of available objects
        GameObject button = Instantiate(objectButtonPrefab, objectsPanel);
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        buttonText.text = newObject.name;
        button.GetComponent<Button>().onClick.AddListener(() => SelectObject(newObject.prefab));

        //saving the prefab to the file
        SavePrefabData savePrefabData = new SavePrefabData{
            saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            prefabCount = userObjects.Count,
            prefabs = userObjects
        };
        string json = JsonUtility.ToJson(savePrefabData, true);
        string filePath = Path.Combine(PATH_TO_FILES, FILE_NAME);
        File.WriteAllText(filePath, json);


    }

}

[System.Serializable]
public class SavePrefabData
{
    public string saveTime;
    public int prefabCount;
    public List<savingCustomObjects> prefabs;
}

    [System.Serializable]
    public class savingCustomObjects
    {
        public string name;
        public string prefab;
        public string imageName;
    }