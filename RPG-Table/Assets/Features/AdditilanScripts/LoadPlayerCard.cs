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

    public GameObject manager;




    void Start()
    {
    }



    // Update is called once per frame
    void Update()
    {

    }

    public void savePlayerCard(string cardPath, GameObject pca, GameObject pcw)
    {
        PlayerCardWindow = pcw;
        playerCardArea = pca.GetComponent<RectTransform>();
        CardAreaSaver.SaveCardArea(Path.Combine(SettingsManager._CurrentSettings.GameCardsPath, cardPath));
    }
    
    public void loadPlayerCard(string cardPath, GameObject pca, GameObject pcw)
{
    PlayerCardWindow = pcw;
    PlayerCardWindow.SetActive(true);
    playerCardArea = pca.GetComponent<RectTransform>();
    CardAreaSaver.LoadCardArea(Path.Combine(SettingsManager._CurrentSettings.GameCardsPath, cardPath));
    

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
