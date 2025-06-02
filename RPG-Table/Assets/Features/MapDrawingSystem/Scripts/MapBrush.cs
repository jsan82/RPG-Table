using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class MapBrushDrawer : MonoBehaviour
{
    public RawImage drawingSurface;
    public Texture2D brushTexture;
    public int brushSize = 16;
    public Color brushColor = Color.black;
    public GameObject brushSizeColorField;
    public Slider brushSizeSlider;
    private Texture2D drawingTexture;
    private RectTransform drawRect;
    public bool isDrawing = false;
    public Toggle ErasingToggle;
    public bool isErasing = false;
    private Vector2 previousDrawPosition;

    public GameObject scrollView;

    void Start()
    {
        drawRect = drawingSurface.rectTransform;
        drawingTexture = new Texture2D(3000, 3000, TextureFormat.RGBA32, false);
        ClearTexture();
        brushSizeColorField.GetComponent<TMP_InputField>().text = "000000";

        drawingSurface.texture = drawingTexture;
        previousDrawPosition = Vector2.zero;
    }

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
        
        // Ustaw kolor tylko gdy nie jesteśmy w trybie gumki
        if (!isErasing)
        {
            brushColor = ColorUtility.TryParseHtmlString(brushSizeColorField.GetComponent<TMP_InputField>().text, out Color parsedColor) ? parsedColor : Color.black;
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

    void DrawLine(Vector2 start, Vector2 end)
    {
        // Algorytm Bresenhama do rysowania linii
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
    
    public void ToggleDrawing()
    {
        isDrawing = !isDrawing;
        isErasing = false; // Wyłącz tryb gumki gdy włączamy rysowanie
    }

    // Nowa metoda do przełączania trybu gumki
    public void ToggleErasing()
    {
        isErasing = !isErasing;
        isDrawing = false; // Wyłącz tryb rysowania gdy włączamy gumkę
    }

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

    public Texture2D GetDrawnTexture()
    {
        return drawingTexture;
    }

    public void SaveDrawingToFile(string filePath)
    {
        try
        {
            DrawingData data = new DrawingData();
            data.width = drawingTexture.width;
            data.height = drawingTexture.height;

            // Konwersja tekstury do bajtów
            byte[] textureBytes = drawingTexture.EncodeToPNG();
            // Konwersja bajtów do Base64 string
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

            // Konwersja Base64 string z powrotem do bajtów
            byte[] textureBytes = Convert.FromBase64String(data.textureData);

            // Tworzenie nowej tekstury
            Texture2D loadedTexture = new Texture2D(data.width, data.height);
            loadedTexture.LoadImage(textureBytes); // Ta metoda automatycznie zastosuje teksturę

            // Zastąp obecną teksturę załadowaną
            Destroy(drawingTexture);
            drawingTexture = loadedTexture;
            drawingSurface.texture = drawingTexture;

            // Ustaw parametry pędzla
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

[System.Serializable]
public class DrawingData
{
    public int width;
    public int height;
    public string textureData; // Base64 zakodowana tekstura
    public int brushSize;
    public string brushColor;
}