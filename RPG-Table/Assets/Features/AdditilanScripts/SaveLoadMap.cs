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


public class SaveLoadMap : MonoBehaviour
{
    public GameObject mapTypeWhileCreatingMap;
    public GameObject mapNameWhileCreatingMap;
    public GameObject GameManager;

    public GameObject CreateMapWindow;

    public string mapName;
    float timer = 0f;
    float interval = 10f;


    public void createMap()
    {
        MapInfo mapInfo = new MapInfo();
        mapInfo.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        mapInfo.mapType = mapTypeWhileCreatingMap.GetComponent<TMP_Dropdown>().options[mapTypeWhileCreatingMap.GetComponent<TMP_Dropdown>().value].text;

        //GameManager.GetComponent<LayerSystem>()._GAME_MODE = mapInfo.mapType;

        mapName = mapNameWhileCreatingMap.GetComponent<TMP_InputField>().text + ".json";

        string json = JsonUtility.ToJson(mapInfo, true);
        string filePath = Path.Combine(SettingsManager._CurrentSettings.MapsPath, $"{mapName}");
        File.WriteAllText(filePath, json);
        CreateMapWindow.SetActive(false);
        loadMap(mapName);
        CancelMapCreation();
        GameManager.GetComponent<AssetLoaderAndPlacer>().restartMaps();
    }

    public void ClearMap()
    {
        if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "2D")
        {
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
        }
        else if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "3D")
        {
            // Implement clearing logic for 3D layers if needed
        }
    }
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

        }
    }

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
    }

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


            Debug.Log($"Map loaded from: {filePath}");
        }
        else
        {
            Debug.LogError($"Map file not found: {filePath}");
        }
    }

    public void OpenCreateMapWindow()
    {
        CreateMapWindow.SetActive(true);
        mapTypeWhileCreatingMap.GetComponent<TMP_Dropdown>().value = 0;
        mapNameWhileCreatingMap.GetComponent<TMP_InputField>().text = "";
    }
    public void CancelMapCreation()
    {
        CreateMapWindow.SetActive(false);
        mapTypeWhileCreatingMap.GetComponent<TMP_Dropdown>().value = 0;
        mapNameWhileCreatingMap.GetComponent<TMP_InputField>().text = "";
    }

}


[System.Serializable]
public class MapInfo
{
    public string saveTime;
    public string mapType;
    public List<ObjectData> tokenLayer;
    public List<ObjectData> propLayer;
    public List<ObjectData> mapLayer;
}

[System.Serializable]
public class ObjectData
{
    public string assetName;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}