using UnityEngine;

public class ApplyDrawnTexture : MonoBehaviour
{
    public MapBrushDrawer brushDrawer;
    public Renderer planeRenderer;

    void Update()
    {
        if (brushDrawer != null && planeRenderer != null)
        {
            Texture2D drawnTexture = brushDrawer.GetDrawnTexture();
            planeRenderer.material.mainTexture = drawnTexture;
        }
    }
}
