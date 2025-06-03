using UnityEngine;

/// <summary>
/// Applies a drawn texture from a MapBrushDrawer to a target Renderer's material.
/// </summary>
public class ApplyDrawnTexture : MonoBehaviour
{
    /// <summary>
    /// Reference to the MapBrushDrawer component containing the drawn texture.
    /// </summary>
    public MapBrushDrawer brushDrawer;
    
    /// <summary>
    /// The Renderer component whose material will receive the drawn texture.
    /// </summary>
    public Renderer planeRenderer;

    /// <summary>
    /// Updates the target renderer's texture with the current drawn texture every frame.
    /// Ensures both required components are available before applying the texture.
    /// </summary>
    void Update()
    {
        if (brushDrawer != null && planeRenderer != null)
        {
            Texture2D drawnTexture = brushDrawer.GetDrawnTexture();
            planeRenderer.material.mainTexture = drawnTexture;
        }
    }
}