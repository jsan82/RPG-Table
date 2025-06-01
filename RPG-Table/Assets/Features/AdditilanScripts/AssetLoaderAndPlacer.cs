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
/// Central manager for loading and placing 2D/3D assets in the game environment
/// </summary>
/// <remarks>
/// Handles:
/// - Loading and displaying available assets from directories
/// - Creating UI buttons for asset selection
/// - Placing assets in appropriate layers
/// - Managing 2D/3D mode switching
/// - Supporting textures, skyboxes, player cards and maps
/// </remarks>
public class AssetLoaderAndPlacer : MonoBehaviour
{
    private SFXManager _sfxManager;
    public GameObject imagePrefab;
    public GameObject assetPanel;
    public GameObject asset2DAssetPanel;
    public GameObject asset2DPanel;
    public GameObject asset2Dtext;
    public GameObject texturePanel;
    public GameObject skyboxPanel;
    public GameObject asset3DAssetPanel;
    public GameObject asset3DPanel;
    public GameObject asset3Dtext;
    public GameObject buttonPrefab2D;
    public GameObject buttonPrefab3D;
    public GameObject GameManager;
    public GameObject PlayerCardWindow;
    public GameObject PlayerCardPanel;
    public GameObject PlayerCardNotes;

    public Terrain terrain;

    public GameObject MapPanel;
    private List<string> fileNames2D = new List<string>();
    private List<string> fileNames3D = new List<string>();
    private List<string> fileNamesPlayerCards = new List<string>();
    private List<string> fileNamesMaps = new List<string>();
    private string PATH_TO_2D_ASSETS = SettingsManager._CurrentSettings.Assets2DPath;
    private string PATH_TO_3D_ASSETS = SettingsManager._CurrentSettings.Assets3DPath;
    private string PATH_TO_PLAYER_CARDS = SettingsManager._CurrentSettings.GameCardsPath;
    private LoadPlayerCard lpc;

