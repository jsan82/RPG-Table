using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaneHandler : MonoBehaviour
{
    private float brushSize { get; set; } // Size of bruh
    private float brushPower { get; set; } // Power per tick of Bruh
    private float brushTimer;
    private float brushLimit { get; set; } // Rate on hold of Bruh

    public Terrain terrain;
    private TerrainData terrainData;
    private int heightmapResolution;

    // Start is called before the first frame update
    void Start()
    {
        brushSize = 10.0f;
        brushPower = 0.01f;
        brushLimit = 0.01f;

        terrainData = terrain.terrainData;
        heightmapResolution = terrainData.heightmapResolution;
    }

    // Update is called once per frame
    void Update()
    {
        HandleElevation();
        HandleHole();
    }

    //pyknij terrain
    public void GiveTerrain(Terrain newTerrain)
    {
        terrain = newTerrain;
    }


    private void HandleElevation()
    {
        bool growHeld = Input.GetKey(KeyCode.J);
        bool lowerHeld = Input.GetKey(KeyCode.K);
        bool resetHeld = Input.GetKey(KeyCode.L);

        bool growDown = Input.GetKeyDown(KeyCode.J);
        bool lowerDown = Input.GetKeyDown(KeyCode.K);
        bool resetDown = Input.GetKeyDown(KeyCode.L);

        brushTimer += Time.deltaTime;

        if (growDown || (growHeld && brushTimer >= brushLimit)) //J
        {
            brushTimer = 0f;
            ModifyTerrainAtPosition(0);
        }
        else if (lowerDown || (lowerHeld && brushTimer >= brushLimit)) //K
        {
            brushTimer = 0f;
            ModifyTerrainAtPosition(1);
        }
        else if (resetDown || (resetHeld && brushTimer >= brushLimit)) //L
        {
            brushTimer = 0f;
            ModifyTerrainAtPosition(2);
        }


        if (!growHeld && !lowerHeld && !resetHeld)
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
        if (!Physics.Raycast(ray, out RaycastHit hit))
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


    private void ModifyTerrainAtPosition(int raiseTerrain)
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

                if (raiseTerrain == 0) map[z, x] += strength;
                else if (raiseTerrain == 1) map[z, x] -= strength;
                else if (raiseTerrain == 2) map[z, x] = 0;

                map[z, x] = Mathf.Clamp01(map[z, x]);
            }
        }

        terrainData.SetHeights(startX, startZ, map);
    }


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
}