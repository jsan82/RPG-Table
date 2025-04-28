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
                Debug.Log($"Zapisano {childrenData.Count} obiektów w CardArea do: {fullSavePath}");
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
            
            // Dodaj debug log przed wczytywaniem skryptów
            Debug.Log($"Przetwarzanie obiektu: {newChild.name}");
            
            foreach (ScriptData scriptData in childData.scripts)
            {
                try
                {
                    Type scriptType = Type.GetType(scriptData.type);
                    if (scriptType != null)
                    {
                        Debug.Log($"Próbuję dodać skrypt typu: {scriptType}");
                        
                        // Sprawdź czy komponent już istnieje
                        Component component = newChild.GetComponent(scriptType);
                        if (component == null)
                        {
                            component = newChild.AddComponent(scriptType);
                            Debug.Log($"Dodano nowy komponent: {scriptType}");
                        }
                        
                        if (component != null)
                        {
                            Debug.Log($"Dane skryptu do nadpisania: {scriptData.data}");
                            JsonUtility.FromJsonOverwrite(scriptData.data, component);
                            Debug.Log($"Pomyślnie nadpisano dane skryptu");
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

        }

        if (debugLog)
        {
            Debug.Log($"Wczytano {loadedData.childCount} obiektów do CardArea");
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
                newChild.GetComponentInChildren<Text>().text = childData.Text;
                Debug.Log($"Obrazek: {childData.backgroundImage}");
                if(newChild.TryGetComponent<Image>(out Image imageComponent))
                {
                    imageComponent.sprite = Resources.Load<Sprite>(childData.backgroundImage);
                }
                else
                {
                    Debug.LogWarning($"Nie znaleziono komponentu Image w obiekcie {newChild.name}");
                }
                // if (Resources.Load<Sprite>(childData.backgroundImage) != null)
                // {
                //     //newChild.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(childData.backgroundImage);
                //     Debug.Log($"Załadowano obrazek");
                // }
                // else
                // {
                //     //newChild.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Background");
                //     Debug.Log($"NIe załadowano");
                // }
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
            Text = child.GetComponentInChildren<Text>().text;
            backgroundImage = child.GetComponentInChildren<Image>().sprite.name;
            var operations = NewBehaviourScript.GetOperationsForObject(objectID);
            if (operations != null)
            {
                currentOperations = operations.Select(op => new OperationData(op.Item1, op.Item2)).ToList();
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