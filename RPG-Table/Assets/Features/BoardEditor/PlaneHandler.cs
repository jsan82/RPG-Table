using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// Handles terrain modification including elevation, flattening, holes, and texture changes.
/// Provides tools for user interaction and terrain data import/export.
/// </summary>
public class PlaneHandler : MonoBehaviour
{
    /// <summary>Current brush size used for terrain modification.</summary>
    private float brushSize { get; set; }
    /// <summary>Current brush power (influence strength).</summary>
    private float brushPower { get; set; }
    /// <summary>Time tracker for continuous brush input.</summary>
    private float brushTimer;
    /// <summary>Minimum interval between brush applications.</summary>
    private float brushLimit { get; set; }
    /// <summary>Default terrain height used for resetting elevation.</summary>
    private float brushDefoult;
    /// <summary>Reference to the active terrain object.</summary>
    public Terrain terrain;
    /// <summary>TerrainData associated with the current terrain.</summary>
    private UnityEngine.TerrainData terrainData;
    /// <summary>Resolution of the terrain heightmap.</summary>
    private int heightmapResolution;
    /// <summary>Path to texture for terrain.</summary>
    public string terrainTexturePath;
    /// <summary>Switch for filling the terrain texture.</summary>
    public Toggle fillTexture;
    /// <summary>Switch for edit mode.</summary>
    public Toggle editModeOn;
    /// <summary>Slider variable for brush size.</summary>
    public Slider brushSizeSlider;

    /// <summary>
    /// Initializes brush settings and terrain data.
    /// </summary>
    void Start()
    {
        brushSize = 10.0f;
        brushPower = 0.01f;
        brushLimit = 0.01f;
        brushDefoult = 0.0f;

        terrainData = terrain.terrainData;
        heightmapResolution = terrainData.heightmapResolution;
        

        //ChangeTerrainTexture("[P A T H]"); //comment if not testing
    }

    /// <summary>
    /// Handles terrain editing logic each frame.
    /// </summary>
    void Update()
    {   
        if (!editModeOn.isOn) return;
        if(editModeOn.isOn && Input.GetKeyDown(KeyCode.Escape))
        {
            editModeOn.isOn = false;
            return;
        }
        HandleElevation();
        HandleAllElevation();
        HandleHole();
        ChangeBrushSize();
/*        if (Input.GetKeyDown(KeyCode.T))//comment if not testing
        {
            ExportTerrain("P A T H");
        }*/

/*        if (Input.GetKeyDown(KeyCode.G))//comment if not testing
        {
            ImportTerrain("P A T H");
        }*/
    }

    public void ChangeBrushSize()
    {
        brushSize = brushSizeSlider.value;
    }

    /// <summary>
    /// Sets a new terrain reference.
    /// </summary>
    /// <param name="newTerrain">The new terrain to use.</param>
    public void GiveTerrain(Terrain newTerrain)
    {
        terrain = newTerrain;
    }

    /// <summary>
    /// Handles terrain elevation at the mouse position (raise, lower, reset).
    /// </summary>
    private void HandleElevation()
    {
        bool plusHeld = Input.GetKey(KeyCode.J);
        bool minusHeld = Input.GetKey(KeyCode.K);
        bool resetHeld = Input.GetKey(KeyCode.L);

        bool plusDown = Input.GetKeyDown(KeyCode.J);
        bool minusDown = Input.GetKeyDown(KeyCode.K);
        bool resetDown = Input.GetKeyDown(KeyCode.L);

        brushTimer += Time.deltaTime;

        if (plusDown || (plusHeld && brushTimer >= brushLimit)) //J
        {
            brushTimer = 0f;
            ModifyTerrainAtPosition(0);
        }
        else if (minusDown || (minusHeld && brushTimer >= brushLimit)) //K
        {
            brushTimer = 0f;
            ModifyTerrainAtPosition(1);
        }
        else if (resetDown || (resetHeld && brushTimer >= brushLimit)) //L
        {
            brushTimer = 0f;
            ModifyTerrainAtPosition(2);
        }


        if (!plusHeld && !minusHeld && !resetHeld)
        {
            brushTimer = brushLimit;
        }
    }

    /// <summary>
    /// Raises or lowers the entire terrain uniformly.
    /// </summary>
    private void HandleAllElevation()
    {
        bool plusHeld = Input.GetKey(KeyCode.O);
        bool minusHeld = Input.GetKey(KeyCode.P);

        bool plusDown = Input.GetKeyDown(KeyCode.O);
        bool minusDown = Input.GetKeyDown(KeyCode.P);

        brushTimer += Time.deltaTime;

        if (plusDown || (plusHeld && brushTimer >= brushLimit)) //O
        {
            brushTimer = 0f;
            ModifyAllTerrain(true);
        }
        else if (minusDown || (minusHeld && brushTimer >= brushLimit)) //P
        {
            brushTimer = 0f;
            ModifyAllTerrain(false);
        }

        if (!plusHeld && !minusHeld)
        {
            brushTimer = brushLimit;
        }
    }

