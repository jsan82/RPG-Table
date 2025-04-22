using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropSpwner : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        rotateLimit = 0.1f;
        rotatePower = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        HandleDrag();
        HandleRotation();
        HandleElevation();

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
            bool rotateLeftHeld = Input.GetKey(KeyCode.Q);
            bool rotateRightHeld = Input.GetKey(KeyCode.E);

            bool rotateLeftDown = Input.GetKeyDown(KeyCode.Q);
            bool rotateRightDown = Input.GetKeyDown(KeyCode.E);
            
            rotateTimer += Time.deltaTime;

            //ratatuj
            if (rotateLeftDown || (rotateLeftHeld && rotateTimer >= rotateLimit)) //E
            {
                rotateTimer = 0f;
                selectedProp.OnRotate(Vector3.up, -rotatePower);
            }
            else if (rotateRightDown || (rotateRightHeld && rotateTimer >= rotateLimit)) //Q
            {
                rotateTimer = 0f;
                selectedProp.OnRotate(Vector3.up, rotatePower);
            }

            if (!rotateLeftHeld && !rotateRightHeld)
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
            bool elevateLeftHeld = Input.GetKey(KeyCode.Z);
            bool elevateRightHeld = Input.GetKey(KeyCode.X);

            bool elevateLeftDown = Input.GetKeyDown(KeyCode.Z);
            bool elevateRightDown = Input.GetKeyDown(KeyCode.X);

            elevateTimer += Time.deltaTime;

            //elewatuj
            if (elevateLeftDown || (elevateLeftHeld && elevateTimer >= elevateLimit)) //X
            {
                elevateTimer = 0f;
                Vector3 newPosition = selectedProp.transform.position + new Vector3(0, elevatePower, 0);
                selectedProp.OnDrag(newPosition);
            }
            else if (elevateRightDown || (elevateRightHeld && elevateTimer >= elevateLimit)) //Z
            {
                elevateTimer = 0f;
                Vector3 newPosition = selectedProp.transform.position + new Vector3(0, -elevatePower, 0);
                selectedProp.OnDrag(newPosition);
            }

            if (!elevateLeftHeld && !elevateRightHeld)
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
                    dragOffset = hit.point - selectedProp.transform.position;
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
                selectedProp.OnDrag(hitPoint - dragOffset);
            }
        }
    }
}
