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

    // Start is called before the first frame update
    void Start()
    {
        rotateLimit = 0.1f;
        rotatePower = 5.0f;
        elevateLimit = 0.1f;
        elevatePower = 0.1f;
        scaleLimit = 0.1f;
        scalePower = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        HandleDrag();
        HandleRotation();
        HandleElevation();
        HandleScale();

        if (Input.GetKeyDown(KeyCode.R))
            SpawnProp();
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

    public void SpawnProp()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5f;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos); //o tutej spawnuj

        Instantiate(objectToSpawn, worldPos, Quaternion.identity);
    }

    private void HandleRotation()
    {
        if (selectedProp != null)
        {
            bool leftHeld = Input.GetKey(KeyCode.Q);
            bool rightHeld = Input.GetKey(KeyCode.E);

            bool leftDown = Input.GetKeyDown(KeyCode.Q);
            bool rightDown = Input.GetKeyDown(KeyCode.E);

            rotateTimer += Time.deltaTime;

            //ratatuj
            if (leftDown || (leftHeld && rotateTimer >= rotateLimit)) //E
            {
                rotateTimer = 0f;
                selectedProp.OnRotate(Vector3.up, -rotatePower);
            }
            else if (rightDown || (rightHeld && rotateTimer >= rotateLimit)) //Q
            {
                rotateTimer = 0f;
                selectedProp.OnRotate(Vector3.up, rotatePower);
            }

            if (!leftHeld && !rightHeld)
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
            bool leftHeld = Input.GetKey(KeyCode.Z);
            bool rightHeld = Input.GetKey(KeyCode.X);

            bool leftDown = Input.GetKeyDown(KeyCode.Z);
            bool rightDown = Input.GetKeyDown(KeyCode.X);

            elevateTimer += Time.deltaTime;

            //elewatuj
            if (leftDown || (leftHeld && elevateTimer >= elevateLimit)) //X
            {
                elevateTimer = 0f;
                Vector3 newPosition = selectedProp.GetPosition() + new Vector3(0, elevatePower, 0);
                selectedProp.OnDrag(newPosition);
            }
            else if (rightDown || (rightHeld && elevateTimer >= elevateLimit)) //Z
            {
                elevateTimer = 0f;
                Vector3 newPosition = selectedProp.GetPosition() + new Vector3(0, -elevatePower, 0);
                selectedProp.OnDrag(newPosition);
            }

            if (!leftHeld && !rightHeld)
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
            bool leftHeld = Input.GetKey(KeyCode.C);
            bool rightHeld = Input.GetKey(KeyCode.V);

            bool leftDown = Input.GetKeyDown(KeyCode.C);
            bool rightDown = Input.GetKeyDown(KeyCode.V);

            scaleTimer += Time.deltaTime;

            //scaleuj
            if (leftDown || (leftHeld && scaleTimer >= scaleLimit)) //C
            {
                scaleTimer = 0f;
                Vector3 newScale = selectedProp.GetScale() + Vector3.one * scalePower;
                selectedProp.OnScale(newScale);
            }
            else if (rightDown || (rightHeld && scaleTimer >= scaleLimit)) //V
            {
                scaleTimer = 0f;
                Vector3 newScale = selectedProp.GetScale() - Vector3.one * scalePower;

                //cap
                newScale.x = Mathf.Max(0.1f, newScale.x);
                newScale.y = Mathf.Max(0.1f, newScale.y);
                newScale.z = Mathf.Max(0.1f, newScale.z);

                selectedProp.OnScale(newScale);
            }

            if (!leftHeld && !rightHeld)
            {
                scaleTimer = scaleLimit;
            }
        }
        else
        {
            scaleTimer = scaleLimit;
        }
    }
}