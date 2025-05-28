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
    public GameObject imagePrefab;
    public GameObject assetPanel;
    public GameObject asset2DAssetPanel;
    public GameObject asset3DAssetPanel;
    public GameObject buttonPrefab2D;
    public GameObject buttonPrefab3D;
    public GameObject playerCardArea;
    public GameObject PlayerCardWindow;
    public GameObject PlayerCardPanel;
    public GameObject MapPanel;
    private List<string> fileNames2D = new List<string>();
    private List<string> fileNames3D = new List<string>();
    private List<string> fileNamesPlayerCards = new List<string>();
    private List<string> fileNamesMaps = new List<string>();
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
                
                button.AddComponent<Placing2D>();
                button.GetComponent<Placing2D>().asset = filePath;
                button.GetComponent<Placing2D>().GameManager = playerCardArea;
                button.GetComponent<Placing2D>().imagePrefab = imagePrefab;
                button.GetComponent<Button>().onClick.AddListener(() => button.GetComponent<Placing2D>().PlaceAsset());
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
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                playerCardArea.GetComponent<PropHandler>().LoadOBJFromPath(filePath);
                playerCardArea.GetComponent<PropHandler>().spawnActive = true;
                playerCardArea.GetComponent<PropHandler>().spawnObjectName = Path.GetFileName(filePath);
            });
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

        //Maps
        fileNamesMaps.AddRange(Directory.GetFiles(SettingsManager._CurrentSettings.MapsPath, "*.json"));
        foreach (string filePath in fileNamesMaps)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Debug.Log(fileName);
            GameObject button = Instantiate(buttonPrefab3D, MapPanel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
            button.name = fileName; // Set the name of the button to the file name
            button.GetComponent<Button>().onClick.AddListener(() => playerCardArea.GetComponent<SaveLoadMap>().loadMap($"{fileName}.json"));
        }
        
    }
    
    public void restartMaps()
    {
        foreach (Transform child in MapPanel.transform)
        {
            Destroy(child.gameObject);
        }
        fileNamesMaps.Clear();
        fileNamesMaps.AddRange(Directory.GetFiles(SettingsManager._CurrentSettings.MapsPath, "*.json"));
        foreach (string filePath in fileNamesMaps)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            Debug.Log(fileName);
            GameObject button = Instantiate(buttonPrefab3D, MapPanel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
            button.name = fileName; // Set the name of the button to the file name
            button.GetComponent<Button>().onClick.AddListener(() => playerCardArea.GetComponent<SaveLoadMap>().loadMap($"{fileName}.json"));
        }
    }

    public void PlaceToken(string assetName, Vector3 position, Quaternion rotation, Vector3 scale, int layerIndex = 0)
    {
        if (playerCardArea.GetComponent<LayerSystem>()._GAME_MODE == "2D")
        {
            switch (layerIndex)
            {
                case 0: // Token Layer
                    Texture2D texture = new Texture2D(2, 2);
                    byte[] fileData = File.ReadAllBytes(Path.Combine(SettingsManager._CurrentSettings.Assets2DPath, assetName));
                    texture.LoadImage(fileData);
                    Sprite sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    GameObject token = Instantiate(imagePrefab, playerCardArea.GetComponent<LayerSystem>().token2DLayer.transform);

                    token.name = assetName;

                    token.GetComponent<Image>().sprite = sprite;
                    token.GetComponent<Image>().preserveAspect = true;


                    RectTransform rectTransform = token.GetComponent<RectTransform>();

                    rectTransform.anchoredPosition = position;
                    rectTransform.localScale = scale;
                    token.AddComponent<SmartDragHandler>();
                    token.AddComponent<AssetName>();
                    token.GetComponent<AssetName>().assetName = assetName;
                    break;

                case 1: // Prop Layer
                    Texture2D propTexture = new Texture2D(2, 2);
                    byte[] propFileData = File.ReadAllBytes(Path.Combine(SettingsManager._CurrentSettings.Assets2DPath, assetName));
                    Sprite propSprite = Sprite.Create(
                        propTexture,
                        new Rect(0, 0, propTexture.width, propTexture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    GameObject prop = Instantiate(imagePrefab, playerCardArea.GetComponent<LayerSystem>().prop2DLayer.transform);
                    prop.name = assetName;
                    prop.GetComponent<Image>().sprite = propSprite;
                    prop.GetComponent<Image>().preserveAspect = true;


                    RectTransform propRectTransform = prop.GetComponent<RectTransform>();
                    propRectTransform.anchoredPosition = position;
                    propRectTransform.localScale = scale;
                    prop.AddComponent<SmartDragHandler>();
                    prop.AddComponent<AssetName>();
                    prop.GetComponent<AssetName>().assetName = assetName;
                    break;

                case 2: // Map Layer
                    Texture2D mapTexture = new Texture2D(2, 2);
                    byte[] mapFileData = File.ReadAllBytes(Path.Combine(SettingsManager._CurrentSettings.MapsPath, assetName));
                    mapTexture.LoadImage(mapFileData);
                    Sprite mapSprite = Sprite.Create(
                        mapTexture,
                        new Rect(0, 0, mapTexture.width, mapTexture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                    GameObject mapObject = Instantiate(imagePrefab, playerCardArea.GetComponent<LayerSystem>().map2DLayer.transform);
                    mapObject.name = assetName;
                    mapObject.GetComponent<Image>().sprite = mapSprite;
                    mapObject.GetComponent<Image>().preserveAspect = true;
                    RectTransform mapRectTransform = mapObject.GetComponent<RectTransform>();
                    mapRectTransform.anchoredPosition = position;
                    mapRectTransform.localScale = scale;
                    mapObject.AddComponent<SmartDragHandler>();
                    mapObject.AddComponent<AssetName>();
                    mapObject.GetComponent<AssetName>().assetName = assetName;
                    break;

                default:
                    Debug.LogError("Invalid layer index for token placement.");
                    break;
            }
        }
        else if (playerCardArea.GetComponent<LayerSystem>()._GAME_MODE == "3D")
        {
            // Implement 3D token placement logic here
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
