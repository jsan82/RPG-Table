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
    [Header("Ustawienia")]
    public string saveFileName = "cardAreaSave.json";
    public bool debugLog = true;

    private Transform cardArea;
    private List<ObjectDictData> objectDictData;
    private string fullSavePath;

    void Awake()
    {
        cardArea = GameObject.Find("CardArea").transform;
        fullSavePath = Path.Combine(Application.persistentDataPath, saveFileName);
    }

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
                    NewBehaviourScript.printObj();
                    // Zbierz dane wszystkich skryptów
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
                Debug.Log(jsonData);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Błąd zapisu: {e.Message}");
        }
    }

    public void LoadCardArea()
{
    try
    {
        if (!File.Exists(fullSavePath))
        {
            Debug.LogWarning($"Plik zapisu nie istnieje: {fullSavePath}");
            return;
        }

        string jsonData = File.ReadAllText(fullSavePath);
        SaveData loadedData = JsonUtility.FromJson<SaveData>(jsonData);

        ClearCardArea();
        ObjectID.ClearObjectDictionary();

        foreach (ChildData childData in loadedData.children)
        {
            GameObject newChild = CreateChildFromData(childData);
        
            foreach (ScriptData scriptData in childData.scripts)
            {
                try
                {
                    Type scriptType = Type.GetType(scriptData.type);
                    if (scriptType != null)
                    {
                        
                        // Sprawdź czy komponent już istnieje
                        Component component = newChild.GetComponent(scriptType);
                        if (component == null)
                        {
                            component = newChild.AddComponent(scriptType);

                        }
                        if (component != null)
                        {
                            JsonUtility.FromJsonOverwrite(scriptData.data, component);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Nie można rozpoznać typu: {scriptData.type}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Błąd podczas przetwarzania skryptu {scriptData.type}: {e.Message}");
                }
            }
        List<OperationData> operations = childData.currentOperations;
        NewBehaviourScript.loadOperations(childData.objectID, operations.Select(op => (op.operationType, op.value)).ToList());

        }

    }
    catch (Exception e)
    {
        Debug.LogError($"Błąd wczytywania: {e.Message}\n{e.StackTrace}");
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
        GameObject newChild = Resources.Load<GameObject>(childData.objectType);
        newChild = Instantiate(newChild, cardArea);
        

        //newChild.name = childData.objectID;
        newChild.transform.localPosition = childData.localPosition;
        newChild.transform.localRotation = childData.localRotation;
        newChild.transform.localScale = childData.localScale;
        
        switch (childData.objectType)
        {
            case "Button":
                newChild.GetComponentInChildren<TMP_Text>().text = childData.Text;
                Debug.Log($"Obrazek: {childData.backgroundImage}");
                if(newChild.TryGetComponent<Image>(out Image imageComponent))
                {
                    imageComponent.sprite = Resources.Load<Sprite>(childData.backgroundImage);
                }
                else
                {
                    imageComponent.sprite = Resources.Load<Sprite>("Background");
                }
                break;
            case "TextBlockPrefab":
                TextMeshProUGUI textBlock = newChild.GetComponent<TextMeshProUGUI>();
                if (textBlock != null && !string.IsNullOrEmpty(childData.Text))
                {
                    textBlock.text = childData.Text;
                }
                break;
            case "InputField":
                newChild.GetComponent<TMP_InputField>().text = childData.Text;
                newChild.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(childData.backgroundImage);
                switch (childData.inputType)
                {
                    case "InputFieldStandard":
                        newChild.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Standard;
                        break;
                    case "InputFieldDecimalNumber":
                        newChild.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.DecimalNumber;
                        break;
                    case "InputFieldIntegerNumber":
                        newChild.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.IntegerNumber;
                        break;
                }
                break;
        }

        newChild.SetActive(childData.isActive);
        ObjectID objID = newChild.GetComponent<ObjectID>();
        if(objID == null) objID = newChild.AddComponent<ObjectID>();
        objID.SetID(childData.objectID, newChild, childData.objectType);
        newChild.name = childData.objectID; // Ustawiamy nazwę obiektu na ID

        ObjectPlacementSystem.SetObjectComponentsEnabled(newChild, true);
        return newChild; // Zwracamy obiekt dla dalszej obróbki
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
public class OperationData
{
    public string operationType;
    public string value;

    public OperationData(string type, string val)
    {
        operationType = type;
        value = val;
    }
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

    public List<OperationData> currentOperations = new List<OperationData>();

    public ChildData(Transform child)
    {
        objectID = child.name;
        objectType = child.GetComponent<ObjectID>().GetPrefab();
        
        if(objectType == "TextBlockPrefab")
        {
            Text = child.GetComponent<TextMeshProUGUI>().text;
        }
        else if(objectType == "Button")
        {
            Text = child.GetComponentInChildren<TMP_Text>().text;
            backgroundImage = child.GetComponentInChildren<Image>().sprite.name;
            var operations = NewBehaviourScript.GetOperationsForObject(objectID);
            if (operations != null)
            {
                currentOperations = operations.Select(op => new OperationData(op.Item1, op.Item2)).ToList();
                foreach (var operation in currentOperations)
                {
                    Debug.Log($"Operacja: {operation.operationType}, Wartość: {operation.value}");
                }
            };
            
        }
        else if(objectType == "InputField")
        {
            Text = child.GetComponent<TMP_InputField>().text;
            backgroundImage = child.GetComponentInChildren<Image>().sprite.name;
        switch (child.GetComponent<TMP_InputField>().contentType)
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
        
        localPosition = child.localPosition;
        localRotation = child.localRotation;
        localScale = child.localScale;
        isActive = child.gameObject.activeSelf;
    }
}