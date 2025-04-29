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
    //Dictionary to store operations for each object ID
    private static Dictionary<string, List<(string Operation, string Value)>> _objectDictionary = new Dictionary<string, List<(string Operation, string Value)>>(); 

    //List to store the current operations for the edited button
    private List<(string Operation, string Value)> _currentOperations = new List<(string Operation, string Value)>();
    [SerializeField] private TMP_Dropdown buttonDropdown; // Dropdown for selecting buttons
    [SerializeField] private Toggle _editMode; // Toggle for edit mode
    [SerializeField] private GameObject _editDialog; // Dialog for editing operations
    [SerializeField] private Toggle _numberValue;  // Toggle for number value
    [SerializeField] private Toggle _idValue;   // Toggle for ID value
    [SerializeField] private GameObject _idDropdown; // Dropdown for selecting IDs
    [SerializeField] private GameObject _numberInputbox; // Input box for number value
    [SerializeField] private GameObject _addSelector; // Selector for adding operations
    [SerializeField] private GameObject currentConfig; // Text field to display current configuration

    private string _currentConfigText;
    private bool isNumberValueOn = true;
    private bool isIdValueOn = false;
    private string _currentlyEditedButtonId = null; // To keep track of which button is being edited

    //Returns the operations for a given object ID
    public static List<(string, string)> GetOperationsForObject(string objectId)
    {
        if (_objectDictionary.ContainsKey(objectId))
        {
            return _objectDictionary[objectId];
        }
        return null;
    }

    //Logick to handle the click on the UI elements
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
                    LoadOperationsForCurrentButton(); 
                    _editDialog.SetActive(true);
                    buttonDropdown.value = 0;
                    break;
                }
            }
        }
    }

    //Method to handle the button click event
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
            // if(_editDialog.activeSelf)
            // {
            //     //if value of the button dropdown changes, update the currently edited button ID
            //     if ( _currentlyEditedButtonId != buttonDropdown.options[buttonDropdown.value].text)
            //     {
            //         _currentlyEditedButtonId = buttonDropdown.options[buttonDropdown.value].text;
            //         LoadOperationsForCurrentButton();
            //     }
            // }

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

    //Method to handle the cancel button click event
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

    //Method to handle the save button click event
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
    
    //Method to load operations for the currently edited button
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
            _currentConfigText = "";
            UpdateCurrentConfigText();
            Debug.Log("No operations found for current button or no button selected");
        }
    }

    //Method to update the current config text in the UI
    private void UpdateCurrentConfigText()
    {
        _currentConfigText = string.Join(" | ", _currentOperations.Select(op => $"{op.Operation}:{op.Value}"));
        currentConfig.GetComponent<TextMeshProUGUI>().text = _currentConfigText;
    }

    //Method to update the dropdowns with available buttons and input fields
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

    //Method to handle the addition of a value to the current operations
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

    //Method to handle the removal of a value from the current operations
    public void confirmButton()
    {
        if (!string.IsNullOrEmpty(_currentlyEditedButtonId))
        {
            // Update or add the operations for this button
            _objectDictionary[_currentlyEditedButtonId] = new List<(string Operation, string Value)>(_currentOperations);
            
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