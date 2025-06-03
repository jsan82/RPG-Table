using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

/// <summary>
/// Enables drawing, erasing, and saving/loading textures on a UI canvas.
/// </summary>
public class MapBrushDrawer : MonoBehaviour
{
    /// <summary>
    /// The RawImage component that serves as the drawing surface.
    /// </summary>
    public RawImage drawingSurface;
    
    /// <summary>
    /// Texture used as the brush stamp (unused in current implementation).
    /// </summary>
    public Texture2D brushTexture;
    
    /// <summary>
    /// Current size of the brush in pixels.
    /// </summary>
    public int brushSize = 16;
    
    /// <summary>
    /// Current color of the brush.
    /// </summary>
    public Color brushColor = Color.black;
    
    /// <summary>
    /// Input field for setting brush color via hex code.
    /// </summary>
    public GameObject brushSizeColorField;
    
    /// <summary>
    /// Slider control for adjusting brush size.
    /// </summary>
    public Slider brushSizeSlider;
    
    private Texture2D drawingTexture;
    private RectTransform drawRect;
    
    /// <summary>
    /// Flag indicating whether drawing mode is active.
    /// </summary>
    public bool isDrawing = false;
    
    /// <summary>
    /// Toggle control for erasing mode.
    /// </summary>
    public Toggle ErasingToggle;
    
    /// <summary>
    /// Flag indicating whether erasing mode is active.
    /// </summary>
    public bool isErasing = false;
    private Vector2 previousDrawPosition;

    /// <summary>
    /// ScrollView container for the drawing surface.
    /// </summary>
    public GameObject scrollView;

    /// <summary>
    /// Initializes the drawing surface and sets up default values.
    /// Creates a blank texture and prepares the drawing interface.
    /// </summary>
    void Start()
    {
        drawRect = drawingSurface.rectTransform;
        drawingTexture = new Texture2D(3000, 3000, TextureFormat.RGBA32, false);
        ClearTexture();
        brushSizeColorField.GetComponent<TMP_InputField>().text = "000000";

        drawingSurface.texture = drawingTexture;
        previousDrawPosition = Vector2.zero;
    }

