using UnityEngine;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Handles 2D asset placement in the UI layer system
/// </summary>
/// <remarks>
/// Features:
/// - Loads and places 2D assets from files
/// - Manages placement preview with mouse tracking
/// - Handles placement cancellation
/// - Integrates with LayerSystem for proper positioning
/// </remarks>
public class Placing2D : MonoBehaviour
{
    [Header("Configuration")]
    public string asset;               // Path to the asset file
    public GameObject GameManager;     // Reference to game manager
    public GameObject imagePrefab;     // Prefab with Image component for instantiation

    [Header("Runtime State")]
    public GameObject placedObject;    // Currently placed/previewed object

    /// <summary>
    /// Places the configured asset in the game world
    /// </summary>
    /// <remarks>
    /// - Validates asset path
    /// - Loads texture from file
    /// - Creates UI Image with proper sprite
    /// - Adds necessary components (SmartDragHandler, AssetName)
    /// - Positions at mouse location
    /// </remarks>
    public void PlaceAsset()
    {
        if (string.IsNullOrEmpty(asset))
        {
            Debug.LogError("Asset is not set.");
            return;
        }

        string filePath = Path.Combine(SettingsManager._CurrentSettings.Assets2DPath, asset);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Asset file not found: {filePath}");
            return;
        }

        // Ładowanie tekstury
        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = File.ReadAllBytes(filePath);
        texture.LoadImage(fileData);

        if (GameManager.GetComponent<LayerSystem>()._GAME_MODE == "2D")
        {
            // Tworzenie sprite
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Instancjonowanie obiektu UI
            placedObject = Instantiate(imagePrefab, GameManager.GetComponent<LayerSystem>()._CURRENT_LAYER.transform);

            // Ustawianie Image
            Image img = placedObject.GetComponent<Image>();
            if (img == null)
            {
                img = placedObject.AddComponent<Image>();
            }
            img.sprite = sprite;
            img.preserveAspect = true;
            placedObject.AddComponent<SmartDragHandler>();

            RectTransform rt = placedObject.GetComponent<RectTransform>();
            rt.anchoredPosition = Input.mousePosition;
            rt.localScale = Vector3.one;
            placedObject.AddComponent<AssetName>();
            placedObject.GetComponent<AssetName>().assetName = Path.GetFileName(filePath);
        }


    }


    /// <summary>
    /// Updates placement preview and handles input
    /// </summary>
    /// <remarks>
    /// - Tracks mouse position for preview
    /// - Handles placement confirmation (left click)
    /// - Handles cancellation (ESC key)
    /// </remarks>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
        if (placedObject != null)
        {

            if (Input.GetMouseButtonDown(0))
            {
                placedObject = null;
                return;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
           GameManager.GetComponent<LayerSystem>()._CURRENT_LAYER.GetComponent<RectTransform>(),
           Input.mousePosition,
           null, // Dla UI
           out Vector2 localPoint);

            placedObject.GetComponent<RectTransform>().anchoredPosition = localPoint;

        }
    }

    /// <summary>
    /// Cancels current placement operation
    /// </summary>
    /// <remarks>
    /// Destroys preview object if exists
    /// </remarks>
    private void CancelPlacement()
    {
        if (placedObject != null)
        {
            Destroy(placedObject);
            placedObject = null;
        }
    }

    /// <summary>
    /// Enables/disables components on placed object
    /// </summary>
    /// <param name="enabled">Whether to enable components</param>
    /// <remarks>
    /// Affects:
    /// - All Collider2D components
    /// - All MonoBehaviour components
    /// </remarks>
    private void SetObjectComponentsEnabled(bool enabled)
    {
        foreach (var collider in placedObject.GetComponents<Collider2D>())
            collider.enabled = enabled;


        foreach (var behaviour in placedObject.GetComponents<MonoBehaviour>())
            behaviour.enabled = enabled;
    }
}