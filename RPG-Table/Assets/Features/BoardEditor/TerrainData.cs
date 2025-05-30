using System.Collections.Generic;

[System.Serializable]
public class TerrainData
{
    public float brushDefoultSave;
    public int heightmapWidth;
    public int heightmapHeight;
    public int holemapWidth;
    public int holemapHeight;

    public List<float> heightMap;
    public List<bool> holeMap;
}
