using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages game settings including loading, saving, and STL file conversion
/// </summary>
public class SettingsManager : MonoBehaviour
{
    /// <summary>Current game settings</summary>
    public static GameSettings _CurrentSettings;
    /// <summary>Path to the settings JSON file</summary>
    private static string savePath => Path.Combine(Application.persistentDataPath, "settings.json");
    /// <summary>STL to OBJ converter instance</summary>
    private StlConverter stlConverter;
    /// <summary>List of STL filenames to convert</summary>
    private List<string> fileNames = new List<string>();

    /// <summary>
    /// Called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        if (stlConverter == null)
        {
            stlConverter = new StlConverter();
        }
        LoadSettings();
        loadSTLAsOBJ();
    }

    /// <summary>
    /// Saves current settings to a JSON file
    /// </summary>
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

    /// <summary>
    /// Loads settings from JSON file or creates default settings if file doesn't exist
    /// </summary>
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
                if(!Directory.Exists(Path.Combine(_CurrentSettings.GameCardsPath, "playerCardNotes/")))
                {
                    Directory.CreateDirectory(Path.Combine(_CurrentSettings.GameCardsPath, "playerCardNotes/"));
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

    /// <summary>
    /// Converts STL files to OBJ format and moves original STL files to convertedSTL directory
    /// </summary>
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

/// <summary>
/// Serializable class containing all game settings and paths
/// </summary>
[System.Serializable]
public class GameSettings
{
    /// <summary>Path for player cards storage</summary>
    public string playerCardsPath = Path.Combine(Application.persistentDataPath, "PlayerCards/");
    /// <summary>Path for player card prefabs</summary>
    public string playerCardsPrefabPath = Path.Combine(Application.persistentDataPath, "PlayerPrefab/");
    /// <summary>Path for 2D assets</summary>
    public string Assets2DPath = Path.Combine(Application.persistentDataPath, "2DAssets/");
    /// <summary>Path for 3D assets</summary>
    public string Assets3DPath = Path.Combine(Application.persistentDataPath, "3DAssets/");
    /// <summary>Path for game cards</summary>
    public string GameCardsPath = Path.Combine(Application.persistentDataPath, "PlayerCardsGame/");
    /// <summary>Path for maps</summary>
    public string MapsPath = Path.Combine(Application.persistentDataPath, "Maps/");
}