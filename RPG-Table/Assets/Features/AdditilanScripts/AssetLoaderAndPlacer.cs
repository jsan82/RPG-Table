using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;
using TMPro;
public class AssetLoaderAndPlacer : MonoBehaviour
{

    public GameObject assetPanel;
    public GameObject asset2DAssetPanel;
    public GameObject asset3DAssetPanel;
    public GameObject buttonPrefab;
    private List<string> fileNames = new List<string>();

    private string PATH_TO_2D_ASSETS = SettingsManager._CurrentSettings.Assets2DPath;
    // Start is called before the first frame update
    void Start()
    {
    
        if (!Directory.Exists(PATH_TO_2D_ASSETS))
        {
            Directory.CreateDirectory(PATH_TO_2D_ASSETS);
        }
        fileNames.AddRange(Directory.GetFiles(PATH_TO_2D_ASSETS, "*.png"));
        fileNames.AddRange(Directory.GetFiles(PATH_TO_2D_ASSETS, "*.jpg"));
        foreach (string filePath in fileNames)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            GameObject button = Instantiate(buttonPrefab, asset2DAssetPanel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
            Image image = button.transform.Find("photo").GetComponent<Image>();
            Texture2D texture = new Texture2D(2, 2);
            if (image == null)
            {
                Debug.LogError("Image component not found in button prefab.");
                continue;
            }
            byte[] fileData = File.ReadAllBytes(filePath);
            
            if (texture.LoadImage(fileData)) 
            {

                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f) 
                );
                

                image.sprite = sprite;
            }
            else
            {
                Debug.LogError("Failed to load image at path: " + filePath);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (assetPanel.activeSelf)
        {
            
        }
    }
}