    /// <summary>
    /// Adds or removes holes in the terrain at the mouse position.
    /// </summary>
    private void HandleHole()
    {
        bool leftHeld = Input.GetKey(KeyCode.N);
        bool rightHeld = Input.GetKey(KeyCode.M);

        bool leftDown = Input.GetKeyDown(KeyCode.N);
        bool rightDown = Input.GetKeyDown(KeyCode.M);

        brushTimer += Time.deltaTime;

        if (leftDown || (leftHeld && brushTimer >= brushLimit)) //N
        {
            brushTimer = 0f;
            HoleTerrainAtPosition(true);
        }
        else if (rightDown || (rightHeld && brushTimer >= brushLimit)) //M
        {
            brushTimer = 0f;
            HoleTerrainAtPosition(false);
        }

        if (!leftHeld && !rightHeld)
        {
            brushTimer = brushLimit;
        }
    }

    /// <summary>
    /// Calculates terrain brush area and transformation details.
    /// </summary>
    /// <returns>
    /// Tuple of radius, start X/Z, width and height, or null if raycast fails.
    /// </returns>
    private (int modifRadius, int startX, int startZ, int width, int height)? CalcualteTerrainData()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit) || hit.collider.gameObject != terrain.gameObject)
            return null;

        Vector3 terrainPos = hit.point - terrain.transform.position;

        int xRes = terrainData.heightmapResolution;
        int yRes = terrainData.heightmapResolution;

        float relativeX = terrainPos.x / terrainData.size.x;
        float relativeZ = terrainPos.z / terrainData.size.z;

        int posX = Mathf.RoundToInt(relativeX * xRes);
        int posZ = Mathf.RoundToInt(relativeZ * yRes);

        int modifRadius = Mathf.RoundToInt((brushSize / terrainData.size.x) * xRes);
        int startX = Mathf.Clamp(posX - modifRadius, 0, xRes - 1);
        int startZ = Mathf.Clamp(posZ - modifRadius, 0, yRes - 1);

        int width = Mathf.Clamp((posX + modifRadius), 0, xRes) - startX;
        int height = Mathf.Clamp((posZ + modifRadius), 0, yRes) - startZ;

        return (modifRadius, startX, startZ, width, height);
    }

    /// <summary>
    /// Modifies the terrain heightmap at the cursor position.
    /// </summary>
    /// <param name="modTerrain">0 to raise, 1 to lower, 2 to reset.</param>
    private void ModifyTerrainAtPosition(int modTerrain)
    {
        var data = CalcualteTerrainData();
        if (data == null) return;
        var (modifRadius, startX, startZ, width, height) = data.Value;

        float[,] map = terrainData.GetHeights(startX, startZ, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2(width / 2, height / 2));
                float strength = Mathf.Clamp01(1f - (distance / (float)modifRadius)) * brushPower;

                if (modTerrain == 0) map[z, x] += strength;
                else if (modTerrain == 1) map[z, x] -= strength;
                else if (modTerrain == 2) map[z, x] = brushDefoult;

                map[z, x] = Mathf.Clamp01(map[z, x]);
            }
        }

        terrainData.SetHeights(startX, startZ, map);
    }

    /// <summary>
    /// Modifies the entire terrain uniformly based on the brushPower.
    /// </summary>
    /// <param name="raiseTerrain">True to raise, false to lower.</param>
    private void ModifyAllTerrain(bool raiseTerrain)
    {
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        float[,] heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        for (int x = 0; x < heightmapWidth; x++)
        {
            for (int y = 0; y < heightmapHeight; y++)
            {
                if (raiseTerrain)
                {
                    heights[y, x] += brushPower;
                }
                else
                {
                    heights[y, x] -= brushPower;
                }


                heights[y, x] = Mathf.Clamp01(heights[y, x]);
            }
        }

        terrainData.SetHeights(0, 0, heights);

        if (raiseTerrain)
        {
            brushDefoult += brushPower;
        }
        else
        {
            brushDefoult -= brushPower;
            if (brushDefoult < 0) brushDefoult = 0;
        }
    }

    /// <summary>
    /// Edits terrain holes at the cursor position.
    /// </summary>
    /// <param name="holeTerrain">True to add a hole, false to remove.</param>
    private void HoleTerrainAtPosition(bool holeTerrain)
    {
        var data = CalcualteTerrainData();
        if (data == null) return;
        var (modifRadius, startX, startZ, width, height) = data.Value;

        bool[,] map = terrainData.GetHoles(startX, startZ, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2(width / 2, height / 2));
                if (distance <= modifRadius)
                {
                    if (holeTerrain)
                        map[z, x] = false;
                    else
                        map[z, x] = true;
                }
            }
        }

        terrainData.SetHoles(startX, startZ, map);
    }

    /// <summary>
    /// Exports terrain data (height and holes) to a JSON file.
    /// </summary>
    /// <param name="outputPath">The file path to write to.</param>
    public void ExportTerrain(string outputPath)
    {
        int heightRes = terrainData.heightmapResolution;
        int holeRes = terrainData.holesResolution;

        float[,] heightMap2D = terrainData.GetHeights(0, 0, heightRes, heightRes);
        bool[,] holeMap2D = terrainData.GetHoles(0, 0, holeRes, holeRes);

        List<float> heightMap1D = new List<float>(heightRes * heightRes);
        List<bool> holeMap1D = new List<bool>(holeRes * holeRes);

        for (int y = 0; y < heightRes; y++)
        {
            for (int x = 0; x < heightRes; x++)
            {
                heightMap1D.Add(heightMap2D[y, x]);
            }
        }

        for (int y = 0; y < holeRes; y++)
        {
            for (int x = 0; x < holeRes; x++)
            {
                holeMap1D.Add(holeMap2D[y, x]);
            }
        }

        TerrainData exportData = new TerrainData
        {
            brushDefoultSave = brushDefoult,
            heightmapWidth = heightRes,
            heightmapHeight = heightRes,
            holemapWidth = holeRes,
            holemapHeight = holeRes,
            heightMap = heightMap1D,
            holeMap = holeMap1D
        };

        string json = JsonUtility.ToJson(exportData, true);
        File.WriteAllText(outputPath, json);
    }

    /// <summary>
    /// Imports terrain data (height and holes) from a JSON file.
    /// </summary>
    /// <param name="inputPath">The file path to read from.</param>
    public void ImportTerrain(string inputPath)
    {
        if (!File.Exists(inputPath))
            return;

        string json = File.ReadAllText(inputPath);
        TerrainData exportData = JsonUtility.FromJson<TerrainData>(json);

        float[,] heightMap2D = new float[exportData.heightmapHeight, exportData.heightmapWidth];
        bool[,] holeMap2D = new bool[exportData.holemapHeight, exportData.holemapWidth];

        for (int y = 0; y < exportData.heightmapHeight; y++)
        {
            for (int x = 0; x < exportData.heightmapWidth; x++)
            {
                int index = y * exportData.heightmapWidth + x;
                heightMap2D[y, x] = exportData.heightMap[index];
            }
        }

        for (int y = 0; y < exportData.holemapHeight; y++)
        {
            for (int x = 0; x < exportData.holemapWidth; x++)
            {
                int index = y * exportData.holemapWidth + x;
                holeMap2D[y, x] = exportData.holeMap[index];
            }
        }

        terrainData.SetHeights(0, 0, heightMap2D);
        terrainData.SetHoles(0, 0, holeMap2D);
        brushDefoult = exportData.brushDefoultSave;
    }
    
    /// <summary>
    /// Resets terrain elevation and holes.
    /// </summary>
    public void ClearTerrain()
    {
        int heightRes = terrainData.heightmapResolution;
        int holeRes = terrainData.holesResolution;

        float[,] heightMap2D = terrainData.GetHeights(0, 0, heightRes, heightRes);
        bool[,] holeMap2D = terrainData.GetHoles(0, 0, holeRes, holeRes);
        for (int y = 0; y < heightRes; y++)
        {
            for (int x = 0; x < heightRes; x++)
            {
                heightMap2D[y, x] = 0f; // Reset height to 0
            }
        }
        for (int y = 0; y < holeRes; y++)
        {
            for (int x = 0; x < holeRes; x++)
            {
                holeMap2D[y, x] = true; // Reset holes to false
            }
        }
        terrainData.SetHeights(0, 0, heightMap2D);
        terrainData.SetHoles(0, 0, holeMap2D);
        terrainTexturePath = ""; // Clear texture path
        terrain.terrainData.terrainLayers = new TerrainLayer[] { }; // Clear terrain layers

    }

    /// <summary>
    /// Starts the process to change the terrain texture from a local file.
    /// </summary>
    /// <param name="imagePath">Full path to the image file.</param>
    public void ChangeTerrainTexture(string imagePath)
    {
        if (imagePath == null || imagePath == "")
        {
            imagePath = terrainTexturePath;
        }
        if (!File.Exists(imagePath))
            {
                Debug.Log("Texture file not found: " + imagePath);
                return;
            }
        terrainTexturePath = imagePath;
        StartCoroutine(LoadTerrainTexture(imagePath));
    }

    /// <summary>
    /// Loads a texture asynchronously from a local file and applies it to the terrain.
    /// </summary>
    /// <param name="path">Full path to the texture image.</param>
    /// <returns>Coroutine for Unity async operation.</returns>
    private IEnumerator LoadTerrainTexture(string path)
    {
        string uri = "file:///" + path.Replace("\\", "/");
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);

        TerrainLayer newLayer = new TerrainLayer();
        newLayer.diffuseTexture = texture;
        if (fillTexture.isOn)
        {
            newLayer.tileSize = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);
        }
        else
        {
            newLayer.tileSize = new Vector2(10, 10);
        }


        terrain.terrainData.terrainLayers = new TerrainLayer[] { newLayer };
    }
}