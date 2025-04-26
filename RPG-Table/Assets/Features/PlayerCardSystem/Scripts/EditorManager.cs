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
    public GameObject buttonPrefab; // Prefab przycisku
    public GameObject popupPanel; // Panel z input field
    public TMP_InputField nameInputField; // Pole do wpisywania nazwy
    public Transform buttonsParent; // Gdzie będą tworzone przyciski

    private GameObject currentEditedButton; // Aktualnie edytowany przycisk

    public void ShowPopupForNewButton()
    {
        popupPanel.SetActive(true);
        nameInputField.text = "";
        currentEditedButton = null;
    }

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

    public void EditExistingButton(GameObject buttonToEdit)
    {
        currentEditedButton = buttonToEdit;
        nameInputField.text = buttonToEdit.GetComponentInChildren<TextMeshProUGUI>().text;
        popupPanel.SetActive(true);
    }
}