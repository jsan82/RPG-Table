using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using System.IO;


/// <summary>
/// Singleton manager for handling button operations and expressions in the UI
/// </summary>
/// <remarks>
/// Implements IUIBehavior to manage interactive editing of button expressions.
/// Maintains a dictionary of button operations and provides UI for editing them.
/// Supports mathematical operations and references to other UI elements.
/// </remarks>
public class NewBehaviourScript : MonoBehaviour, IUIBehavior
{
    // Singleton instance
    private static NewBehaviourScript _instance;

    /// <summary>Singleton instance accessor</summary>
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

    /// <summary>Dictionary storing operations for each button ID</summary>
    private Dictionary<string, string> _objectDictionary = new Dictionary<string, string>();

    /// <summary>Currently edited operations string</summary>
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
    /// <summary>Currently edited button ID</summary>
    private string _currentlyEditedButtonId = null; // To keep track of which button is being edited

    private void Start()
    {
        Debug.Log("NewBehaviourScript Awake called");
    }
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

    /// <summary>
    /// Retrieves operations string for a specific object ID
    /// </summary>
    /// <param name="objectId">The ID of the object to look up</param>
    /// <returns>Operations string if found, null otherwise</returns>
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

    /// <summary>
    /// Handles UI click events to select buttons for editing
    /// </summary>
    /// <remarks>
    /// Activates when in edit mode and detects clicks on button objects.
    /// Updates the UI to show the selected button's current operations.
    /// </remarks>
    public void HandleUIClick()
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
        if (_editMode)
        {
            if (_editMode.isOn)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (_addSelector.activeSelf)
                    {
                        addValueCancel();
                    }
                    else
                    {
                        cancelEditMode();
                    }
                }

                HandleUIClick();
            }
            else
            {
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

    /// <summary>
    /// Loads operations for the currently selected button
    /// </summary>
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

    /// <summary>
    /// Updates dropdown menus with available buttons and input fields
    /// </summary>
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


    /// <summary>
    /// Adds a mathematical operator to the current operations
    /// </summary>
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

    
    /// <summary>
    /// Shows the value addition selector UI
    /// </summary>
    public void addButton()
    {
        _addSelector.SetActive(true);
    }

    /// <summary>
    /// Adds a numeric or ID reference value to the operations
    /// </summary>
    public void addValueButton()
    {
        if (isNumberValueOn)
        {
            string value = _numberInputbox.GetComponent<TMP_InputField>().text;
            if (value == "")
            {
                currentConfig.GetComponent<TMP_InputField>().text += ($"0");
            }
            else
            {
                currentConfig.GetComponent<TMP_InputField>().text += ($"{value}");
            }
        }
        else if (isIdValueOn)
        {
            string id = _idDropdown.GetComponent<TMP_Dropdown>().options[_idDropdown.GetComponent<TMP_Dropdown>().value].text;
            currentConfig.GetComponent<TMP_InputField>().text += ($"@{id}");
        }
        _addSelector.SetActive(false);
    }
    public void addValueCancel()
    {
        _addSelector.SetActive(false);
    }

    /// <summary>
    /// Saves the current operations to the selected button
    /// </summary>
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


    /// <summary>
    /// Clears all stored operations from the dictionary
    /// </summary>
    public void clearDictionary()
    {
        _objectDictionary.Clear();
        Debug.Log("Cleared all operations from the dictionary.");
    }

    /// <summary>
    /// Removes operations for a specific ID from the dictionary
    /// </summary>
    /// <param name="id">The ID to remove</param>
    public void deleteKey(string id)
    {
        if (_objectDictionary.ContainsKey(id))
        {
            _objectDictionary.Remove(id);
        }
    }

}


/// <summary>
/// Interface for UI click handling behavior
/// </summary>
public interface IUIBehavior
{
    /// <summary>
    /// Method to handle UI click interactions
    /// </summary>
    void HandleUIClick();
}
