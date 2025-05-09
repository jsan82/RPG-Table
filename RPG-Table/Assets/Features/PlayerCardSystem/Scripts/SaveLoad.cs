using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class CardAreaSaver : MonoBehaviour
{
    [Header("Settings")]
    public static string saveFileName;
    public bool debugLog = true;
    public Transform cardArea;
    private string _fullSavePath;
    
    private string PATH_TO_2D_ASSETS = SettingsManager._CurrentSettings.Assets2DPath;

    public static Dictionary<string, GameObject> _objectDictionary = new Dictionary<string, GameObject>();//Dictionary to store objects by ID

    

    void Awake()
    {
        //_fullSavePath = Path.Combine(Application.persistentDataPath, saveFileName);
        
    }

    // Method to save the card area to a JSON file
    public void SaveCardArea()
    {
        try
        {
            _fullSavePath = saveFileName;
            List<ChildData> childrenData = new List<ChildData>();

            foreach (Transform child in cardArea)
            {
                if (child.gameObject.activeSelf)
                {
                    ChildData childData = new ChildData(child);
                    MonoBehaviour[] scripts = child.GetComponents<MonoBehaviour>();
                    foreach (var script in scripts)
                    {
                        if (script == null || script.GetType() == typeof(ObjectID)) continue;
                        
                        ScriptData scriptData = new ScriptData
                        {
                            type = script.GetType().AssemblyQualifiedName,
                            data = JsonUtility.ToJson(script)
                        };
                        childData.scripts.Add(scriptData);
                    }
                    
                    // Save button operations if this is a button
                    if (childData.objectType == "Button")
                    {
                        string operations = NewBehaviourScript.Instance.GetOperationsForObject(childData.objectID);
                        if (!string.IsNullOrEmpty(operations))
                        {
                            childData.currentOperations = operations;
                        }
                    }
                    
                    childrenData.Add(childData);
                }
            }
            
            SaveData saveData = new SaveData
            {
                saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                childCount = childrenData.Count,
                children = childrenData
            };

            string jsonData = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(_fullSavePath, jsonData);

            if (debugLog)
            {
                Debug.Log($"Saved to: {_fullSavePath}");
                Debug.Log(jsonData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Save error: {e.Message}\n{e.StackTrace}");
        }
    }

    public static void SaveCardArea(string filePath)
    {
        saveFileName = filePath;
        CardAreaSaver instance = FindObjectOfType<CardAreaSaver>();
        if (instance != null)
        {
            instance.SaveCardArea();
        }
        else
        {
            Debug.LogError("CardAreaSaver instance not found in the scene.");
        }
    }
    
    // Method to load the card area from a JSON file
    public static void LoadCardArea(string filePath)
    {
        saveFileName = filePath;
        CardAreaSaver instance = FindObjectOfType<CardAreaSaver>();
        if (instance != null)
        {
            instance.LoadCardArea();
        }
        else
        {
            Debug.LogError("CardAreaSaver instance not found in the scene.");
        }
    }
    public void LoadCardArea()
    {
        try
        {
            _fullSavePath = saveFileName;
        
            // Ensure the directory exists
            string directory = Path.GetDirectoryName(_fullSavePath);
            Debug.Log($"Directory: {directory}");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (!File.Exists(_fullSavePath))
            {
                Debug.LogWarning($"Save file doesn't exist: {_fullSavePath}");
                return;
            }

            string jsonData = File.ReadAllText(_fullSavePath);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(jsonData);

            ClearCardArea();
             //_objectDictionary = new Dictionary<string, GameObject>();
            NewBehaviourScript.Instance.clearDictionary();

            foreach (ChildData childData in loadedData.children)
            {
                if (childData == null) continue;
                GameObject newChild = CreateChildFromData(childData);
                SetImage(newChild, childData.backgroundImage);
            }
            foreach (ChildData childData in loadedData.children)
            {
                if (childData == null || string.IsNullOrEmpty(childData.currentOperations)) continue;
                
                if (childData.objectType == "Button")
                {
                    NewBehaviourScript.Instance.loadOperations(childData.objectID, childData.currentOperations);
                    if (debugLog) Debug.Log($"Loaded operations for {childData.objectID}: {childData.currentOperations}");
                }
            }

            if (debugLog)
            {
                Debug.Log($"Loaded from: {_fullSavePath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Load error: {e.Message}\n{e.StackTrace}");
        }
    }

    public void ClearCardArea()
    {
        _objectDictionary = new Dictionary<string, GameObject>();
        foreach (Transform child in cardArea) //Del objectid script from object
        {
            Destroy(child.gameObject.GetComponent<ObjectID>()); 
            Destroy(child.gameObject);
        }   
    }

    public void deleteObject(string id)
    {
        if(_objectDictionary.ContainsKey(id))
        {
            NewBehaviourScript.Instance.deleteKey(id);
            GameObject toKill = _objectDictionary[id];
            Destroy(toKill.GetComponent<ObjectID>());
            Destroy(toKill);
            _objectDictionary.Remove(id);
        }
    }

    public virtual GameObject CreateChildFromData(ChildData childData, bool dragOn = true)
    {
        GameObject prefab = Resources.Load<GameObject>(childData.objectType);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {childData.objectType}");
            return null;
        }

        GameObject newChild = Instantiate(prefab, cardArea);
        newChild.transform.localPosition = childData.localPosition;
        newChild.transform.localRotation = childData.localRotation;
        newChild.transform.localScale = childData.localScale;
        
        ((RectTransform)newChild.transform).sizeDelta =new Vector2(float.Parse(childData.Width),float.Parse(childData.Height));
        
        if(newChild.GetComponent<Image>() != null){
            newChild.GetComponent<Image>().color = new Color( 
                newChild.GetComponent<Image>().color.r,
                newChild.GetComponent<Image>().color.g,
                newChild.GetComponent<Image>().color.b,
                Mathf.Clamp01(float.Parse(childData.transparency)/100));
        }
        Color newColor;
        // Handle different object types
        switch (childData.objectType)
        {
            case "Button":
                var textComponent = newChild.GetComponentInChildren<TMP_Text>();
                if (textComponent != null)
                {
                    textComponent.text = childData.Text;
                    
                    if(childData.isBold)
                    {
                        textComponent.fontStyle |= FontStyles.Bold;
                    }
                    if(childData.isItalic)
                    {
                        textComponent.fontStyle |= FontStyles.Italic;
                    }
                    ColorUtility.TryParseHtmlString("#" + childData.fontColor, out newColor);
                    textComponent.color = newColor;
                    textComponent.fontSize = float.Parse(childData.fontSize);
                }
                
                break;
                
            case "TextBlockPrefab":
                var textBlock = newChild.GetComponent<TextMeshProUGUI>();
                if (textBlock != null && !string.IsNullOrEmpty(childData.Text))
                {
                    textBlock.text = childData.Text;
                    textBlock.text = childData.Text;
                    if(childData.isBold)
                    {
                        textBlock.fontStyle|= FontStyles.Bold;
                    }
                    if(childData.isItalic)
                    {
                        textBlock.fontStyle |= FontStyles.Italic;
                    }
                    ColorUtility.TryParseHtmlString("#" + childData.fontColor, out newColor);
                    textBlock.color = newColor;
                    textBlock.fontSize = float.Parse(childData.fontSize);
                }

                break;
                
            case "InputField":
                var inputField = newChild.GetComponent<TMP_InputField>();
                if (inputField != null)
                {
                    inputField.text = childData.Text;
                    inputField.pointSize = float.Parse(childData.fontSize);
                     if(childData.isBold)
                    {
                        inputField.textComponent.fontStyle |= FontStyles.Bold;
                    }
                    if(childData.isItalic)
                    {
                        inputField.textComponent.fontStyle |= FontStyles.Italic;
                    }
                    ColorUtility.TryParseHtmlString("#" + childData.fontColor, out newColor);
                    inputField.textComponent.color = newColor;  
                    
                    var inputImage = newChild.GetComponentInChildren<Image>();
                    if (inputImage != null)
                    {
                        Sprite inputSprite = Resources.Load<Sprite>(childData.backgroundImage);
                        if (inputSprite != null)
                        {
                            inputImage.sprite = inputSprite;
                        }
                    }
                    inputField.text = childData.Text;
                    
                    inputField.contentType = TMP_InputField.ContentType.Standard;
                    if (!string.IsNullOrEmpty(childData.inputType))
                    {
                        
                        switch (childData.inputType)
                        {
                            case "InputFieldStandard":
                                inputField.contentType = TMP_InputField.ContentType.Standard;
                                break;
                            case "InputFieldDecimalNumber":
                                inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                                break;
                            case "InputFieldIntegerNumber":
                                inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                                break;
                        }
                    }
                }
                break;
        }

        newChild.SetActive(childData.isActive);

        ObjectID objID = newChild.GetComponent<ObjectID>();
        if (objID == null) 
        {
            objID = newChild.AddComponent<ObjectID>();
        }
        objID.SetID(childData.objectID, newChild, childData.objectType);
        newChild.name = childData.objectID;

        /*

        // IMPORTANT TO OVERIDE/DISABLE SOMETHING IN THIS CLASS FOR PURPOSE OF LOADING THIS IN GAME TO DISABLE THE LOADING DRAG AND DROP FUNCTIONALITY

        */
        // SmartDragHandler dragComponent = newChild.GetComponent<SmartDragHandler>();
        // if (dragComponent != null)
        // {
        //    newChild.AddComponent<SmartDragHandler>(); 
        // }
        //newChild.GetComponent<SmartDragHandler>().enabled = dragOn; // Disable drag functionality for loaded objects

        ObjectPlacementSystem.SetObjectComponentsEnabled(newChild, true);
        return newChild;
    }

    public string GetSavePath()
    {
        return _fullSavePath;
    }

    public void SetImage(GameObject obj, string imageName)
    {   
        if(obj != null){
            Debug.Log("GameObj not null");
            if(imageName!=null){
                Debug.Log("image not null");
            }
        }
        if(File.Exists(Path.Combine(PATH_TO_2D_ASSETS, imageName)))
                {   
                    byte[] imageBytes = File.ReadAllBytes(Path.Combine(PATH_TO_2D_ASSETS, imageName));
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageBytes); // Load the image data into the texture
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    sprite.name = imageName;
                    if (obj.GetComponent<Image>() != null)
                    {
                        obj.GetComponent<Image>().sprite = sprite;
                    }
                    else{
                        Debug.Log("image component existingn't");
                    }
                }   
                   
    }

    

    ~CardAreaSaver()
    {
        if (debugLog)
        {
            Debug.Log("CardAreaSaver destroyed");
        }
    }
}

[System.Serializable]
public class SaveData
{
    public string saveTime;
    public int childCount;
    public List<ChildData> children;
}

[System.Serializable]
public class ObjectDictData
{
    public string objectID;
    public string objectName;
}

[Serializable]
public class ScriptData
{
    public string type;
    public string data;
}

[System.Serializable]
public class ChildData
{
    public string objectID; 
    public string objectType;
    public string Text;
    public string fontSize;
    public string fontColor;
    public bool isBold;
    public bool isItalic;
    public string Width;
    public string Height;
    public string backgroundImage;
    public string transparency;
    public string inputType;
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;
    public bool isActive;
    public List<ScriptData> scripts = new List<ScriptData>();
    public string currentOperations; // Changed from List<OperationData> to string

    
    public ChildData(Transform child)
    {
        ObjectID objID = child.GetComponent<ObjectID>();
        if (objID == null)
        {
            Debug.LogError($"ObjectID component missing on {child.name}");
            return;
        }

        objectID = child.name;
        objectType = objID.GetPrefab();
        
        Width = ((RectTransform)child).rect.width.ToString("F2");
        Height = ((RectTransform)child).rect.height.ToString("F2");

        if (objectType == "TextBlockPrefab")
        {
            var textComp = child.GetComponent<TextMeshProUGUI>();
            if (textComp != null)
            {
                Text = textComp.text;
                isItalic = (textComp.fontStyle & FontStyles.Italic) == FontStyles.Italic;
                isBold = (textComp.fontStyle & FontStyles.Bold) == FontStyles.Bold;
                fontSize = textComp.fontSize.ToString("F0");
                fontColor = ColorUtility.ToHtmlStringRGB(textComp.color);
            }
        }
        else if (objectType == "Button")
        {
            var textComp = child.GetComponentInChildren<TMP_Text>();
            if (textComp != null)
            {
                Text = textComp.text;
                isItalic = (textComp.fontStyle & FontStyles.Italic) == FontStyles.Italic;
                isBold = (textComp.fontStyle & FontStyles.Bold) == FontStyles.Bold;
                fontSize = textComp.fontSize.ToString("F0");
                fontColor = ColorUtility.ToHtmlStringRGB(textComp.color);
            }

            var imageComp = child.GetComponentInChildren<Image>();
            if (imageComp != null && imageComp.sprite != null)
            {
                backgroundImage = imageComp.sprite.name;
                transparency = (imageComp.color.a * 100).ToString("F2");
            }

            string operations = NewBehaviourScript.Instance.GetOperationsForObject(objectID);
            if (!string.IsNullOrEmpty(operations))
            {
                currentOperations = operations;
            }
        }
        else if (objectType == "InputField")
        {
            var inputField = child.GetComponent<TMP_InputField>();
            if (inputField != null)
            {
                Text = inputField.text;
                isItalic = (inputField.textComponent.fontStyle & FontStyles.Italic) == FontStyles.Italic;
                isBold = (inputField.textComponent.fontStyle & FontStyles.Bold) == FontStyles.Bold;
                fontSize = inputField.pointSize.ToString("F0");
                fontColor = ColorUtility.ToHtmlStringRGB(inputField.textComponent.color);

                var imageComp = child.GetComponent<Image>();
                if (imageComp != null && imageComp.sprite != null)
                {
                    backgroundImage = imageComp.sprite.name;
                    transparency = (imageComp.color.a * 100).ToString("F2");
                }
                
                switch (inputField.contentType)
                {
                    case TMP_InputField.ContentType.Standard:
                        inputType = "InputFieldStandard";
                        break;
                    case TMP_InputField.ContentType.DecimalNumber:
                        inputType = "InputFieldDecimalNumber";
                        break;
                    case TMP_InputField.ContentType.IntegerNumber:
                        inputType = "InputFieldIntegerNumber";
                        break;
                }
            }
        }

        
        if(child.GetComponent<Image>()!=null){
            backgroundImage = child.GetComponent<Image>().sprite.name;
            transparency = (child.GetComponent<Image>().color.a * 100).ToString("F2");
        }
        localPosition = child.localPosition;
        localRotation = child.localRotation;
        localScale = child.localScale;
        isActive = child.gameObject.activeSelf;
    }
}