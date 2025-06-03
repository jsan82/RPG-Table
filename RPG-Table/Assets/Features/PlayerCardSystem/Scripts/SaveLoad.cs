using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// System for saving and loading card-based UI layouts with full serialization
/// </summary>
/// <remarks>
/// Handles persistence of UI elements including their positions, appearances,
/// and custom behaviors. Supports both editor and runtime usage.
/// </remarks>
public class CardAreaSaver : MonoBehaviour
{
    /// <summary>Header for settings section</summary>
    [Header("Settings")]
    /// <summary>Filename for saving</summary>
    public static string saveFileName;
    /// <summary>Enable debug logging</summary>
    public bool debugLog = true;
    /// <summary>Parent transform for card area</summary>
    public Transform cardArea;
    /// <summary>Complete path for save file</summary>
    public static string _fullSavePath;

    /// <summary>Path to 2D assets directory</summary>
    private string PATH_TO_2D_ASSETS = SettingsManager._CurrentSettings.Assets2DPath;

    /// <summary>Global dictionary tracking all UI objects by ID</summary>    
    public static Dictionary<string, GameObject> _objectDictionary = new Dictionary<string, GameObject>();

    /// <summary>
    /// Called when the script instance is being loaded
    /// </summary>
    void Awake()
    {
        // Initialization if needed
    }

    /// <summary>
    /// Saves the current card area layout to JSON
    /// </summary>
    /// <remarks>
    /// Captures:
    /// - Object transforms and visibility
    /// - Custom component states
    /// - Button operations
    /// - Visual properties (colors, text, etc.)
    /// </remarks>
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

    /// <summary>
    /// Static method to save card area to specified file path
    /// </summary>
    /// <param name="filePath">Path to save file</param>
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

    /// <summary>
    /// Static method to load card area from specified file path
    /// </summary>
    /// <param name="filePath">Path to load file</param>
    /// <param name="Editing">Whether to enable editing functionality</param>
    public static void LoadCardArea(string filePath, bool Editing = true)
    {
        saveFileName = filePath;
        CardAreaSaver instance = FindObjectOfType<CardAreaSaver>();
        if (instance != null)
        {
            instance.LoadCardArea(Editing);
        }
        else
        {
            Debug.LogError("CardAreaSaver instance not found in the scene.");
        }
    }

    /// <summary>
    /// Loads a card area layout from JSON
    /// </summary>
    /// <param name="Editing">If true, enables editing functionality</param>
    /// <remarks>
    /// Reconstructs the entire UI hierarchy from serialized data.
    /// Can operate in either editor or play mode.
    /// </remarks>
    public void LoadCardArea(bool Editing)
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
            NewBehaviourScript.Instance.clearDictionary();

