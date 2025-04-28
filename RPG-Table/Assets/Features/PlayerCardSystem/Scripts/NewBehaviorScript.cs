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
    [SerializeField] private  TMP_Dropdown buttonDropdown;
    [SerializeField] private Toggle _editMode;
    [SerializeField] private GameObject _editDialog;
    [SerializeField] private Toggle _numberValue;
    [SerializeField] private Toggle _idValue;
    [SerializeField] private GameObject _idDropdown;
    [SerializeField] private GameObject _numberInputbox;
    [SerializeField] private GameObject _addSelector;
    [SerializeField] private GameObject currentConfig;

    private string _currentConfigText ;
    private bool isNumberValueOn = true;
    private bool isIdValueOn = false;
    
    public static List<(string, string)> GetOperationsForObject(string objectId)
{
    if (_objectDictionary.ContainsKey(objectId))
    {
        return _objectDictionary[objectId];
    }
    return null;
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
                    _editDialog.SetActive(true);
                    buttonDropdown.value = 0; // Resetuj dropdown do pierwszej opcji
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
            foreach (var operation in _currentOperations)
            {
                _currentConfigText += ($" {operation.Operation} : {operation.Value}|");
            }
            if(_currentConfigText != currentConfig.GetComponent<TextMeshProUGUI>().text)
            {
                currentConfig.GetComponent<TextMeshProUGUI>().text = _currentConfigText;
            }
            _currentConfigText = ""; // Resetuj tekst po aktualizacji
            HandleUIClick(); // Dodaj wywołanie funkcji sprawdzającej kliknięcia
        }

        // Ensure toggles work correctly using helper variables
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


        if (isNumberValueOn)
        {
            _idValue.isOn = false;
            _idDropdown.SetActive(false);
            _numberInputbox.SetActive(true);
        }
        else if (isIdValueOn)
        {
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
        _currentConfigText = ""; // Resetuj tekst po zakończeniu edycji
        _idDropdown.SetActive(false);
        _numberInputbox.SetActive(true);
        _addSelector.SetActive(false);
    }

    void UpdateDropdown()
    {
        Dictionary<string, GameObject> buttons = ObjectID.GetAllObjects()
            .Where(x => x.Value.GetComponent<ObjectID>() != null && x.Value.GetComponent<ObjectID>().GetPrefab() == "Button")
            .ToDictionary(x => x.Key, x => x.Value);

        Dictionary<string, GameObject> inputBox = ObjectID.GetAllObjects()
            .Where(x => x.Value.GetComponent<ObjectID>() != null && x.Value.GetComponent<ObjectID>().GetPrefab() == "InputField")
            .ToDictionary(x => x.Key, x => x.Value);

        buttonDropdown.ClearOptions();
        foreach (var button in buttons)
        {
            Debug.Log($"ID: {button.Key}, Obiekt: {button.Value.name}");
        }
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
            _currentOperations.Add(("add:", "@"+id));
        }
        _addSelector.SetActive(false);
    }
    public void confirmButton()
    {
        string id = buttonDropdown.options[buttonDropdown.value].text;
        _objectDictionary[id] = _currentOperations;
        Debug.Log($"Dodano operacje do ID: {id} | Operacje: {string.Join(", ", _currentOperations.Select(op => $"{op.Operation} {op.Value}"))}");   
        
    }
    public static void ClearOperations()
    {
        _objectDictionary.Clear();
    }

}
