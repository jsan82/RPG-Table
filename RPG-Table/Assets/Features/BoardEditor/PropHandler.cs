using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class PropHandler : MonoBehaviour
{
    public GameObject objectToSpawn;

    private MovableProp selectedProp;
    private Vector3 dragOffset;
    private Plane dragPlane;

    private float rotatePower;
    private float rotateTimer;
    private float rotateLimit;

    private float elevatePower;
    private float elevateTimer;
    private float elevateLimit;

    private float scalePower;
    private float scaleTimer;
    private float scaleLimit;

    private float colorPower;
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

            Instantiate(objectToSpawn, worldPos, Quaternion.identity);
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
}