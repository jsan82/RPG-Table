using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class AddPlayerCard : MonoBehaviour
{

    public GameObject PlayerCardWindow;
    public GameObject playerCardSelector;
    public GameObject playerCardDropdown;
    public GameObject playerCardName;
    public GameObject playerButtonPrefab;
    public GameObject playerCardPanel;
    public GameObject playerCardArea;
    public GameObject playerCardCardPanel;
    public GameObject PlayerCardNotesPanel;
    private List<string> playerCardNames;
    float timer = 0f;
    float interval = 10f;
    private string saveFile;
    private LoadPlayerCard lpc;

    // Start is called before the first frame update
    void Start()
    {
        playerCardNames = new List<string>();
        playerCardDropdown.GetComponent<TMP_Dropdown>().ClearOptions();
        playerCardNames.AddRange(Directory.GetFiles(SettingsManager._CurrentSettings.playerCardsPath, "*.json"));
        foreach (string filePath in playerCardNames)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            playerCardDropdown.GetComponent<TMP_Dropdown>().options.Add(new TMP_Dropdown.OptionData(fileName));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (CardAreaSaver._fullSavePath != null && CardAreaSaver._fullSavePath != "")
        {
            saveFile = CardAreaSaver._fullSavePath;
        }
        if (lpc == null)
        {
            lpc = new LoadPlayerCard();
        }
        if (PlayerCardWindow.activeSelf)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                timer = 0f;
                if (CardAreaSaver._fullSavePath != null && CardAreaSaver._fullSavePath != "")
                {

                    lpc.savePlayerCard(saveFile, playerCardArea, PlayerCardWindow, PlayerCardNotesPanel);
                }
                else
                {
                    Debug.LogError("Card area save path is not set.");
                }

                Debug.Log("Wykonuję się co sekundę!");
            }
        }

    }

    public void PlayerCardNotes()
    {
        PlayerCardNotesPanel.SetActive(true);
        playerCardCardPanel.SetActive(false);
    }

    public void PlayerCardCard()
    {
        PlayerCardNotesPanel.SetActive(false);
        playerCardCardPanel.SetActive(true);
    }

    public void ShowPlayerCardSelector()
{
    playerCardSelector.SetActive(true);
    playerCardName.GetComponent<TMP_InputField>().text = "";
    playerCardDropdown.GetComponent<TMP_Dropdown>().value = 0;
}
    public void Cancel()
    {
        playerCardName.GetComponent<TMP_InputField>().text = "";
        playerCardDropdown.GetComponent<TMP_Dropdown>().value = 0;
        playerCardSelector.SetActive(false);
    }

    public void addPlayerCard()
    {
        string selectedCard = playerCardDropdown.GetComponent<TMP_Dropdown>().options[playerCardDropdown.GetComponent<TMP_Dropdown>().value].text;
        string playerName = playerCardName.GetComponent<TMP_InputField>().text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Player name cannot be empty.");
            return;
        }

        if (string.IsNullOrEmpty(selectedCard))
        {
            Debug.LogError("No player card selected.");
            return;
        }
        if (playerCardPanel.transform.Find(playerName) != null)
        {
            Destroy(playerCardPanel.transform.Find(playerName));
        }
        GameObject newPlayerButton = Instantiate(playerButtonPrefab, playerCardPanel.transform);
        newPlayerButton.GetComponentInChildren<TextMeshProUGUI>().text = playerName;
        newPlayerButton.name = playerName; // Set the name of the button to the player's name
       
        //newPlayerButton.GetComponent<PlayerButton>().playerCardPath = Path.Combine(SettingsManager._CurrentSettings.playerCardsPath, selectedCard + ".json");
        File.Copy(Path.Combine(SettingsManager._CurrentSettings.playerCardsPath, selectedCard + ".json"), Path.Combine(SettingsManager._CurrentSettings.GameCardsPath, playerName + ".json"), true);
        lpc = new LoadPlayerCard();

        newPlayerButton.GetComponent<Button>().onClick.AddListener(() => lpc.loadPlayerCard(playerName +".json", playerCardArea, PlayerCardWindow,playerCardName));
        playerCardName.GetComponent<TMP_InputField>().text = "";
        playerCardDropdown.GetComponent<TMP_Dropdown>().value = 0;
        playerCardSelector.SetActive(false);
    }
    public void exitPlayerCardWindow()
    {
        PlayerCardWindow.SetActive(false);
        
        
    }
    public void butClick(string clickedButton)
    {
        Debug.Log(clickedButton);
    }
}
