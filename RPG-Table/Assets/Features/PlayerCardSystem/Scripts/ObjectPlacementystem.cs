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
    
    private string PATH_TO_FILES = SettingsManager._CurrentSettings.playerCardsPrefabPath;
    private string FILE_NAME = "PlayerPrefab.json";
    private string PATH_TO_2D_ASSETS = SettingsManager._CurrentSettings.Assets2DPath;
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

    public GameObject deleteConfirmationDialog;
    private bool isDeleteMode = false;
    private savingCustomObjects objectToDelete;


    [System.Serializable]
    public class PlaceableObject
    {
        public string name;
        public GameObject prefab;
        public string imageName;

    }
    
    public void EnterDeleteMode()
    {
        isDeleteMode = true;
        // You might want to highlight deletable objects or change cursor here
        Debug.Log("Delete mode activated. Click on a prefab to delete it.");
    }
    void Start()
    {
        //PATH_TO_FILES = Path.Combine(Application.persistentDataPath, "PlayerPrefab/");
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
            button.name = obj.name;
            button.GetComponent<Button>().onClick.AddListener(() => SelectObject(obj.prefab));
        }
       if (File.Exists(Path.Combine(PATH_TO_FILES, FILE_NAME)))
       {
            string json = File.ReadAllText(Path.Combine(PATH_TO_FILES, FILE_NAME));
            SavePrefabData savePrefabData = JsonUtility.FromJson<SavePrefabData>(json);
            userObjects = savePrefabData.prefabs;

            foreach (var obj in userObjects) //initialize buttons
            {
                GameObject button = Instantiate(objectButtonPrefab, objectsPanel);
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                buttonText.text = obj.name;
                button.name = obj.name;
                GameObject prefab = Resources.Load<GameObject>(obj.prefab);
  
                if(File.Exists(Path.Combine(PATH_TO_2D_ASSETS, obj.imageName)))
                {   
                    byte[] imageBytes = File.ReadAllBytes(Path.Combine(PATH_TO_2D_ASSETS, obj.imageName));
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageBytes); // Load the image data into the texture
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    button.GetComponent<Button>().image.sprite = sprite;
                    Debug.Log("Obrazek dodany");
                }
                else
                {
                    Debug.Log("Nie ma obrazka");
                }
                button.GetComponent<Button>().onClick.AddListener(() => SelectObject(prefab,obj));
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

            bool isInsideCardArea = RectTransformUtility.RectangleContainsScreenPoint(
                cardArea as RectTransform,
                Input.mousePosition,
                null
            );
            if (Input.GetMouseButtonDown(0) && isInsideCardArea) 
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
    public void SelectObject(GameObject prefab, savingCustomObjects obj = null)
    {

        if (isDeleteMode && obj != null)
        {
            objectToDelete = obj;
            deleteConfirmationDialog.SetActive(true);
            deleteConfirmationDialog.transform.Find("PrefabNamePlace").GetComponent<TextMeshProUGUI>().text = obj.name;
            return;
        }


        selectedPrefab = prefab;
        if (pendingObject != null) Destroy(pendingObject);
        
        pendingObject = Instantiate(prefab, cardArea);

        if(obj!= null)
        {

            CardAreaSaver cardAreaSaver = new CardAreaSaver();
            cardAreaSaver.SetImage(pendingObject, obj.imageName);
            pendingObject.GetComponent<RectTransform>().sizeDelta = new Vector2(float.Parse(obj.Width), float.Parse(obj.Height));
            if(obj.prefab != "Image")
            {
                Color newColor;
                if(pendingObject.GetComponent<TextMeshProUGUI>() != null)
                {
                    pendingObject.GetComponent<TextMeshProUGUI>().fontSize = float.Parse(obj.fontSize);

                    FontStyles style = FontStyles.Normal;
                    if(obj.isBold)
                        style |= FontStyles.Bold;
                    if(obj.isItalic)
                        style |= FontStyles.Italic;
                    pendingObject.GetComponent<TextMeshProUGUI>().fontStyle = style;

                    ColorUtility.TryParseHtmlString("#" + obj.fontColor, out newColor);
                    pendingObject.GetComponent<TextMeshProUGUI>().color = newColor;
                } else if(pendingObject.GetComponent<TMP_Text>() != null)
                {
                    pendingObject.GetComponent<TMP_Text>().fontSize = float.Parse(obj.fontSize);

                    FontStyles style = FontStyles.Normal;
                    if(obj.isBold)
                        style |= FontStyles.Bold;
                    if(obj.isItalic)
                        style |= FontStyles.Italic;
                    pendingObject.GetComponent<TMP_Text>().fontStyle = style;

                    ColorUtility.TryParseHtmlString("#" + obj.fontColor, out newColor);
                    pendingObject.GetComponent<TMP_Text>().color = newColor;
                }
            }
            if(obj.prefab != "TextBlockPrefab")
            {
                Image image = pendingObject.GetComponent<Image>();
                pendingObject.GetComponent<Image>().color = new Color(image.color.r, image.color.g, image.color.b,
                    Mathf.Clamp01(float.Parse(obj.transparency)/100));
            }

        }
        
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

    public void ConfirmDeletion()
    {
        if (objectToDelete != null)
        {
            // Remove from list
            userObjects.Remove(objectToDelete);
            
            // Update save file
            SavePrefabData savePrefabData = new SavePrefabData{
                saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                prefabCount = userObjects.Count,
                prefabs = userObjects
            };
            string json = JsonUtility.ToJson(savePrefabData, true);
            File.WriteAllText(Path.Combine(PATH_TO_FILES, FILE_NAME), json);
            
            // Refresh UI (you might need to implement this)
            RefreshObjectButtons();
        }
        
        CancelDeletion();
    }

    public void CancelDeletion()
    {
        isDeleteMode = false;
        objectToDelete = null;
        deleteConfirmationDialog.SetActive(false);
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

    private void RefreshObjectButtons()
{
    // Clear existing buttons
    foreach (Transform child in objectsPanel)
    {
        Destroy(child.gameObject);
    }
    
    // Recreate buttons for available objects
    foreach (var obj in availableObjects)
    {
        GameObject button = Instantiate(objectButtonPrefab, objectsPanel);
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        buttonText.text = obj.name;
        button.name = obj.name;
        button.GetComponent<Button>().onClick.AddListener(() => SelectObject(obj.prefab));
    }
    
    // Recreate buttons for user objects
    foreach (var obj in userObjects)
    {
        GameObject button = Instantiate(objectButtonPrefab, objectsPanel);
        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        buttonText.text = obj.name;
        button.name = obj.name;
        GameObject prefab = Resources.Load<GameObject>(obj.prefab);
        
        if(File.Exists(Path.Combine(PATH_TO_2D_ASSETS, obj.imageName)))
        {   
            byte[] imageBytes = File.ReadAllBytes(Path.Combine(PATH_TO_2D_ASSETS, obj.imageName));
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            button.GetComponent<Button>().image.sprite = sprite;
        }
        button.GetComponent<Button>().onClick.AddListener(() => SelectObject(prefab, obj));
    }
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
        
    }
    public void onCustomPrefabSaveButtonClick(savingCustomObjects customObject)
    {
        customObject.name = CustomPrefabCreator.transform.Find("PrefabName").GetComponent<TMP_InputField>().text;
        userObjects.Add(customObject);
        SavePrefabData savePrefabData = new SavePrefabData{
            saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            prefabCount = userObjects.Count,
            prefabs = userObjects
        };
        string json = JsonUtility.ToJson(savePrefabData, true);
        string filePath = Path.Combine(PATH_TO_FILES, FILE_NAME);
        File.WriteAllText(filePath, json);

        CustomPrefabCreator.SetActive(false);
        CustomPrefabCreator.transform.Find("PrefabName").GetComponent<TMP_InputField>().text = "";

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
        public string transparency;
        public string text;
        public string fontSize;
        public string fontColor;
        public bool isBold;
        public bool isItalic;
        public string Width;
        public string Height;
    }