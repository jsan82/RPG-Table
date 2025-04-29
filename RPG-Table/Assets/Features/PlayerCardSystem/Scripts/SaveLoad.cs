using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CardAreaSaver : MonoBehaviour
{
    [Header("Settings")]
    public string saveFileName = "cardAreaSave.json";
    public bool debugLog = true;
    public Transform cardArea;
    private string fullSavePath;

    void Awake()
    {
        fullSavePath = Path.Combine(Application.persistentDataPath, saveFileName);
        
        // Ensure the directory exists
        string directory = Path.GetDirectoryName(fullSavePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    // Method to save the card area to a JSON file
    public void SaveCardArea()
    {
        try
        {
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
            File.WriteAllText(fullSavePath, jsonData);

            if (debugLog)
            {
                Debug.Log($"Saved to: {fullSavePath}");
                Debug.Log(jsonData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Save error: {e.Message}\n{e.StackTrace}");
        }
    }

    // Method to load the card area from a JSON file
    public void LoadCardArea()
    {
        try
        {
            if (!File.Exists(fullSavePath))
            {
                Debug.LogWarning($"Save file doesn't exist: {fullSavePath}");
                return;
            }

            string jsonData = File.ReadAllText(fullSavePath);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(jsonData);

            ClearCardArea();
            ObjectID.ClearObjectDictionary();
            NewBehaviourScript.Instance.clearDictionary();

            foreach (ChildData childData in loadedData.children)
            {
                if (childData == null) continue;
                GameObject newChild = CreateChildFromData(childData);
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
                Debug.Log($"Loaded from: {fullSavePath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Load error: {e.Message}\n{e.StackTrace}");
        }
    }

    private void ClearCardArea()
    {
        foreach (Transform child in cardArea)
        {
            Destroy(child.gameObject);
        }
    }

    private GameObject CreateChildFromData(ChildData childData)
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
        
        // Handle different object types
        switch (childData.objectType)
        {
            case "Button":
                var textComponent = newChild.GetComponentInChildren<TMP_Text>();
                if (textComponent != null)
                {
                    textComponent.text = childData.Text;
                }
                
                var imageComponent = newChild.GetComponent<Image>();
                if (imageComponent != null)
                {
                    Sprite sprite = Resources.Load<Sprite>(childData.backgroundImage);
                    if (sprite != null)
                    {
                        imageComponent.sprite = sprite;
                    }
                    else
                    {
                        imageComponent.sprite = Resources.Load<Sprite>("Background");
                        Debug.LogWarning($"Sprite not found: {childData.backgroundImage}, using default");
                    }
                }
                break;
                
            case "TextBlockPrefab":
                var textBlock = newChild.GetComponent<TextMeshProUGUI>();
                if (textBlock != null && !string.IsNullOrEmpty(childData.Text))
                {
                    textBlock.text = childData.Text;
                }
                break;
                
            case "InputField":
                var inputField = newChild.GetComponent<TMP_InputField>();
                if (inputField != null)
                {
                    inputField.text = childData.Text;
                    
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

        ObjectPlacementSystem.SetObjectComponentsEnabled(newChild, true);
        return newChild;
    }

    public string GetSavePath()
    {
        return fullSavePath;
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
    public string backgroundImage;
    public string Image;
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
        
        if (objectType == "TextBlockPrefab")
        {
            var textComp = child.GetComponent<TextMeshProUGUI>();
            if (textComp != null)
            {
                Text = textComp.text;
            }
        }
        else if (objectType == "Button")
        {
            var textComp = child.GetComponentInChildren<TMP_Text>();
            if (textComp != null)
            {
                Text = textComp.text;
            }

            var imageComp = child.GetComponentInChildren<Image>();
            if (imageComp != null && imageComp.sprite != null)
            {
                backgroundImage = imageComp.sprite.name;
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
                
                var imageComp = child.GetComponentInChildren<Image>();
                if (imageComp != null && imageComp.sprite != null)
                {
                    backgroundImage = imageComp.sprite.name;
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
        
        localPosition = child.localPosition;
        localRotation = child.localRotation;
        localScale = child.localScale;
        isActive = child.gameObject.activeSelf;
    }
}