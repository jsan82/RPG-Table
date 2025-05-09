using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;

public class SelectLoad : MonoBehaviour
{
    public GameObject objectButtonPrefab;
    public GameObject loadSelectWindow;
    public GameObject saveWindow;
    public Transform filesPanel;
    private List<string> fileNames = new List<string>();
    private string PATH_TO_FILES = SettingsManager._CurrentSettings.playerCardsPath;
    private string currentEditingFileName;

    public GameObject deleteConfirmationDialog;
    private bool isDeleteMode = false;
    private string saveToDelete;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (loadSelectWindow.activeSelf)
            {
                if (deleteConfirmationDialog.activeSelf)
                {
                    CancelDelete();
                }
                else if (isDeleteMode)
                {
                    CancelDelete();
                }
                else
                {
                    onCancelButtonClick();
                }
            }
        }
    }
    
    void LoadCard(string filePath)
    {
        if (isDeleteMode)
        {
  
            saveToDelete = filePath;
            deleteConfirmationDialog.SetActive(true);
            

            var fileNameText = deleteConfirmationDialog.transform.Find("SaveNamePlace");
            if (fileNameText != null)
            {
                var textComponent = fileNameText.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = Path.GetFileNameWithoutExtension(filePath);
                }
            }
            return;
        }

        Debug.Log("Loading card from: " + filePath);
        CardAreaSaver.LoadCardArea(filePath);
        loadSelectWindow.SetActive(false);
        ClearFileList();
    }

    public void EnterDeleteMode()
    {
        isDeleteMode = true;
        Debug.Log("Delete mode activated. Click on a save file to delete it.");
    }

    public void ConfirmDelete()
    {
        if (!string.IsNullOrEmpty(saveToDelete))
        {
            try
            {
                File.Delete(saveToDelete);
                Debug.Log("Deleted file: " + saveToDelete);
                RefreshFileList();
            }
            catch (Exception e)
            {
                Debug.LogError("Error deleting file: " + e.Message);
            }
        }
        CancelDelete();
    }

    public void CancelDelete()
    {
        isDeleteMode = false;
        saveToDelete = null;
        deleteConfirmationDialog.SetActive(false);
    }

    public void onSaveButtonClick()
    {
        saveWindow.SetActive(true);
        if(currentEditingFileName != null)
        {
            saveWindow.transform.Find("CardName").GetComponent<TMP_InputField>().text = currentEditingFileName;
        }
        Debug.Log(PATH_TO_FILES);  
    }

    public void savingButtonClick()
    {
        currentEditingFileName = saveWindow.transform.Find("CardName").GetComponent<TMP_InputField>().text;
        Debug.Log("Saving card as: " + currentEditingFileName);
        CardAreaSaver.SaveCardArea(Path.Combine(PATH_TO_FILES, currentEditingFileName + ".json"));
        saveWindow.SetActive(false);
    }

    public void onLoadButtonClick()
    {
        fileNames.Clear();
        fileNames.AddRange(Directory.GetFiles(PATH_TO_FILES, "*.json"));
        
        foreach (string file in fileNames)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            GameObject button = Instantiate(objectButtonPrefab, filesPanel);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = fileName;
            button.name = fileName;
            currentEditingFileName = fileName;
            button.GetComponent<Button>().onClick.AddListener(() => LoadCard(file));
        }
        loadSelectWindow.SetActive(true);
    }

    public void onCancelButtonClick()
    {
        if(saveWindow.activeSelf)
        {
            saveWindow.SetActive(false);
        }
        else if (loadSelectWindow.activeSelf)
        {
            ClearFileList();
            loadSelectWindow.SetActive(false);
        }
    }

    private void ClearFileList()
    {
        fileNames.Clear();  
        foreach (Transform child in filesPanel)
        {
            Destroy(child.gameObject);
        }
    }

    private void RefreshFileList()
    {
        ClearFileList();
        onLoadButtonClick();
    }
}