            foreach (ChildData childData in loadedData.children)
            {
                if (childData == null) continue;
                GameObject newChild = CreateChildFromData(childData);
                if (!Editing)
                {
                    newChild.GetComponent<SmartDragHandler>().enabled = false;
                }
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

    /// <summary>
    /// Clears all objects from the card area
    /// </summary>
    public void ClearCardArea()
    {
        _objectDictionary = new Dictionary<string, GameObject>();
        foreach (Transform child in cardArea)
        {
            Destroy(child.gameObject.GetComponent<ObjectID>());
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Deletes an object by ID
    /// </summary>
    /// <param name="id">ID of object to delete</param>
    public void deleteObject(string id)
    {
        if (_objectDictionary.ContainsKey(id))
        {
            NewBehaviourScript.Instance.deleteKey(id);
            GameObject toKill = _objectDictionary[id];
            Destroy(toKill.GetComponent<ObjectID>());
            Destroy(toKill);
            _objectDictionary.Remove(id);
        }
    }

    /// <summary>
    /// Creates a UI object from serialized data
    /// </summary>
    /// <param name="childData">Serialized object data</param>
    /// <param name="dragOn">Enable drag functionality</param>
    /// <returns>Reconstructed GameObject</returns>
    public virtual GameObject CreateChildFromData(ChildData childData, bool dragOn = true)
    {
        GameObject prefab = Resources.Load<GameObject>(childData.objectType);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {childData.objectType}");
            return null;
        }

        GameObject newChild = Instantiate(prefab, cardArea);
        Debug.Log($"Creating child at {newChild.transform.localPosition} with rotation {newChild.transform.localRotation}");
        newChild.transform.SetParent(cardArea, false);

        newChild.transform.localScale = childData.localScale;
        ((RectTransform)newChild.transform).sizeDelta = new Vector2(float.Parse(childData.Width), float.Parse(childData.Height));

        if (newChild.GetComponent<Image>() != null)
        {
            newChild.GetComponent<Image>().color = new Color(
                newChild.GetComponent<Image>().color.r,
                newChild.GetComponent<Image>().color.g,
                newChild.GetComponent<Image>().color.b,
                Mathf.Clamp01(float.Parse(childData.transparency) / 100));
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

                    if (childData.isBold)
                    {
                        textComponent.fontStyle |= FontStyles.Bold;
                    }
                    if (childData.isItalic)
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
                    if (childData.isBold)
                    {
                        textBlock.fontStyle |= FontStyles.Bold;
                    }
                    if (childData.isItalic)
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
                    if (childData.isBold)
                    {
                        inputField.textComponent.fontStyle |= FontStyles.Bold;
                    }
                    if (childData.isItalic)
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
        
        newChild.transform.localPosition = childData.localPosition;
        newChild.transform.localRotation = childData.localRotation;
        Debug.Log($"Setting local position to {newChild.transform.localPosition}");
        newChild.SetActive(childData.isActive);

        ObjectID objID = newChild.GetComponent<ObjectID>();
        if (objID == null)
        {
            objID = newChild.AddComponent<ObjectID>();
        }
        objID.SetID(childData.objectID, newChild, childData.objectType);
        newChild.name = childData.objectID;

        ObjectPlacementSystem.SetObjectComponentsEnabled(newChild, true);
        return newChild;
    }

    /// <summary>
    /// Gets the current save path
    /// </summary>
    /// <returns>Current save path</returns>
    public string GetSavePath()
    {
        return _fullSavePath;
    }

    /// <summary>
    /// Sets an image on a UI object from file
    /// </summary>
    /// <param name="obj">Target object</param>
    /// <param name="imageName">Filename of the image</param>
    public void SetImage(GameObject obj, string imageName)
    {
        if (obj != null && imageName != null)
        {
            Debug.Log("GameObj and image not null");
        }
        
        if (File.Exists(Path.Combine(PATH_TO_2D_ASSETS, imageName)))
        {
            byte[] imageBytes = File.ReadAllBytes(Path.Combine(PATH_TO_2D_ASSETS, imageName));
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            sprite.name = imageName;
            if (obj.GetComponent<Image>() != null)
            {
                obj.GetComponent<Image>().sprite = sprite;
            }
            else
            {
                Debug.Log("Image component missing");
            }
        }
    }

    /// <summary>
    /// Destructor for cleanup
    /// </summary>
    ~CardAreaSaver()
    {
        if (debugLog)
        {
            Debug.Log("CardAreaSaver destroyed");
        }
    }
}

/// <summary>
/// Top-level container for saved layout data
/// </summary>
[System.Serializable]
public class SaveData
{
    /// <summary>Timestamp of save</summary>
    public string saveTime;
    /// <summary>Number of child objects</summary>
    public int childCount;
    /// <summary>List of child data</summary>
    public List<ChildData> children;
}

/// <summary>
/// Complete serialization data for a UI element
/// </summary>
[System.Serializable]
public class ObjectDictData
{
    /// <summary>Object identifier</summary>
    public string objectID;
    /// <summary>Object name</summary>
    public string objectName;
}

/// <summary>
/// Serialized script data
/// </summary>
[Serializable]
public class ScriptData
{
    /// <summary>Type of script</summary>
    public string type;
    /// <summary>Serialized script data</summary>
    public string data;
}

/// <summary>
/// Complete data for a child UI element
/// </summary>
[System.Serializable]
public class ChildData
{
    /// <summary>Unique identifier for the object</summary>
    public string objectID; 
    /// <summary>Type of UI object</summary>
    public string objectType;
    /// <summary>Text content</summary>
    public string Text;
    /// <summary>Font size</summary>
    public string fontSize;
    /// <summary>Font color in hex</summary>
    public string fontColor;
    /// <summary>Whether text is bold</summary>
    public bool isBold;
    /// <summary>Whether text is italic</summary>
    public bool isItalic;
    /// <summary>Width of object</summary>
    public string Width;
    /// <summary>Height of object</summary>
    public string Height;
    /// <summary>Background image name</summary>
    public string backgroundImage;
    /// <summary>Transparency percentage</summary>
    public string transparency;
    /// <summary>Input field type</summary>
    public string inputType;
    /// <summary>Local position</summary>
    public Vector3 localPosition;
    /// <summary>Local rotation</summary>
    public Quaternion localRotation;
    /// <summary>Local scale</summary>
    public Vector3 localScale;
    /// <summary>Whether object is active</summary>
    public bool isActive;
    /// <summary>List of attached scripts</summary>
    public List<ScriptData> scripts = new List<ScriptData>();
    /// <summary>Current operations (for buttons)</summary>
    public string currentOperations;

    /// <summary>
    /// Constructor that creates ChildData from a Transform
    /// </summary>
    /// <param name="child">Transform to serialize</param>
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