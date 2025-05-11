using System;
using UnityEngine;
using UnityEngine.UI;

public class MapBrushDrawer : MonoBehaviour
{
    public RawImage drawingSurface;
    public Texture2D brushTexture;
    public int brushSize = 16;
    public Color brushColor = Color.black;

    private Texture2D drawingTexture;
    private RectTransform drawRect;
    private bool isDrawing = false;

    void Start()
    {
        drawRect = drawingSurface.rectTransform;
        drawingTexture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
        ClearTexture();

        drawingSurface.texture = drawingTexture;
    }

    void Update()
    {
        if (!isDrawing) return;

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
                Console.WriteLine(x);
                Console.WriteLine(y);
                DrawCircle((int)x, (int)y);
                drawingTexture.Apply();
            }
        }
    }

    void DrawCircle(int x, int y)
    {
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
                        drawingTexture.SetPixel(px, py, brushColor);
                    }
                }
            }
        }
    }

    public void ToggleDrawing()
    {
        isDrawing = !isDrawing;
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

}
