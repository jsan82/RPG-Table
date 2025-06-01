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
/// Handles loading and saving player card data and associated notes
/// </summary>
/// <remarks>
/// Manages:
/// - Player card UI state
/// - Card content serialization
/// - Notes storage/retrieval
/// - Content size calculation
/// </remarks>
public class LoadPlayerCard : MonoBehaviour
{
    [Header("UI References")]
    public GameObject PlayerCardWindow;    // Main player card window
    public RectTransform playerCardArea;   // Container for card content
    public GameObject playerCardNotes;     // Notes input field
    public GameObject manager;             // Game manager reference

    /// <summary>
    /// Saves player card data and associated notes
    /// </summary>
    /// <param name="cardPath">Destination file path</param>
    /// <param name="pca">Player card area reference</param>
    /// <param name="pcw">Player card window reference</param>
    /// <param name="pcn">Player notes reference</param>
    /// <remarks>
    /// Saves two files:
    /// 1. Card content via CardAreaSaver
    /// 2. Notes as separate JSON file
    /// </remarks>
    public void savePlayerCard(string cardPath, GameObject pca, GameObject pcw, GameObject pcn)
    {
        PlayerCardWindow = pcw;
        playerCardArea = pca.GetComponent<RectTransform>();
        playerCardNotes = pcn;
        CardAreaSaver.SaveCardArea(Path.Combine(SettingsManager._CurrentSettings.GameCardsPath, cardPath));

        PlayercardNotes notes = new PlayercardNotes();
        notes.Notes = playerCardNotes.GetComponent<TMP_InputField>().text;
        string jsonData = JsonUtility.ToJson(notes, true);
        string notesPath = Path.Combine(SettingsManager._CurrentSettings.GameCardsPath,
            "playerCardNotes",
            Path.GetFileNameWithoutExtension(cardPath) + "_notes.json");
        jsonData = jsonData.Replace("\\\\", "/");
        File.WriteAllText(notesPath, jsonData);

    }

    /// <summary>
    /// Loads player card data and associated notes
    /// </summary>
    /// <param name="cardPath">Source file path</param>
    /// <param name="pca">Player card area reference</param>
    /// <param name="pcw">Player card window reference</param>
    /// <param name="pcn">Player notes reference</param>
    /// <remarks>
    /// Handles cases where notes file doesn't exist
    /// </remarks>
    public void loadPlayerCard(string cardPath, GameObject pca, GameObject pcw, GameObject pcn)
    {
        PlayerCardWindow = pcw;
        PlayerCardWindow.SetActive(true);
        playerCardArea = pca.GetComponent<RectTransform>();
        playerCardNotes = pcn;
        CardAreaSaver.LoadCardArea(Path.Combine(SettingsManager._CurrentSettings.GameCardsPath, cardPath), false);
        pcn.GetComponent<TMP_InputField>().text = "";
        string notesPath = Path.Combine(SettingsManager._CurrentSettings.GameCardsPath,
            "playerCardNotes",
            Path.GetFileNameWithoutExtension(cardPath) + "_notes.json");
        notesPath = notesPath.Replace("\\\\", "/");
        if (File.Exists(notesPath))
        {
            string jsonData = File.ReadAllText(notesPath);
            PlayercardNotes notes = JsonUtility.FromJson<PlayercardNotes>(jsonData);
            pcn.GetComponent<TMP_InputField>().text = notes.Notes;
        }
        else
        {
            pcn.GetComponent<TMP_InputField>().text = "";
        }


    }

    /// <summary>
    /// Updates content container size based on children
    /// </summary>
    /// <remarks>
    /// Calculates total height and maximum width of all child elements
    /// </remarks>
    public void UpdateContentSize()
    {
        if (playerCardArea == null) return;

        float totalHeight = 0f;
        float maxWidth = 0f;

        // Oblicz całkowitą wysokość i szerokość dzieci
        foreach (RectTransform child in playerCardArea)
        {
            totalHeight += child.sizeDelta.y;
            maxWidth = Mathf.Max(maxWidth, child.sizeDelta.x);
        }

        // Ustaw nowy rozmiar
        playerCardArea.sizeDelta = new Vector2(maxWidth, totalHeight);
    }
}

/// <summary>
/// Serializable container for player card notes
/// </summary>
[Serializable]
public class PlayercardNotes {
    /// <summary>The text content of player notes</summary>
    public string Notes;
}