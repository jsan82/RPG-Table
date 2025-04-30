using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class BackToMenu : MonoBehaviour {
    public void ReturnToMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}

public class EditableButtonCreator : MonoBehaviour
{
    public GameObject buttonPrefab; //Button prefab
    public GameObject popupPanel; // Popup panel for editing button names
    public TMP_InputField nameInputField; // Input field for button name
    public Transform buttonsParent; // Parent transform for the buttons

    private GameObject currentEditedButton; // Currently edited button

    //Method to create a new button
    public void ShowPopupForNewButton()
    {
        popupPanel.SetActive(true);
        nameInputField.text = "";
        currentEditedButton = null;
    }

    //Method to create a new button with the name from the input field
    public void SaveButtonName()
    {
        if (currentEditedButton == null)
        {
            // Tworzenie nowego przycisku
            GameObject newButton = Instantiate(buttonPrefab, buttonsParent);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = nameInputField.text;

            // Dodajemy funkcjonalność usuwania
            Button deleteBtn = newButton.transform.Find("DeleteButton").GetComponent<Button>();
            deleteBtn.onClick.AddListener(() => Destroy(newButton));
        }
        else
        {
            // Edycja istniejącego przycisku
            currentEditedButton.GetComponentInChildren<TextMeshProUGUI>().text = nameInputField.text;
        }

        popupPanel.SetActive(false);
    }

    //Method to cancel the popup
    public void EditExistingButton(GameObject buttonToEdit)
    {
        currentEditedButton = buttonToEdit;
        nameInputField.text = buttonToEdit.GetComponentInChildren<TextMeshProUGUI>().text;
        popupPanel.SetActive(true);
    }
}