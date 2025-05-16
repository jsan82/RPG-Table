using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.IO;

public class PlaneHandler : MonoBehaviour
{
    private float brushSize { get; set; } // Size of bruh
    private float brushPower { get; set; } // Power per tick of Bruh
    private float brushTimer;
    private float brushLimit { get; set; } // Rate on hold of Bruh
    private float brushDefoult;

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
    }

    // Update is called once per frame
    void Update()
    {
        HandleElevation();
        HandleAllElevation();
        HandleHole();
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
        var data = CalcualteTerrainData();
        if (data == null) return;
        var (modifRadius, startX, startZ, width, height) = data.Value;

        bool[,] mapHole = terrainData.GetHoles(startX, startZ, width, height);
        float[,] mapHeight = terrainData.GetHeights(startX, startZ, width, height);

        TerrainData exportData = new TerrainData
        {
            width = width,
            height = height,
            heightMap = mapHeight,
            holeMap = mapHole
        };

        string jsonString = JsonUtility.ToJson(exportData);
        File.WriteAllText(outputPath, jsonString);
    }

    public void ImportTerrain(string inputPath)
    {
        if (!File.Exists(inputPath))
            return;

        string jsonString = File.ReadAllText(inputPath);
        TerrainData exportData = JsonUtility.FromJson<TerrainData>(jsonString);

        int width = exportData.width;
        int height = exportData.height;
        var terrainData = terrain.terrainData;

        terrainData.heightmapResolution = Mathf.Max(exportData.width, exportData.height);
        terrainData.SetHeights(0, 0, exportData.heightMap);
        terrainData.SetHoles(0, 0, exportData.holeMap);
    }
}