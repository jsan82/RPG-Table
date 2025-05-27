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
public class AssetLoaderAndPlacer : MonoBehaviour
{

    public GameObject assetPanel;
    public GameObject asset2DAssetPanel;
    public GameObject asset3DAssetPanel;
    public GameObject buttonPrefab2D;
    public GameObject buttonPrefab3D;
    public GameObject playerCardArea;
    public GameObject PlayerCardWindow;
    public GameObject PlayerCardPanel;
    private List<string> fileNames2D = new List<string>();
    private List<string> fileNames3D = new List<string>();
    private List<string> fileNamesPlayerCards = new List<string>();
    private string PATH_TO_2D_ASSETS = SettingsManager._CurrentSettings.Assets2DPath;
    private string PATH_TO_3D_ASSETS = SettingsManager._CurrentSettings.Assets3DPath;
    private string PATH_TO_PLAYER_CARDS = SettingsManager._CurrentSettings.GameCardsPath;
    private LoadPlayerCard lpc;
    // Start is called before the first frame update
    void Start()
    {

        if (!Directory.Exists(PATH_TO_2D_ASSETS))
        {
            Directory.CreateDirectory(PATH_TO_2D_ASSETS);
        }

        if (!Directory.Exists(PATH_TO_3D_ASSETS))
        {
            Directory.CreateDirectory(PATH_TO_3D_ASSETS);
        }
        if (!Directory.Exists(PATH_TO_PLAYER_CARDS))
        {
            Directory.CreateDirectory(PATH_TO_PLAYER_CARDS);
        }

        //2D Assets
        fileNames2D.AddRange(Directory.GetFiles(PATH_TO_2D_ASSETS, "*.png"));
        fileNames2D.AddRange(Directory.GetFiles(PATH_TO_2D_ASSETS, "*.jpg"));
        foreach (string filePath in fileNames2D)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            GameObject button = Instantiate(buttonPrefab2D, asset2DAssetPanel.transform);
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

        //3D Assets

        fileNames3D.AddRange(Directory.GetFiles(PATH_TO_3D_ASSETS, "*.obj"));
        fileNames3D.AddRange(Directory.GetFiles(PATH_TO_3D_ASSETS, "*.fbx"));

        foreach (string filePath in fileNames3D)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            GameObject button = Instantiate(buttonPrefab3D, asset3DAssetPanel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
        }

        //Player Cards
        fileNamesPlayerCards.AddRange(Directory.GetFiles(PATH_TO_PLAYER_CARDS, "*.json"));
        foreach (string filePath in fileNamesPlayerCards)
        {   
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Debug.Log(fileName);
            GameObject button = Instantiate(buttonPrefab3D, PlayerCardPanel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
            button.name = fileName; // Set the name of the button to the file name
            lpc = new LoadPlayerCard();
            
            button.GetComponent<Button>().onClick.AddListener(() => lpc.loadPlayerCard(fileName + ".json", playerCardArea, PlayerCardWindow));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void addCharacter()
    {
        
    }
}
