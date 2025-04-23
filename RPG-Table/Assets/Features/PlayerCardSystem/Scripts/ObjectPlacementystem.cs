using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectPlacementSystem : MonoBehaviour
{
    [System.Serializable]
    public class PlaceableObject
    {
        public string name;
        public GameObject prefab;
        public Sprite icon;
    }

    public List<PlaceableObject> availableObjects;
    public Transform objectsPanel;
    public GameObject objectButtonPrefab;
    public GameObject confirmationDialog;
    public InputField idInputField;
    
    private GameObject selectedPrefab;
    private GameObject pendingObject;

    void Start()
    {
        
    Debug.Log("Rozpoczynam inicjalizację...");
    
    if (availableObjects == null)
        Debug.LogError("availableObjects jest null!");
    else if (availableObjects.Count == 0)
        Debug.LogError("availableObjects jest pusty!");

    if (objectsPanel == null)
        Debug.LogError("objectsPanel nie jest przypisany!");

    if (objectButtonPrefab == null)
        Debug.LogError("objectButtonPrefab nie jest przypisany!");


        foreach (var obj in availableObjects)
        {
            GameObject button = Instantiate(objectButtonPrefab, objectsPanel);
            Text buttonText = button.GetComponentInChildren<Text>();
            buttonText.text = obj.name;
            //Image buttonImage = button.GetComponentInChildren<Image>();
            //buttonImage.sprite = obj.icon;
            //button.GetComponent<Image>().sprite = obj.icon;
            button.GetComponent<Button>().onClick.AddListener(() => SelectObject(obj.prefab));
        }
    }

    void Update()
{
    if (pendingObject != null)
    {
        // Aktualizacja pozycji pendingObject
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        pendingObject.transform.position = mousePos;

        // Sprawdź czy to lewy przycisk myszy i czy nie klikamy na UI
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && 
                hit.collider == null) // Dodatkowe sprawdzenie czy nie klikamy na inny obiekt
            {
                confirmationDialog.SetActive(true);
            }
        }
    }
}

    public void SelectObject(GameObject prefab)
    {
        selectedPrefab = prefab;
        if (pendingObject != null) Destroy(pendingObject);
        pendingObject = Instantiate(prefab);
        
        foreach (var collider in pendingObject.GetComponents<Collider>())
            collider.enabled = false;
        
        foreach (var behaviour in pendingObject.GetComponents<MonoBehaviour>())
            behaviour.enabled = false;
    }

    public void ConfirmPlacement()
    {
        if (pendingObject == null) return;
        
        foreach (var collider in pendingObject.GetComponents<Collider>())
            collider.enabled = true;
        
        foreach (var behaviour in pendingObject.GetComponents<MonoBehaviour>())
            behaviour.enabled = true;
        
        if (!string.IsNullOrEmpty(idInputField.text))
        {
            ObjectID objId = pendingObject.GetComponent<ObjectID>();
            if (objId == null) objId = pendingObject.AddComponent<ObjectID>();
            objId.SetID(idInputField.text);
        }
        
        pendingObject = null;
        selectedPrefab = null;
        idInputField.text = "";
        confirmationDialog.SetActive(false);
    }

    public void CancelPlacement()
    {
        if (pendingObject != null) Destroy(pendingObject);
        pendingObject = null;
        selectedPrefab = null;
        idInputField.text = "";
        confirmationDialog.SetActive(false);
    }
}