using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Dummiesman;

/// <summary>
/// Handles the interaction logic for props including spawning, dragging, rotating, scaling,
/// elevation, bloom toggle, and intensity adjustments.
/// </summary>
public class PropHandler : MonoBehaviour
{
    /// <summary>
    /// The object (prefab or loaded model) to spawn.
    /// </summary>
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

    /// <summary>
    /// Initializes default settings for prop manipulation controls.
    /// </summary>
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

        //LoadOBJFromPath("P A T H"); // comment if not testing
    }

    /// <summary>
    /// Per-frame update to handle all interactions.
    /// </summary>
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

    /// <summary>
    /// Sets the object to be spawned.
    /// </summary>
    /// <param name="newObject">The new object prefab or model.</param>
    public void SetObjectToSpawn(GameObject newObject)
    {
        objectToSpawn = newObject;
    }

    /// <summary>
    /// Loads a prefab from Resources by name and sets it as the object to spawn.
    /// </summary>
    /// <param name="prefabName">Name of the prefab in the Resources folder.</param>
    public void SetObjectToSpawnByName(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        if (prefab != null)
        {
            objectToSpawn = prefab;
        }
    }

    /// <summary>
    /// Instantiates the selected objectToSpawn at the mouse position when R is pressed.
    /// </summary>
    public void HandleSpawnProp()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 5f;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            GameObject spawned = Instantiate(objectToSpawn, worldPos, Quaternion.identity);
            spawned.SetActive(true);
        }
    }

    /// <summary>
    /// Rotates the selected prop using Q and E keys.
    /// </summary>
    private void HandleRotation()
    {
        if (selectedProp != null)
        {
            bool plusHeld = Input.GetKey(KeyCode.Q);
            bool minusHeld = Input.GetKey(KeyCode.E);

            bool plusDown = Input.GetKeyDown(KeyCode.Q);
            bool minusDown = Input.GetKeyDown(KeyCode.E);

            rotateTimer += Time.deltaTime;

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

    /// <summary>
    /// Elevates (moves up/down) the selected prop using Z and X keys.
    /// </summary>
    private void HandleElevation()
    {
        if (selectedProp != null)
        {
            bool plusHeld = Input.GetKey(KeyCode.Z);
            bool minusHeld = Input.GetKey(KeyCode.X);

            bool plusDown = Input.GetKeyDown(KeyCode.Z);
            bool minusDown = Input.GetKeyDown(KeyCode.X);

            elevateTimer += Time.deltaTime;

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

    /// <summary>
    /// Handles selecting and dragging a prop with the left mouse button.
    /// </summary>
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

    /// <summary>
    /// Scales the selected prop using C and V keys.
    /// </summary>
    private void HandleScale()
    {
        if (selectedProp != null)
        {
            bool plusHeld = Input.GetKey(KeyCode.C);
            bool minusHeld = Input.GetKey(KeyCode.V);

            bool plusDown = Input.GetKeyDown(KeyCode.C);
            bool minusDown = Input.GetKeyDown(KeyCode.V);

            scaleTimer += Time.deltaTime;

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

    /// <summary>
    /// Toggles the bloom effect on the selected prop using the T key.
    /// </summary>
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

    /// <summary>
    /// Adjusts bloom intensity of the selected prop using F and G keys.
    /// </summary>
    private void HandleBloomIntensity()
    {
        if (selectedProp != null)
        {
            bool plusHeld = Input.GetKey(KeyCode.F);
            bool minusHeld = Input.GetKey(KeyCode.G);

            bool plusDown = Input.GetKeyDown(KeyCode.F);
            bool minusDown = Input.GetKeyDown(KeyCode.G);
            
            colorTimer += Time.deltaTime;

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

    /// <summary>
    /// Loads an OBJ model from a file path and sets it as the object to spawn.
    /// </summary>
    /// <param name="filePath">Full path to the .obj file.</param>
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