using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;
<<<<<<< HEAD
using UnityEngine.UI;
=======
>>>>>>> e91458933ac7029391988ba4b9ffac29c4b2ced8

public class PlaneHandler : MonoBehaviour
{
    public float brushSize { get; set; } // Size of bruh
    private float brushPower { get; set; } // Power per tick of Bruh
    private float brushTimer;
    private float brushLimit { get; set; } // Rate on hold of Bruh
    private float brushDefoult;
    public string terrainTexturePath;
    public Toggle fillTexture;
    public Toggle editModeOn;
    public Slider brushSizeSlider;

    public Terrain terrain;
    private UnityEngine.TerrainData terrainData;
    private int heightmapResolution;

    // Start is called before the first frame update
    void Start()
    {
        brushSize = 10.0f;
        brushPower = 0.01f;
        brushLimit = 0.01f;
        brushDefoult = 0.0f;

        terrainData = terrain.terrainData;
        heightmapResolution = terrainData.heightmapResolution;
<<<<<<< HEAD
        
=======
>>>>>>> e91458933ac7029391988ba4b9ffac29c4b2ced8

        //ChangeTerrainTexture("[P A T H]"); //comment if not testing
    }

    // Update is called once per frame
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
<<<<<<< HEAD
        ChangeBrushSize();
/*
                if (Input.GetKeyDown(KeyCode.T))//comment if not testing
                {
                    ExportTerrain("P A T H");
                }
                if (Input.GetKeyDown(KeyCode.G))//comment if not testing
                {
                    ImportTerrain("P A T H");
                }*/
    }

    public void ChangeBrushSize()
    {
        brushSize = brushSizeSlider.value;
=======
/*
        if (Input.GetKeyDown(KeyCode.T))//comment if not testing
        {
            ExportTerrain("P A T H");
        }
        if (Input.GetKeyDown(KeyCode.G))//comment if not testing
        {
            ImportTerrain("P A T H");
        }*/
>>>>>>> e91458933ac7029391988ba4b9ffac29c4b2ced8
    }

    //pyknij terrain
    public void GiveTerrain(Terrain newTerrain)
    {
        terrain = newTerrain;
    }


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

    //na kopiec kreta i kilimanjaro
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

    //elevate all
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

        //new def
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

    //kto dolki kopie ten sam w nie wpada
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

    public void ChangeTerrainTexture(string imagePath)
    {
<<<<<<< HEAD
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
=======
        if (!File.Exists(imagePath))
        {
            Debug.LogError("Texture file not found: " + imagePath);
            return;
        }

>>>>>>> e91458933ac7029391988ba4b9ffac29c4b2ced8
        StartCoroutine(LoadTerrainTexture(imagePath));
    }

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
<<<<<<< HEAD
        if (fillTexture.isOn)
        {
            newLayer.tileSize = new Vector2(terrain.terrainData.size.x, terrain.terrainData.size.z);
        }
        else
        {
            newLayer.tileSize = new Vector2(10, 10);
        }


        terrain.terrainData.terrainLayers = new TerrainLayer[] { newLayer };
=======
        newLayer.tileSize = new Vector2(10, 10);

        TerrainLayer[] layers = new TerrainLayer[1];
        layers[0] = newLayer;
        terrain.terrainData.terrainLayers = layers;
>>>>>>> e91458933ac7029391988ba4b9ffac29c4b2ced8
    }
}