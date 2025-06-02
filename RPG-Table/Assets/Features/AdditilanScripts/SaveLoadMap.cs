using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// Manages map saving and loading operations for both 2D and 3D modes
/// </summary>
/// <remarks>
/// Handles:
/// - Map creation and serialization
/// - Layer-based object storage
/// - Automatic periodic saving
/// - Terrain and skybox management
/// - Drawing data persistence
/// </remarks>
public class SaveLoadMap : MonoBehaviour
{
        [Header("UI References")]
    public GameObject mapTypeWhileCreatingMap;  // Dropdown for selecting map type
    public GameObject mapNameWhileCreatingMap;  // Input field for map name
    public GameObject CreateMapWindow;         // Map creation UI window
    public GameObject camera;                  // Main camera reference

    [Header("Scene References")]
    public GameObject GameManager;             // Central game manager
    public Transform terrain;                  // Terrain reference

    [Header("Configuration")]
    public string mapName;                     // Current map name
    private float timer = 0f;                  // Auto-save timer
    private const float interval = 10f;        // Auto-save interval (seconds)


    /// <summary>
    /// Initializes map system by clearing existing terrain and skybox
    /// </summary>
    void Start()
    {
        terrain.GetComponent<PlaneHandler>().ClearTerrain();
        GameManager.GetComponent<SkyboxHandler>().ClearSkybox();
    }

    /// <summary>
    /// Creates a new map with specified parameters
    /// </summary>
    /// <remarks>
    /// - Creates map info file
    /// - Clears existing map
    /// - Loads new empty map
    /// - Refreshes map selection UI
    /// </remarks>
    public void createMap()
    {
        ClearMap(); // Clear the map before loading the new one
        MapInfo mapInfo = new MapInfo();
        mapInfo.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        mapInfo.mapType = mapTypeWhileCreatingMap.GetComponent<TMP_Dropdown>().options[mapTypeWhileCreatingMap.GetComponent<TMP_Dropdown>().value].text;


        mapName = mapNameWhileCreatingMap.GetComponent<TMP_InputField>().text + ".json";

        string json = JsonUtility.ToJson(mapInfo, true);
        string filePath = Path.Combine(SettingsManager._CurrentSettings.MapsPath, $"{mapName}");
        File.WriteAllText(filePath, json);
        CreateMapWindow.SetActive(false);
        loadMap(mapName);
        CancelMapCreation();
        GameManager.GetComponent<AssetLoaderAndPlacer>().restartMaps();
    }