    /// <summary>
    /// Handles per-frame input processing for drawing operations.
    /// Manages drawing/erasing modes, brush properties, and mouse input.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isDrawing = false;
            isErasing = false;
            ErasingToggle.isOn = false;
            scrollView.GetComponent<ScrollRect>().enabled = true;
            drawingSurface.GetComponent<CanvasRenderer>().cullTransparentMesh = false;
        }
        if (!isDrawing && !isErasing) return;
        isErasing = ErasingToggle.isOn;
        brushSize = (int)brushSizeSlider.value;
        scrollView.GetComponent<ScrollRect>().enabled = false;
        drawingSurface.GetComponent<CanvasRenderer>().cullTransparentMesh = true;
        
        // Set color only when not in erasing mode
        if (!isErasing)
        {
            Debug.Log(brushSizeColorField.GetComponent<TMP_InputField>().text);
            brushColor = ColorUtility.TryParseHtmlString("#"+brushSizeColorField.GetComponent<TMP_InputField>().text, out Color parsedColor) ? parsedColor : Color.black;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 localCursor;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawRect, Input.mousePosition, null, out localCursor))
            {
                Vector2 pivotBased = new Vector2(
                    localCursor.x + drawRect.rect.width * 0.5f,
                    localCursor.y + drawRect.rect.height * 0.5f);

                float x = pivotBased.x / drawRect.rect.width * drawingTexture.width;
                float y = pivotBased.y / drawRect.rect.height * drawingTexture.height;

                Vector2 currentPosition = new Vector2(x, y);

                if (Input.GetMouseButtonDown(0))
                {
                    DrawCircle((int)x, (int)y);
                    previousDrawPosition = currentPosition;
                }
                else
                {
                    DrawLine(previousDrawPosition, currentPosition);
                    previousDrawPosition = currentPosition;
                }

                drawingTexture.Apply();
            }
        }
    }

    /// <summary>
    /// Draws a filled circle at the specified coordinates.
    /// </summary>
    /// <param name="x">Center x-coordinate in texture space.</param>
    /// <param name="y">Center y-coordinate in texture space.</param>
    void DrawCircle(int x, int y)
    {
        Color colorToUse = isErasing ? Color.clear : brushColor;

        for (int i = -brushSize; i < brushSize; i++)
        {
            for (int j = -brushSize; j < brushSize; j++)
            {
                if (i * i + j * j <= brushSize * brushSize)
                {
                    int px = x + i;
                    int py = y + j;
                    if (px >= 0 && px < drawingTexture.width && py >= 0 && py < drawingTexture.height)
                    {
                        drawingTexture.SetPixel(px, py, colorToUse);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws a line between two points using Bresenham's algorithm.
    /// </summary>
    /// <param name="start">Starting point in texture space.</param>
    /// <param name="end">Ending point in texture space.</param>
    void DrawLine(Vector2 start, Vector2 end)
    {
        // Bresenham's line algorithm
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            DrawCircle(x0, y0);

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
    
    /// <summary>
    /// Toggles drawing mode on/off.
    /// Automatically disables erasing mode when activated.
    /// </summary>
    public void ToggleDrawing()
    {
        isDrawing = !isDrawing;
        isErasing = false; // Disable erasing mode when enabling drawing
    }

    /// <summary>
    /// Toggles erasing mode on/off.
    /// Automatically disables drawing mode when activated.
    /// </summary>
    public void ToggleErasing()
    {
        isErasing = !isErasing;
        isDrawing = false; // Disable drawing mode when enabling erasing
    }

    /// <summary>
    /// Clears the entire drawing surface to transparent.
    /// </summary>
    public void ClearTexture()
    {
        for (int y = 0; y < drawingTexture.height; y++)
        {
            for (int x = 0; x < drawingTexture.width; x++)
            {
                drawingTexture.SetPixel(x, y, Color.clear);
            }
        }
        drawingTexture.Apply();
    }

    /// <summary>
    /// Gets the current drawing texture.
    /// </summary>
    /// <returns>The Texture2D containing the current drawing.</returns>
    public Texture2D GetDrawnTexture()
    {
        return drawingTexture;
    }

    /// <summary>
    /// Saves the current drawing to a file in JSON format.
    /// </summary>
    /// <param name="filePath">Full path to the destination file.</param>
    public void SaveDrawingToFile(string filePath)
    {
        try
        {
            DrawingData data = new DrawingData();
            data.width = drawingTexture.width;
            data.height = drawingTexture.height;

            // Convert texture to bytes
            byte[] textureBytes = drawingTexture.EncodeToPNG();
            // Convert bytes to Base64 string
            data.textureData = Convert.ToBase64String(textureBytes);

            data.brushSize = brushSize;
            data.brushColor = brushSizeColorField.GetComponent<TMP_InputField>().text;

            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);

            Debug.Log("Drawing saved successfully to: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving drawing: " + e.Message);
        }
    }

    /// <summary>
    /// Loads a drawing from a file and applies it to the drawing surface.
    /// </summary>
    /// <param name="filePath">Full path to the source file.</param>
    public void LoadDrawingFromFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("File not found: " + filePath);
                return;
            }

            string jsonData = File.ReadAllText(filePath);
            DrawingData data = JsonUtility.FromJson<DrawingData>(jsonData);

            // Convert Base64 string back to bytes
            byte[] textureBytes = Convert.FromBase64String(data.textureData);

            // Create new texture
            Texture2D loadedTexture = new Texture2D(data.width, data.height);
            loadedTexture.LoadImage(textureBytes); // This method automatically applies the texture

            // Replace current texture with loaded one
            Destroy(drawingTexture);
            drawingTexture = loadedTexture;
            drawingSurface.texture = drawingTexture;

            // Set brush parameters
            brushSizeSlider.value = data.brushSize;
            brushSizeColorField.GetComponent<TMP_InputField>().text = data.brushColor;
            brushColor = ColorUtility.TryParseHtmlString(data.brushColor, out Color parsedColor) ? parsedColor : Color.black;

            Debug.Log("Drawing loaded successfully from: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading drawing: " + e.Message);
        }
    }
}

/// <summary>
/// Serializable data structure for storing drawing information.
/// </summary>
[System.Serializable]
public class DrawingData
{
    /// <summary>
    /// Width of the drawing texture.
    /// </summary>
    public int width;
    
    /// <summary>
    /// Height of the drawing texture.
    /// </summary>
    public int height;
    
    /// <summary>
    /// Texture data encoded as Base64 string.
    /// </summary>
    public string textureData;
    
    /// <summary>
    /// Brush size at time of saving.
    /// </summary>
    public int brushSize;
    
    /// <summary>
    /// Brush color at time of saving as hex string.
    /// </summary>
    public string brushColor;
}