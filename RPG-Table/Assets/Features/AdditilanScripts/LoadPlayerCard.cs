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

public class LoadPlayerCard : MonoBehaviour
{
    public GameObject PlayerCardWindow;
    public RectTransform playerCardArea;
    public GameObject playerCardNotes;

    public GameObject manager;

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

[Serializable]
public class PlayercardNotes {
    public string Notes;
}