    /// <summary>
    /// Completely clears the current map state
    /// </summary>
    /// <remarks>
    /// Handles both 2D and 3D cases:
    /// - 2D: Clears all layer objects and drawings
    /// - 3D: Clears objects and resets terrain/skybox
    /// </remarks>
    public void ClearMap()
    {
        if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "2D")
        {
            //camera.SetActive(false);
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().token2DLayer.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().prop2DLayer.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().map2DLayer.transform)
            {
                Destroy(child.gameObject);
            }
            GameManager.GetComponent<MapBrushDrawer>().ClearTexture();
        }
        else if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "3D")
        {
            //camera.SetActive(true);
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().token3DLayer.transform)
            {
                Destroy(child.gameObject.GetComponent<AssetName>());
                Destroy(child.gameObject.GetComponent<MovableProp>());
                Destroy(child.gameObject);
            }
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().prop3DLayer.transform)
            {
                Destroy(child.gameObject.GetComponent<AssetName>());
                Destroy(child.gameObject.GetComponent<MovableProp>());
                Destroy(child.gameObject);
            }
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().map3DLayer.transform)
            {
                if (child.name != "Terrain")
                {
                    Destroy(child.gameObject.GetComponent<AssetName>());
                    Destroy(child.gameObject.GetComponent<MovableProp>());
                    Destroy(child.gameObject);
                }
            }
        }
        terrain.GetComponent<PlaneHandler>().ClearTerrain();
        GameManager.GetComponent<SkyboxHandler>().ClearSkybox();
    }

    /// <summary>
    /// Saves current map state to file
    /// </summary>
    /// <remarks>
    /// Saves different data based on mode:
    /// - 2D: Layer objects + drawing data
    /// - 3D: Layer objects + terrain data + skybox
    /// </remarks>
    public void saveMap()
    {
        if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "2D")
        {
            List<ObjectData> tokenLayer = new List<ObjectData>();
            List<ObjectData> propLayer = new List<ObjectData>();
            List<ObjectData> mapLayer = new List<ObjectData>();
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().token2DLayer.transform)
            {
                ObjectData data = new ObjectData
                {
                    assetName = child.GetComponent<AssetName>()?.assetName ?? child.name,
                    position = child.localPosition,
                    rotation = child.rotation,
                    scale = child.localScale
                };
                tokenLayer.Add(data);
            }
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().prop2DLayer.transform)
            {
                ObjectData data = new ObjectData
                {
                    assetName = child.GetComponent<AssetName>()?.assetName ?? child.name,
                    position = child.localPosition,
                    rotation = child.rotation,
                    scale = child.localScale
                };
                propLayer.Add(data);
            }
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().map2DLayer.transform)
            {
                ObjectData data = new ObjectData
                {
                    assetName = child.GetComponent<AssetName>()?.assetName ?? child.name,
                    position = child.localPosition,
                    rotation = child.rotation,
                    scale = child.localScale
                };
                mapLayer.Add(data);
            }
            GameManager.GetComponent<MapBrushDrawer>().SaveDrawingToFile(Path.Combine(SettingsManager._CurrentSettings.MapsPath, "drawingData", $"{mapName}"));
            MapInfo mapInfo = new MapInfo
            {
                saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                mapType = GameManager.GetComponent<LayerSystem>()._GAME_MODE,
                tokenLayer = tokenLayer,
                propLayer = propLayer,
                mapLayer = mapLayer
            };
            string json = JsonUtility.ToJson(mapInfo, true);
            string filePath = Path.Combine(SettingsManager._CurrentSettings.MapsPath, $"{mapName}");
            File.WriteAllText(filePath, json);
        }
        else if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "3D")
        {
            List<ObjectData> tokenLayer = new List<ObjectData>();
            List<ObjectData> propLayer = new List<ObjectData>();
            List<ObjectData> mapLayer = new List<ObjectData>();
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().token3DLayer.transform)
            {
                ObjectData data = new ObjectData
                {
                    assetName = child.GetComponent<AssetName>()?.assetName ?? child.name,
                    position = child.localPosition,
                    rotation = child.rotation,
                    scale = child.localScale
                };
                tokenLayer.Add(data);
            }
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().prop3DLayer.transform)
            {
                ObjectData data = new ObjectData
                {
                    assetName = child.GetComponent<AssetName>()?.assetName ?? child.name,
                    position = child.localPosition,
                    rotation = child.rotation,
                    scale = child.localScale
                };
                propLayer.Add(data);
            }
            foreach (Transform child in GameManager.GetComponent<LayerSystem>().map3DLayer.transform)
            {
                if (child.name == "Terrain")
                {
                    child.GetComponent<PlaneHandler>().ExportTerrain(Path.Combine(SettingsManager._CurrentSettings.MapsPath, $"terrainData/{mapName}"));
                }
                else
                {
                    ObjectData data = new ObjectData
                    {
                        assetName = child.GetComponent<AssetName>()?.assetName ?? child.name,
                        position = child.localPosition,
                        rotation = child.rotation,
                        scale = child.localScale
                    };
                    mapLayer.Add(data);

                }

            }
            MapInfo mapInfo = new MapInfo
            {
                saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                mapType = GameManager.GetComponent<LayerSystem>()._GAME_MODE,
                tokenLayer = tokenLayer,
                propLayer = propLayer,
                mapLayer = mapLayer,
                terrainTexturePath = terrain.GetComponent<PlaneHandler>().terrainTexturePath,
                terrainFill = terrain.GetComponent<PlaneHandler>().fillTexture.isOn,
                skyboxTexturepath = GameManager.GetComponent<SkyboxHandler>().skyboxTexturePath
            };
            string json = JsonUtility.ToJson(mapInfo, true);
            string filePath = Path.Combine(SettingsManager._CurrentSettings.MapsPath, $"{mapName}");
            File.WriteAllText(filePath, json);
        }
    }


    /// <summary>
    /// Handles periodic auto-saving and camera control
    /// </summary>
    void Update()
    {

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            if (!string.IsNullOrEmpty(mapName))
            {
                saveMap();
                Debug.Log("Map saved successfully!");
            }
            else
            {
                Debug.Log("Map name is not set.");
            }
        }
        if (CreateMapWindow.activeSelf && camera.activeSelf)
        {
            camera.GetComponent<FreeCameraController>().enabled = false; // Disable camera movement while creating map
        }
        else
        {
            camera.GetComponent<FreeCameraController>().enabled = true; // Enable camera movement when not creating map
        }
    }


    /// <summary>
    /// Loads a map from file
    /// </summary>
    /// <param name="mName">Map filename to load</param>
    /// <remarks>
    /// - Automatically saves current map before loading
    /// - Handles both 2D and 3D map types
    /// - Restores all layer objects
    /// - Loads additional data (drawings/terrain)
    /// </remarks>
    public void loadMap(string mName)
    {
        if (mapName != null && mapName != "")
        {
            saveMap(); // Save the current map before loading a new one
            ClearMap(); // Clear the current map before loading a new one
        }

        mapName = mName;
        string filePath = Path.Combine(SettingsManager._CurrentSettings.MapsPath, $"{mName}");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            MapInfo mapInfo = JsonUtility.FromJson<MapInfo>(json);

            GameManager.GetComponent<LayerSystem>()._GAME_MODE = mapInfo.mapType;

            if (mapInfo.mapType == "2D")
            {
                foreach (ObjectData token in mapInfo.tokenLayer)
                {
                    GameManager.GetComponent<AssetLoaderAndPlacer>().PlaceToken(token.assetName, token.position, token.rotation, token.scale, 0);
                }
                foreach (ObjectData prop in mapInfo.propLayer)
                {
                    GameManager.GetComponent<AssetLoaderAndPlacer>().PlaceToken(prop.assetName, prop.position, prop.rotation, prop.scale, 1);
                }
                foreach (ObjectData map in mapInfo.mapLayer)
                {
                    GameManager.GetComponent<AssetLoaderAndPlacer>().PlaceToken(map.assetName, map.position, map.rotation, map.scale, 2);
                }
                // Load drawing data if available
                string drawingDataPath = Path.Combine(SettingsManager._CurrentSettings.MapsPath, "drawingData", $"{mName}");
                if (File.Exists(drawingDataPath))
                {
                    GameManager.GetComponent<MapBrushDrawer>().LoadDrawingFromFile(drawingDataPath);
                }
                else
                {
                    Debug.LogWarning($"Drawing data not found for map: {mName}");
                }

            }
            else if (mapInfo.mapType == "3D")
            {
                foreach (ObjectData token in mapInfo.tokenLayer)
                {
                    GameManager.GetComponent<AssetLoaderAndPlacer>().PlaceToken(token.assetName, token.position, token.rotation, token.scale, 0);
                }
                foreach (ObjectData prop in mapInfo.propLayer)
                {
                    GameManager.GetComponent<AssetLoaderAndPlacer>().PlaceToken(prop.assetName, prop.position, prop.rotation, prop.scale, 1);
                }
                foreach (ObjectData map in mapInfo.mapLayer)
                {
                    GameManager.GetComponent<AssetLoaderAndPlacer>().PlaceToken(map.assetName, map.position, map.rotation, map.scale, 2);
                }
                // Load terrain data if available
                string terrainDataPath = Path.Combine(SettingsManager._CurrentSettings.MapsPath, "terrainData", $"{mName}");
                if (File.Exists(terrainDataPath))
                {
                    terrain.GetComponent<PlaneHandler>().ImportTerrain(terrainDataPath);
                }
                else
                {
                    Debug.LogWarning($"Terrain data not found for map: {mName}");
                }
                // Load terrain texture
                GameManager.GetComponent<SkyboxHandler>().ChangeSkybox(mapInfo.skyboxTexturepath);
                terrain.GetComponent<PlaneHandler>().ChangeTerrainTexture(mapInfo.terrainTexturePath);
                terrain.GetComponent<PlaneHandler>().fillTexture.isOn = mapInfo.terrainFill;
            }


            Debug.Log($"Map loaded from: {filePath}");
        }
        else
        {
            Debug.LogError($"Map file not found: {filePath}");
        }
    }

    /// <summary>
    /// Opens the map creation UI window
    /// </summary>
    public void OpenCreateMapWindow()
    {
        CreateMapWindow.SetActive(true);
        mapTypeWhileCreatingMap.GetComponent<TMP_Dropdown>().value = 0;
        mapNameWhileCreatingMap.GetComponent<TMP_InputField>().text = "";
    }


    /// <summary>
    /// Cancels map creation and resets UI
    /// </summary>
    public void CancelMapCreation()
    {
        CreateMapWindow.SetActive(false);
        mapTypeWhileCreatingMap.GetComponent<TMP_Dropdown>().value = 0;
        mapNameWhileCreatingMap.GetComponent<TMP_InputField>().text = "";
    }

}


/// <summary>
/// Container for map metadata and content
/// </summary>
[System.Serializable]
public class MapInfo
{
    public string saveTime;                // Timestamp of last save
    public string mapType;                 // "2D" or "3D"
    public string terrainTexturePath;      // Path to terrain texture
    public bool terrainFill;               // Whether terrain uses fill texture
    public string skyboxTexturepath;       // Path to skybox texture
    public List<ObjectData> tokenLayer;    // Token layer objects
    public List<ObjectData> propLayer;     // Prop layer objects
    public List<ObjectData> mapLayer;      // Map layer objects
}

/// <summary>
/// Serialized data for individual map objects
/// </summary>
[System.Serializable]
public class ObjectData
{
    public string assetName;       // Name/path of the asset
    public Vector3 position;      // World position
    public Quaternion rotation;   // Object rotation
    public Vector3 scale;         // Object scale
    public float[,] heightMap;    // Terrain heightmap (3D only)
    public bool[,] holeMap;       // Terrain holes (3D only)}
}