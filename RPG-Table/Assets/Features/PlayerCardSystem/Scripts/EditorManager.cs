using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles navigation back to the main menu scene
/// </summary>

public class BackToMenu : MonoBehaviour
{
    /// <summary>
    /// Loads the "MainMenu" scene when called
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

/// <summary>
/// Creates and manages editable UI buttons with name customization
/// </summary>
/// <remarks>
/// Provides functionality to create, edit, and delete buttons through a popup interface.
/// Buttons can be dynamically added to a specified parent transform.
/// </remarks>
public class EditableButtonCreator : MonoBehaviour
{
    public GameObject buttonPrefab; //Button prefab
    public GameObject popupPanel; // Popup panel for editing button names
    public TMP_InputField nameInputField; // Input field for button name
    public Transform buttonsParent; // Parent transform for the buttons

    private GameObject currentEditedButton; // Currently edited button

    /// <summary>Display popup to create a new button</summary>
    public void ShowPopupForNewButton()
    {
        popupPanel.SetActive(true);
        nameInputField.text = "";
        currentEditedButton = null;
    }

    /// <summary>
    /// Saves the current button name from input field
    /// </summary>
    /// <remarks>
    /// Creates new button if none is being edited,
    /// otherwise updates the currently edited button's text
    /// </remarks>
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

    /// <summary>
    /// Opens popup to edit an existing button's name
    /// </summary>
    /// <param name="buttonToEdit">The button GameObject to modify</param>
    public void EditExistingButton(GameObject buttonToEdit)
    {
        currentEditedButton = buttonToEdit;
        nameInputField.text = buttonToEdit.GetComponentInChildren<TextMeshProUGUI>().text;
        popupPanel.SetActive(true);
    }
}