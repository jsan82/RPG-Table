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


/// <summary>
/// Manager for player card creation and selection in the game UI
/// </summary>
/// <remarks>
/// Handles:
/// - Player card template selection
/// - Player name assignment
/// - Card instantiation
/// - Auto-saving functionality
/// - Notes/card view switching
/// </remarks>
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

    /// <summary>
    /// Initializes available player card templates
    /// </summary>
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


    /// <summary>
    /// Handles auto-saving when player card window is active
    /// </summary>
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

    /// <summary>
    /// Switches to notes view
    /// </summary>
    public void PlayerCardNotes()
    {
        PlayerCardNotesPanel.SetActive(true);
        playerCardCardPanel.SetActive(false);
    }


    /// <summary>
    /// Switches to card view
    /// </summary>
    public void PlayerCardCard()
    {
        PlayerCardNotesPanel.SetActive(false);
        playerCardCardPanel.SetActive(true);
    }


    /// <summary>
    /// Shows the player card selection interface
    /// </summary>
    public void ShowPlayerCardSelector()
    {
        playerCardSelector.SetActive(true);
        playerCardName.GetComponent<TMP_InputField>().text = "";
        playerCardDropdown.GetComponent<TMP_Dropdown>().value = 0;
    }

    /// <summary>
    /// Cancels card creation and resets UI
    /// </summary>
    public void Cancel()
    {
        playerCardName.GetComponent<TMP_InputField>().text = "";
        playerCardDropdown.GetComponent<TMP_Dropdown>().value = 0;
        playerCardSelector.SetActive(false);
    }

    /// <summary>
    /// Creates a new player card instance
    /// </summary>
    /// <remarks>
    /// Validates inputs and:
    /// - Creates player button
    /// - Copies template file
    /// - Sets up click handler
    /// </remarks>
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

        newPlayerButton.GetComponent<Button>().onClick.AddListener(() => lpc.loadPlayerCard(playerName + ".json", playerCardArea, PlayerCardWindow, playerCardName));
        playerCardName.GetComponent<TMP_InputField>().text = "";
        playerCardDropdown.GetComponent<TMP_Dropdown>().value = 0;
        playerCardSelector.SetActive(false);
    }

    /// <summary>
    /// Closes the player card window
    /// </summary>
    public void exitPlayerCardWindow()
    {
        PlayerCardWindow.SetActive(false);


    }


    /// <summary>
    /// Debug method for button click handling
    /// </summary>
    /// <param name="clickedButton">Name of clicked button</param>
    public void butClick(string clickedButton)
    {
        Debug.Log(clickedButton);
    }
}
