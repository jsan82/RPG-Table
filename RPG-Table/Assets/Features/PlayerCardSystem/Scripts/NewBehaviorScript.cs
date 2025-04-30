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
    // Singleton instance
    private static NewBehaviourScript _instance;
    
    public static NewBehaviourScript Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NewBehaviourScript>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<NewBehaviourScript>();
                    singletonObject.name = typeof(NewBehaviourScript).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    //Dictionary to store operations for each object ID
    private Dictionary<string, string> _objectDictionary = new Dictionary<string, string>(); 

    //List to store the current operations for the edited button
    private string _currentOperations = "";
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

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    //Returns the operations for a given object ID
    public string GetOperationsForObject(string objectId)
    {
        foreach (var kvp in _objectDictionary)
        {
            Debug.Log($"ID: {kvp.Key}, Operations: {kvp.Value}");
        }
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
                    buttonDropdown.value = buttonDropdown.options.FindIndex(option => option.text == _currentlyEditedButtonId);
                    buttonDropdown.RefreshShownValue();
                    if (_currentlyEditedButtonId != buttonDropdown.options[buttonDropdown.value].text)
                    {
                        Debug.Log($"Currently edited button ID: {_currentlyEditedButtonId}");
                        _currentlyEditedButtonId = buttonDropdown.options[buttonDropdown.value].text;
                        LoadOperationsForCurrentButton(); // Load operations for the selected button
                        break;
                    }
                    LoadOperationsForCurrentButton(); 
                    _editDialog.SetActive(true);

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
        _currentOperations = "";
        _currentConfigText = "";
        _idDropdown.SetActive(false);
        _numberInputbox.SetActive(true);
        _addSelector.SetActive(false);
        _currentlyEditedButtonId = null;
    }

    //Method to handle the save button click event
    public void loadOperations(string id, string operations)
    {
        if (_objectDictionary.ContainsKey(id))
        {
            Debug.Log("ID already exists.");
        }
        else
        {
            _objectDictionary.Add(id, operations);
            Debug.Log($"Added operations for ID: {id}");
        }
    }
    
    //Method to load operations for the currently edited button
    public void LoadOperationsForCurrentButton()
    {
        if (!string.IsNullOrEmpty(_currentlyEditedButtonId) && _objectDictionary.ContainsKey(_currentlyEditedButtonId))
        {
            _currentOperations = _objectDictionary[_currentlyEditedButtonId];
            UpdateCurrentConfigText();
            Debug.Log($"Loaded operations for button {_currentlyEditedButtonId}");
        }
        else
        {
            _currentOperations = "";
            _currentConfigText = "";
            UpdateCurrentConfigText();
            Debug.Log("No operations found for current button or no button selected");
        }
    }

    //Method to update the current config text in the UI
    private void UpdateCurrentConfigText()
    {
        _currentConfigText = _currentOperations;
        currentConfig.GetComponent<TMP_InputField>().text = _currentConfigText;
    }

    //Method to update the dropdowns with available buttons and input fields
    void UpdateDropdown()
    {
        buttonDropdown.ClearOptions();
        _idDropdown.GetComponent<TMP_Dropdown>().ClearOptions();
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

        
        buttonDropdown.AddOptions(buttons.Keys.ToList());


        _idDropdown.GetComponent<TMP_Dropdown>().AddOptions(inputBox.Keys.ToList());
    }

    public void plusButton()
    {
        _currentConfigText += ("+");
    }

    public void minusButton()
    {
        _currentConfigText += ("-");
    }

    public void timesButton()
    {
        _currentConfigText += ("*");
    }

    public void divideButton()
    {
        _currentConfigText += ("/");
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
            _currentConfigText += ($"{value}");
        }
        else if (isIdValueOn)
        {
            string id = _idDropdown.GetComponent<TMP_Dropdown>().options[_idDropdown.GetComponent<TMP_Dropdown>().value].text;
            _currentConfigText += ($"@{id}");
        }
        _addSelector.SetActive(false);
    }

    //Method to handle the removal of a value from the current operations
    public void confirmButton()
    {
        if (!string.IsNullOrEmpty(_currentlyEditedButtonId))
        {
            // Update or add the operations for this button
            _objectDictionary[_currentlyEditedButtonId] = currentConfig.GetComponent<TMP_InputField>().text;
            Debug.Log($"Updated operations for button {_currentlyEditedButtonId}: {currentConfig.GetComponent<TMP_InputField>().text}");
            // Clear current editing state
            _currentOperations = "";
            _currentConfigText = "";
            _editDialog.SetActive(false);
            _currentlyEditedButtonId = null;
        }
        else
        {
            Debug.LogWarning("No button is currently being edited!");
        }
    }
    
    public void clearDictionary()
    {
        _objectDictionary.Clear();
        Debug.Log("Cleared all operations from the dictionary.");
    }
}