    // <summary>
    /// Initializes asset directories and populates UI panels
    /// </summary>
    /// <remarks>
    /// Loads assets from configured paths and creates:
    /// - 2D asset buttons with preview images
    /// - Terrain texture buttons
    /// - Skybox selection buttons
    /// - 3D model buttons
    /// - Player card buttons
    /// - Map selection buttons
    /// </remarks>
    void Start()
    {
        _sfxManager = FindObjectOfType<SFXManager>();

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
                button.GetComponent<Placing2D>().GameManager = GameManager;
                button.GetComponent<Placing2D>().imagePrefab = imagePrefab;
                button.GetComponent<Button>().onClick.AddListener(() => button.GetComponent<Placing2D>().PlaceAsset());
                button.GetComponent<Button>().onClick.AddListener(() => _sfxManager.Play(SFXType.BUTTON_CLICK));
                image.sprite = sprite;
            }
            else
            {
                Debug.LogError("Failed to load image at path: " + filePath);
            }
        }
        foreach (string filePath in fileNames2D)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            GameObject button = Instantiate(buttonPrefab2D, texturePanel.transform);
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

                button.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                button.GetComponent<Button>().onClick.AddListener(() => terrain.GetComponent<PlaneHandler>().ChangeTerrainTexture(filePath));
                button.GetComponent<Button>().onClick.AddListener(() => _sfxManager.Play(SFXType.BUTTON_CLICK));
                image.sprite = sprite;
            }
            else
            {
                Debug.LogError("Failed to load image at path: " + filePath);
            }
        }
        //Skyboxes
        foreach (string filePath in fileNames2D)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            GameObject button = Instantiate(buttonPrefab2D, skyboxPanel.transform);
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
                button.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameManager.GetComponent<SkyboxHandler>().ChangeSkybox(filePath);
                });
                button.GetComponent<Button>().onClick.AddListener(() => _sfxManager.Play(SFXType.BUTTON_CLICK));
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
                GameManager.GetComponent<PropHandler>().spawnActive = true;
                GameManager.GetComponent<PropHandler>().LoadOBJFromPath(filePath);
                GameManager.GetComponent<PropHandler>().spawnObjectName = Path.GetFileName(filePath);
            });
            button.GetComponent<Button>().onClick.AddListener(() => _sfxManager.Play(SFXType.BUTTON_CLICK));
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

            button.GetComponent<Button>().onClick.AddListener(() => lpc.loadPlayerCard(fileName + ".json", GameManager, PlayerCardWindow, PlayerCardNotes));
            button.GetComponent<Button>().onClick.AddListener(() => _sfxManager.Play(SFXType.BUTTON_CLICK));
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
            button.GetComponent<Button>().onClick.AddListener(() => GameManager.GetComponent<SaveLoadMap>().loadMap($"{fileName}.json"));
            button.GetComponent<Button>().onClick.AddListener(() => _sfxManager.Play(SFXType.BUTTON_CLICK));
        }

    }

    /// <summary>
    /// Refreshes the map selection panel
    /// </summary>
    /// <remarks>
    /// Clears and repopulates the map panel with current map files
    /// </remarks>
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
            button.GetComponent<Button>().onClick.AddListener(() => GameManager.GetComponent<SaveLoadMap>().loadMap($"{fileName}.json"));
        }
    }

    /// <summary>
    /// Places an asset in the game world
    /// </summary>
    /// <param name="assetName">Name of the asset to place</param>
    /// <param name="position">World position for placement</param>
    /// <param name="rotation">Rotation of the placed asset</param>
    /// <param name="scale">Scale of the placed asset</param>
    /// <param name="layerIndex">Destination layer (0=Token, 1=Prop, 2=Map)</param>
    /// <remarks>
    /// Handles placement differently based on current game mode (2D/3D)
    /// Automatically adds required components (SmartDragHandler, AssetName)
    /// </remarks>
    public void PlaceToken(string assetName, Vector3 position, Quaternion rotation, Vector3 scale, int layerIndex = 0)
    {
        if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "2D")
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

                    GameObject token = Instantiate(imagePrefab, GameManager.GetComponent<LayerSystem>().token2DLayer.transform);

                    token.name = assetName;

                    token.GetComponent<Image>().sprite = sprite;
                    token.GetComponent<Image>().preserveAspect = true;


                    RectTransform rectTransform = token.GetComponent<RectTransform>();

                    rectTransform.anchoredPosition = position;
                    rectTransform.localScale = scale;
                    token.AddComponent<SmartDragHandler>();
                    token.GetComponent<SmartDragHandler>().Game = true;
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

                    GameObject prop = Instantiate(imagePrefab, GameManager.GetComponent<LayerSystem>().prop2DLayer.transform);
                    prop.name = assetName;
                    prop.GetComponent<Image>().sprite = propSprite;
                    prop.GetComponent<Image>().preserveAspect = true;


                    RectTransform propRectTransform = prop.GetComponent<RectTransform>();
                    propRectTransform.anchoredPosition = position;
                    propRectTransform.localScale = scale;
                    prop.AddComponent<SmartDragHandler>();
                    prop.GetComponent<SmartDragHandler>().Game = true;
                    prop.AddComponent<AssetName>();
                    prop.GetComponent<AssetName>().assetName = assetName;
                    break;

                case 2: // Map Layer
                    Texture2D mapTexture = new Texture2D(2, 2);
                    byte[] mapFileData = File.ReadAllBytes(Path.Combine(SettingsManager._CurrentSettings.Assets2DPath, assetName));
                    mapTexture.LoadImage(mapFileData);
                    Sprite mapSprite = Sprite.Create(
                        mapTexture,
                        new Rect(0, 0, mapTexture.width, mapTexture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                    GameObject mapObject = Instantiate(imagePrefab, GameManager.GetComponent<LayerSystem>().map2DLayer.transform);
                    mapObject.name = assetName;
                    mapObject.GetComponent<Image>().sprite = mapSprite;
                    mapObject.GetComponent<Image>().preserveAspect = true;
                    RectTransform mapRectTransform = mapObject.GetComponent<RectTransform>();
                    mapRectTransform.anchoredPosition = position;
                    mapRectTransform.localScale = scale;
                    mapObject.AddComponent<SmartDragHandler>();
                    mapObject.GetComponent<SmartDragHandler>().Game = true;
                    mapObject.AddComponent<AssetName>();
                    mapObject.GetComponent<AssetName>().assetName = assetName;
                    break;

                default:
                    Debug.LogError("Invalid layer index for token placement.");
                    break;
            }
        }
        else if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "3D")
        {
            switch (layerIndex)
            {
                case 0:
                    GameManager.GetComponent<PropHandler>().LoadOBJFromPath(Path.Combine(SettingsManager._CurrentSettings.Assets3DPath, assetName));
                    GameObject token3D = Instantiate(GameManager.GetComponent<PropHandler>().objectToSpawn, position, rotation);
                    token3D.transform.SetParent(GameManager.GetComponent<LayerSystem>().token3DLayer.transform);
                    token3D.name = assetName;
                    token3D.SetActive(true);

                    token3D.transform.localScale = scale;
                    token3D.AddComponent<AssetName>();
                    token3D.GetComponent<AssetName>().assetName = assetName;
                    break;
                case 1:
                    GameManager.GetComponent<PropHandler>().LoadOBJFromPath(Path.Combine(SettingsManager._CurrentSettings.Assets3DPath, assetName));
                    GameObject prop3D = Instantiate(GameManager.GetComponent<PropHandler>().objectToSpawn, position, rotation);
                    prop3D.transform.SetParent(GameManager.GetComponent<LayerSystem>().prop3DLayer.transform);
                    prop3D.name = assetName;
                    prop3D.transform.localScale = scale;
                    //prop3D.AddComponent<SmartDragHandler>();
                    prop3D.AddComponent<AssetName>();
                    prop3D.GetComponent<AssetName>().assetName = assetName;
                    prop3D.SetActive(true);
                    break;
                case 2:
                    GameManager.GetComponent<PropHandler>().LoadOBJFromPath(Path.Combine(SettingsManager._CurrentSettings.MapsPath, assetName));
                    GameObject map3D = Instantiate(GameManager.GetComponent<PropHandler>().objectToSpawn, position, rotation);
                    map3D.transform.SetParent(GameManager.GetComponent<LayerSystem>().map3DLayer.transform);
                    map3D.name = assetName;
                    // map3D.transform.position = position;
                    // map3D.transform.rotation = rotation;
                    map3D.transform.localScale = scale;
                    //map3D.AddComponent<SmartDragHandler>();
                    map3D.AddComponent<AssetName>();
                    map3D.GetComponent<AssetName>().assetName = assetName;
                    map3D.SetActive(true);
                    break;
            }

        }
    }

 
    /// <summary>
    /// Updates UI based on current game mode (2D/3D)
    /// </summary>
    /// <remarks>
    /// Shows/hides appropriate panels and indicators
    /// </remarks>
    void Update()
    {
        if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "2D")
        {
            asset2Dtext.SetActive(true);
            asset3Dtext.SetActive(false);
            asset2DPanel.SetActive(true);
            asset3DPanel.SetActive(false);
        }
        else if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "3D")
        {
            asset2Dtext.SetActive(false);
            asset3Dtext.SetActive(true);
            asset2DPanel.SetActive(false);
            asset3DPanel.SetActive(true);
        }
    }


}
