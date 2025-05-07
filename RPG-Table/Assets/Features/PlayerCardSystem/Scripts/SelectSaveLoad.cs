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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && loadSelectWindow.activeSelf)
        {
            onCancelButtonClick();
        }
    }
    
    void LoadCard(string filePath)
    {

        Debug.Log("Loading card from: " + filePath);
        
        CardAreaSaver.LoadCardArea(filePath);
        loadSelectWindow.SetActive(false);
        fileNames.Clear();  
        foreach (Transform child in filesPanel)
        {
            Destroy(child.gameObject);
        }

    }
    public void onSaveButtonClick()
    {
        //PATH_TO_FILES = Path.Combine(Application.persistentDataPath, "PlayerCards/");
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
        CardAreaSaver.SaveCardArea(Path.Combine(PATH_TO_FILES,currentEditingFileName +".json"));
        saveWindow.SetActive(false);
    }
    public void onLoadButtonClick()
    {
        //PATH_TO_FILES = Path.Combine(Application.persistentDataPath, "PlayerCards/");
        fileNames.Clear();
        fileNames.AddRange(Directory.GetFiles(PATH_TO_FILES, "*.json"));
        foreach (string file in fileNames)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            GameObject button = Instantiate(objectButtonPrefab, filesPanel);
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = fileName;
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
            loadSelectWindow.SetActive(false);
            fileNames.Clear();  
            foreach (Transform child in filesPanel)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
