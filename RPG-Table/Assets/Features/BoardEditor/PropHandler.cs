using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Dummiesman;

public class PropHandler : MonoBehaviour
{
    public GameObject objectToSpawn;

    private MovableProp selectedProp;
    private Vector3 dragOffset;
    private Plane dragPlane;

    private float rotatePower { get; set; }
    private float rotateTimer;
    private float rotateLimit;

    private float elevatePower { get; set; }
    private float elevateTimer;
    private float elevateLimit;

    private float scalePower { get; set; }
    private float scaleTimer;
    private float scaleLimit;

    private float colorPower { get; set; }
    private float colorTimer;
    private float colorLimit;

    // Start is called before the first frame update
    void Start()
    {
        rotateLimit = 0.1f;
        rotatePower = 5.0f;

        elevateLimit = 0.1f;
        elevatePower = 0.1f;

        scaleLimit = 0.1f;
        scalePower = 0.1f;

        colorLimit = 0.1f;
        colorPower = 0.1f;

        LoadOBJFromPath("C:\\Users\\huber\\Desktop\\convtest\\uploads_files_4162193_OldBook001_tex\\magic_staff.obj"); // comment if not testing
    }

    // Update is called once per frame
    void Update()
    {
        HandleDrag();
        HandleRotation();
        HandleElevation();
        HandleScale();
        HandleBloomToggle();
        HandleBloomIntensity();
        HandleSpawnProp();
    }

    //pyknij propa via referance
    public void SetObjectToSpawn(GameObject newObject)
    {
        objectToSpawn = newObject;
    }

    //pyknij propa via nazwa
    public void SetObjectToSpawnByName(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        if (prefab != null)
        {
            objectToSpawn = prefab;
        }
    }

