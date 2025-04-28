using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using System.IO;

public class NewBehaviourScript : MonoBehaviour
{
    private static Dictionary<string, List<(string Operation, string Value)>> _objectDictionary = new Dictionary<string, List<(string Operation, string Value)>>();
    private List<(string Operation, string Value)> _currentOperations = new List<(string Operation, string Value)>();
    [SerializeField] private TMP_Dropdown buttonDropdown;
    [SerializeField] private Toggle _editMode;
    [SerializeField] private GameObject _editDialog;
    [SerializeField] private Toggle _numberValue;
    [SerializeField] private Toggle _idValue;
    [SerializeField] private GameObject _idDropdown;
    [SerializeField] private GameObject _numberInputbox;
    [SerializeField] private GameObject _addSelector;
    [SerializeField] private GameObject currentConfig;

    private string _currentConfigText;
    private bool isNumberValueOn = true;
    private bool isIdValueOn = false;
    private string _currentlyEditedButtonId = null; // To keep track of which button is being edited

    public static List<(string, string)> GetOperationsForObject(string objectId)
    {
        if (_objectDictionary.ContainsKey(objectId))
        {
            printObj();
            Debug.Log($"Key in dict: {_objectDictionary.ContainsKey(objectId)}");
            return _objectDictionary[objectId];
        }
        return null;
    }

    public static void printObj()
    {
        Debug.Log("Wszystkie operacje: ");
        foreach (var id2 in _objectDictionary.Keys)
        {
            Debug.Log($"aa");
            foreach (var operation in _objectDictionary[id2])
            {
                Debug.Log("dd");
                Debug.Log($"ID: {id2}, Operacja: {operation.Operation}, Wartość: {operation.Value}");
            }
        }
    }

    void HandleUIClick()
    {
        if (Input.GetMouseButtonDown(0) && _editMode.isOn)
        {
            UpdateDropdown();

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
        {
            ObjectID objectID = result.gameObject.GetComponent<ObjectID>();
            if (objectID != null && objectID.GetPrefab() == "Button")
            {
                _currentlyEditedButtonId = objectID.GetID();
                LoadOperationsForCurrentButton(); // Dodane wywołanie nowej funkcji
                _editDialog.SetActive(true);
                buttonDropdown.value = 0;
                break;
            }
        }
        }
    }

    void Update()
    {
        if (_editMode.isOn)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cancelEditMode();
            }

            _currentConfigText = "";
            foreach (var operation in _currentOperations)
            {
                _currentConfigText += ($" {operation.Operation} : {operation.Value}|");
            }
            
            if (_currentConfigText != currentConfig.GetComponent<TextMeshProUGUI>().text)
            {
                currentConfig.GetComponent<TextMeshProUGUI>().text = _currentConfigText;
            }

