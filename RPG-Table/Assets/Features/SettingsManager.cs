using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;
public class SettingsManager : MonoBehaviour
{
    public static GameSettings _CurrentSettings;
    private static string savePath => Path.Combine(Application.persistentDataPath, "settings.json");
    private StlConverter stlConverter;
    private List<string> fileNames = new List<string>();

    private void Awake()
    {
        if (stlConverter == null)
        {
            stlConverter = new StlConverter();
        }
        LoadSettings();
        loadSTLAsOBJ();

    }

    public static void SaveSettings()
    {
        try
        {
            if (_CurrentSettings == null)
                _CurrentSettings = new GameSettings();

            string jsonData = JsonUtility.ToJson(_CurrentSettings, true);
            jsonData = jsonData.Replace("\\\\", "/");
            File.WriteAllText(savePath, jsonData);
            Debug.Log($"Settings saved to: {savePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save settings: {e.Message}");
        }
    }

    public static void LoadSettings()
    {
        try
        {
            if (File.Exists(savePath))
            {
                string jsonData = File.ReadAllText(savePath);
                _CurrentSettings = new GameSettings(); // Initialize to avoid null reference
                _CurrentSettings = JsonUtility.FromJson<GameSettings>(jsonData);
                if (!Directory.Exists(_CurrentSettings.playerCardsPath))
                {
                    Directory.CreateDirectory(_CurrentSettings.playerCardsPath);
                }
                if (!Directory.Exists(_CurrentSettings.playerCardsPrefabPath))
                {
                    Directory.CreateDirectory(_CurrentSettings.playerCardsPrefabPath);
                }
                if (!Directory.Exists(_CurrentSettings.Assets2DPath))
                {
                    Directory.CreateDirectory(_CurrentSettings.Assets2DPath);
                }
                if (!Directory.Exists(_CurrentSettings.Assets3DPath))
                {
                    Directory.CreateDirectory(_CurrentSettings.Assets3DPath);
                }
                if (!Directory.Exists(_CurrentSettings.GameCardsPath))
                {
                    Directory.CreateDirectory(_CurrentSettings.GameCardsPath);
                }
                if (!Directory.Exists(_CurrentSettings.MapsPath))
                {
                    Directory.CreateDirectory(_CurrentSettings.MapsPath);
                }
                if (!Directory.Exists(Path.Combine(_CurrentSettings.MapsPath, "terrainData/")))
                {
                    Directory.CreateDirectory(Path.Combine(_CurrentSettings.MapsPath, "terrainData/"));
                }
                if (!Directory.Exists(Path.Combine(_CurrentSettings.MapsPath, "drawingData/")))
                {
                    Directory.CreateDirectory(Path.Combine(_CurrentSettings.MapsPath, "drawingData/"));
                }
            }
            else
            {
                Debug.Log("No settings file found, using defaults");
                SaveSettings(); // This will create the file with defaults
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load settings: {e.Message}");
            _CurrentSettings = new GameSettings();
        }
    }

    private void loadSTLAsOBJ()
    {
        if (!Directory.Exists(Path.Combine(_CurrentSettings.Assets3DPath, "convertedSTL/")))
        {
            Directory.CreateDirectory(Path.Combine(_CurrentSettings.Assets3DPath, "convertedSTL/"));
        }
        fileNames.AddRange(Directory.GetFiles(_CurrentSettings.Assets3DPath, "*.stl"));
        Debug.Log(_CurrentSettings.Assets3DPath);
        foreach (string filePath in fileNames)
        {
            Debug.Log($"Converting STL file: {filePath}");
            stlConverter.Convert(filePath, Path.Combine(_CurrentSettings.Assets3DPath, Path.GetFileNameWithoutExtension(filePath) + ".obj"));
            File.Move(filePath, Path.Combine(_CurrentSettings.Assets3DPath, "convertedSTL/", Path.GetFileName(filePath)));
        }

    }

    
}

[System.Serializable]
public class GameSettings
{
    public string playerCardsPath = Path.Combine(Application.persistentDataPath, "PlayerCards/");
    public string playerCardsPrefabPath = Path.Combine(Application.persistentDataPath, "PlayerPrefab/");
    public string Assets2DPath = Path.Combine(Application.persistentDataPath, "2DAssets/");
    public string Assets3DPath = Path.Combine(Application.persistentDataPath, "3DAssets/");
    public string GameCardsPath = Path.Combine(Application.persistentDataPath, "PlayerCardsGame/");
    public string MapsPath = Path.Combine(Application.persistentDataPath, "Maps/");

}