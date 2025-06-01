using System.Collections.Generic;

/// <summary>
/// Serializable class that holds terrain-related data including height and hole maps.
/// </summary>
[System.Serializable]
public class TerrainData
{
    /// <summary>
    /// The default brush strength value used for terrain editing or painting.
    /// </summary>
    public float brushDefoultSave;
    /// <summary>
    /// Width of the heightmap in pixels or units.
    /// </summary>
    public int heightmapWidth;
    /// <summary>
    /// Height of the heightmap in pixels or units.
    /// </summary>
    public int heightmapHeight;
    /// <summary>
    /// Width of the holemap, typically matching the terrain tile's resolution.
    /// </summary>
    public int holemapWidth;
    /// <summary>
    /// Height of the holemap, typically matching the terrain tile's resolution.
    /// </summary>
    public int holemapHeight;
    /// <summary>
    /// A flat list representing the height values of the terrain. 
    /// Typically normalized between 0 and 1.
    /// </summary>
    public List<float> heightMap;
    /// <summary>
    /// A flat list representing holes in the terrain.
    /// True indicates a hole; False indicates solid terrain.
    /// </summary>
    public List<bool> holeMap;
}