    public void HandleSpawnProp()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 5f;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos); //o tutej spawnuj

            GameObject spawned = Instantiate(objectToSpawn, worldPos, Quaternion.identity);
            spawned.SetActive(true);
        }
    }

    private void HandleRotation()
    {
        if (selectedProp != null)
        {
            bool plusHeld = Input.GetKey(KeyCode.Q);
            bool minusHeld = Input.GetKey(KeyCode.E);

            bool plusDown = Input.GetKeyDown(KeyCode.Q);
            bool minusDown = Input.GetKeyDown(KeyCode.E);

            rotateTimer += Time.deltaTime;

            //ratatuj
            if (plusDown || (plusHeld && rotateTimer >= rotateLimit)) //E
            {
                rotateTimer = 0f;
                selectedProp.OnRotate(Vector3.up, -rotatePower);
            }
            else if (minusDown || (minusHeld && rotateTimer >= rotateLimit)) //Q
            {
                rotateTimer = 0f;
                selectedProp.OnRotate(Vector3.up, rotatePower);
            }

            if (!plusHeld && !minusHeld)
            {
                rotateTimer = rotateLimit;
            }
        }
        else
        {
            rotateTimer = rotateLimit;
        }
    }
    private void HandleElevation()
    {
        if (selectedProp != null)
        {
            bool plusHeld = Input.GetKey(KeyCode.Z);
            bool minusHeld = Input.GetKey(KeyCode.X);

            bool plusDown = Input.GetKeyDown(KeyCode.Z);
            bool minusDown = Input.GetKeyDown(KeyCode.X);

            elevateTimer += Time.deltaTime;

            //elewatuj
            if (plusDown || (plusHeld && elevateTimer >= elevateLimit)) //X
            {
                elevateTimer = 0f;
                Vector3 newPosition = selectedProp.GetPosition() + new Vector3(0, elevatePower, 0);
                selectedProp.OnDrag(newPosition);
            }
            else if (minusDown || (minusHeld && elevateTimer >= elevateLimit)) //Z
            {
                elevateTimer = 0f;
                Vector3 newPosition = selectedProp.GetPosition() + new Vector3(0, -elevatePower, 0);
                selectedProp.OnDrag(newPosition);
            }

            if (!plusHeld && !minusHeld)
            {
                elevateTimer = elevateLimit;
            }
        }
        else
        {
            elevateTimer = elevateLimit;
        }
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0)) //lmb
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                MovableProp prop = hit.collider.GetComponent<MovableProp>();
                if (prop != null)
                {
                    selectedProp = prop;

                    dragPlane = new Plane(Vector3.up, hit.point);
                    dragOffset = hit.point - selectedProp.GetPosition();
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            selectedProp = null;
        }

        //draguj
        if (selectedProp != null && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                Vector3 newPosition = hitPoint - dragOffset;
                newPosition.y = selectedProp.GetPosition().y;
                selectedProp.OnDrag(newPosition);
            }
        }
    }

    private void HandleScale()
    {
        if (selectedProp != null)
        {
            bool plusHeld = Input.GetKey(KeyCode.C);
            bool minusHeld = Input.GetKey(KeyCode.V);

            bool plusDown = Input.GetKeyDown(KeyCode.C);
            bool minusDown = Input.GetKeyDown(KeyCode.V);

            scaleTimer += Time.deltaTime;

            //scaleuj
            if (plusDown || (plusHeld && scaleTimer >= scaleLimit)) //C
            {
                scaleTimer = 0f;
                Vector3 newScale = selectedProp.GetScale() + Vector3.one * scalePower;
                selectedProp.OnScale(newScale);
            }
            else if (minusDown || (minusHeld && scaleTimer >= scaleLimit)) //V
            {
                scaleTimer = 0f;
                Vector3 newScale = selectedProp.GetScale() - Vector3.one * scalePower;

                //cap
                newScale.x = Mathf.Max(0.1f, newScale.x);
                newScale.y = Mathf.Max(0.1f, newScale.y);
                newScale.z = Mathf.Max(0.1f, newScale.z);

                selectedProp.OnScale(newScale);
            }

            if (!plusHeld && !minusHeld)
            {
                scaleTimer = scaleLimit;
            }
        }
        else
        {
            scaleTimer = scaleLimit;
        }
    }

    private void HandleBloomToggle()
    {
        if (selectedProp != null) 
        {
            if (Input.GetKeyDown(KeyCode.T)) //T
            {
                selectedProp.ToggleBloom();
            }
            
        }
    }

    private void HandleBloomIntensity()
    {
        if (selectedProp != null)
        {
            bool plusHeld = Input.GetKey(KeyCode.F);
            bool minusHeld = Input.GetKey(KeyCode.G);

            bool plusDown = Input.GetKeyDown(KeyCode.F);
            bool minusDown = Input.GetKeyDown(KeyCode.G);
            
            colorTimer += Time.deltaTime;

            //coloruj
            if (plusDown || (plusHeld && colorTimer >= colorLimit)) //F
            {
                colorTimer = 0f;
                selectedProp.SetIntensity(colorPower);
            }
            else if (minusDown || (minusHeld && colorTimer >= colorLimit)) //G
            {
                colorTimer = 0f;
                selectedProp.SetIntensity(-colorPower);
            }

            if (!plusHeld && !minusHeld)
            {
                colorTimer = colorLimit;
            }
        }
        else
        {
            colorTimer = colorLimit;
        }

    }
    ///*
    public void LoadOBJFromPath(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        GameObject obj = new OBJLoader().Load(filePath);
        obj.name = Path.GetFileNameWithoutExtension(filePath);

        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;

        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        var renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            bounds = renderers[0].bounds;
            foreach (var rend in renderers)
            {
                bounds.Encapsulate(rend.bounds);
            }
        }

        BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
        boxCollider.center = obj.transform.InverseTransformPoint(bounds.center);
        boxCollider.size = bounds.size;

        obj.AddComponent<MovableProp>();
        obj.SetActive(false);
        objectToSpawn = obj;
    }
}