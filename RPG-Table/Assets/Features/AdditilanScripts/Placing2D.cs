using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Placing2D : MonoBehaviour
{
    public string asset;
    public GameObject GameManager;
    public GameObject imagePrefab; // To powinien być prefab z komponentem Image

    public Terrain terrain;
    public GameObject placedObject;
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
        else
        {
            TerrainLayer terrainLayer = new TerrainLayer();
            terrainLayer.diffuseTexture = texture;
            terrainLayer.tileSize = new Vector2(700, 700);
            terrain.terrainData.terrainLayers = new TerrainLayer[] { terrainLayer };
        }
    
    }

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

    private void CancelPlacement()
    {
        if (placedObject != null)
        {
            Destroy(placedObject);
            placedObject = null;
        }
    }
    private void SetObjectComponentsEnabled(bool enabled)
    {
        foreach (var collider in placedObject.GetComponents<Collider2D>())
            collider.enabled = enabled;


        foreach (var behaviour in placedObject.GetComponents<MonoBehaviour>())
            behaviour.enabled = enabled;
    }
}