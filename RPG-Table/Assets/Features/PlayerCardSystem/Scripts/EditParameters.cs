using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using System.IO;

public class EditParameters : MonoBehaviour, IUIBehavior
{

    public static EditParameters Instance { get; private set; }

    public GameObject objectPanel;
    public GameObject EditParametersPanel;
    
   private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;

        }
    }
    
    public void HandleUIClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                ObjectID objectID = result.gameObject.GetComponent<ObjectID>();
                if (objectID != null)
                {
                    EditParametersPanel.transform.Find("IDPlace").GetComponent<TMP_InputField>().text = objectID._id;
                    GameObject uiObject = ObjectID.GetObjectByID(objectID._id);
                if (uiObject != null)
                {
                    RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        // Pobierz szerokość i ustaw w polu tekstowym
                        float width = rectTransform.rect.width;
                        float height = rectTransform.rect.height;
                        EditParametersPanel.transform.Find("WidthPlace").GetComponent<TMP_InputField>().text = width.ToString("F2"); // Format do 2 miejsc po przecinku
                        EditParametersPanel.transform.Find("HeightPlace").GetComponent<TMP_InputField>().text = height.ToString("F2"); // Format do 2 miejsc po przecinku
                        switch (objectID._prefabName)
                        {
                            case "TextBlockPrefab":
                                EditParametersPanel.transform.Find("TextPlace").GetComponent<TMP_InputField>().text = 
                                    uiObject.GetComponent<TextMeshProUGUI>().text;
                                break;
                            case "Button":
                                EditParametersPanel.transform.Find("TextPlace").GetComponent<TMP_InputField>().text = 
                                    uiObject.GetComponentInChildren<TMP_Text>().text;
                                break;
                            case "InputField":
                                EditParametersPanel.transform.Find("TextPlace").GetComponent<TMP_InputField>().text = 
                                    uiObject.GetComponent<TMP_InputField>().text;
                                break;
                            
                        }
                    }
                }
                }
                
            }
        }
    }

    void updateObjectParameters()
    {
        string id = EditParametersPanel.transform.Find("IDPlace").GetComponent<TMP_InputField>().text;
        GameObject uiObject = ObjectID.GetObjectByID(id);
        if (uiObject != null)
        {
            RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Ustaw nową szerokość i wysokość
                float width = float.Parse(EditParametersPanel.transform.Find("WidthPlace").GetComponent<TMP_InputField>().text);
                float height = float.Parse(EditParametersPanel.transform.Find("HeightPlace").GetComponent<TMP_InputField>().text);
                rectTransform.sizeDelta = new Vector2(width, height);
            }
        }
    }
    
    public void onEditParametersButtonClick()
    {
        if(objectPanel.activeSelf)
        {
            objectPanel.SetActive(false);
            EditParametersPanel.SetActive(true);
        } else if(EditParametersPanel.activeSelf)
        {
            objectPanel.SetActive(true);
            EditParametersPanel.SetActive(false);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleUIClick();
        updateObjectParameters();
    }
}