            HandleUIClick();
        }

        // Toggle handling
        if (_numberValue.isOn && !isNumberValueOn)
        {
            isNumberValueOn = true;
            isIdValueOn = false;
            _idValue.isOn = false;
            _idDropdown.SetActive(false);
            _numberInputbox.SetActive(true);
        }
        else if (_idValue.isOn && !isIdValueOn)
        {
            isIdValueOn = true;
            isNumberValueOn = false;
            _numberValue.isOn = false;
            _idDropdown.SetActive(true);
            _numberInputbox.SetActive(false);
        }
    }

    public void cancelEditMode()
    {
        _editMode.isOn = false;
        _editDialog.SetActive(false);
        _currentOperations.Clear();
        _currentConfigText = "";
        _idDropdown.SetActive(false);
        _numberInputbox.SetActive(true);
        _addSelector.SetActive(false);
        _currentlyEditedButtonId = null;
    }

    public static void loadOperations(string id, List<(string Operation, string Value)> operations)
    {
        if (_objectDictionary.ContainsKey(id))
        {
            Debug.Log("ID already exists.");
        }
        else
        {
            _objectDictionary.Add(id, operations);
        }
    }

    public void LoadOperationsForCurrentButton()
    {
        if (!string.IsNullOrEmpty(_currentlyEditedButtonId) && _objectDictionary.ContainsKey(_currentlyEditedButtonId))
        {
            _currentOperations = new List<(string Operation, string Value)>(_objectDictionary[_currentlyEditedButtonId]);
            UpdateCurrentConfigText();
            Debug.Log($"Loaded {_currentOperations.Count} operations for button {_currentlyEditedButtonId}");
        }
        else
        {
            _currentOperations.Clear();
            UpdateCurrentConfigText();
            Debug.Log("No operations found for current button or no button selected");
        }
    }

    private void UpdateCurrentConfigText()
    {
        _currentConfigText = string.Join(" | ", _currentOperations.Select(op => $"{op.Operation}:{op.Value}"));
        currentConfig.GetComponent<TextMeshProUGUI>().text = _currentConfigText;
    }

    void UpdateDropdown()
    {
        Debug.Log($"UpdateDropdown called");
        Dictionary<string, GameObject> buttons;
        Dictionary<string, GameObject> inputBox;
        Dictionary<string, GameObject> objectsDict = ObjectID.GetAllObjects();
        foreach (var obj in objectsDict)
        {
            Debug.Log($"ID: {obj.Key}, Obiekt: {obj.Value.name}, Prefab: {obj.Value.GetComponent<ObjectID>().GetPrefab()}");
        }

        buttons = ObjectID.GetAllObjects()
            .Where(x => x.Value.GetComponent<ObjectID>() != null && x.Value.GetComponent<ObjectID>().GetPrefab() == "Button")
            .ToDictionary(x => x.Key, x => x.Value);

        inputBox = ObjectID.GetAllObjects()
            .Where(x => x.Value.GetComponent<ObjectID>() != null && x.Value.GetComponent<ObjectID>().GetPrefab() == "InputField")
            .ToDictionary(x => x.Key, x => x.Value);

        buttonDropdown.ClearOptions();
        buttonDropdown.AddOptions(buttons.Keys.ToList());

        _idDropdown.GetComponent<TMP_Dropdown>().ClearOptions();
        _idDropdown.GetComponent<TMP_Dropdown>().AddOptions(inputBox.Keys.ToList());
    }

    public void plusButton()
    {
        _currentOperations.Add(("ope:", "+"));
    }

    public void minusButton()
    {
        _currentOperations.Add(("ope:", "-"));
    }

    public void timesButton()
    {
        _currentOperations.Add(("ope:", "*"));
    }

    public void divideButton()
    {
        _currentOperations.Add(("ope:", "/"));
    }

    public void addButton()
    {
        _addSelector.SetActive(true);
    }

    public void addValueButton()
    {
        if (isNumberValueOn)
        {
            string value = _numberInputbox.GetComponent<TMP_InputField>().text;
            _currentOperations.Add(("add:", value));
        }
        else if (isIdValueOn)
        {
            string id = _idDropdown.GetComponent<TMP_Dropdown>().options[_idDropdown.GetComponent<TMP_Dropdown>().value].text;
            _currentOperations.Add(("add:", "@" + id));
        }
        _addSelector.SetActive(false);
    }

    public void confirmButton()
    {
        if (!string.IsNullOrEmpty(_currentlyEditedButtonId))
        {
            // Update or add the operations for this button
            _objectDictionary[_currentlyEditedButtonId] = new List<(string Operation, string Value)>(_currentOperations);
            
            Debug.Log($"Operations saved for button {_currentlyEditedButtonId}");
            printObj();
            
            // Clear current editing state
            _currentOperations.Clear();
            _currentConfigText = "";
            _editDialog.SetActive(false);
            _currentlyEditedButtonId = null;
        }
        else
        {
            Debug.LogWarning("No button is currently being edited!");
        }
    }
    public static void clearDictionary()
    {
        _objectDictionary.Clear();
    }
}