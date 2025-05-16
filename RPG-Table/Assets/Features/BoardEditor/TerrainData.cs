public class TerrainData
{
    public int width { get; set; }
    public int height { get; set; }
    public float[,] heightMap { get; set; }
    public bool[,] holeMap { get; set; }
}
