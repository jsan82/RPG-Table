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
    private string prefabName;
    
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
    
    void clearData()
    {
        EditParametersPanel.transform.Find("IDPlace").GetComponent<TMP_InputField>().text = "";
        EditParametersPanel.transform.Find("WidthPlace").GetComponent<TMP_InputField>().text = "";
        EditParametersPanel.transform.Find("HeightPlace").GetComponent<TMP_InputField>().text = "";
        EditParametersPanel.transform.Find("XPlace").GetComponent<TMP_InputField>().text = "";
        EditParametersPanel.transform.Find("YPlace").GetComponent<TMP_InputField>().text = "";
        EditParametersPanel.transform.Find("TransparencyPlace").GetComponent<TMP_InputField>().text = "";
        EditParametersPanel.transform.Find("IMGPlace").GetComponent<TMP_InputField>().text = "";
        EditParametersPanel.transform.Find("TextPanel/FontSizePlace").GetComponent<TMP_InputField>().text = "";
        EditParametersPanel.transform.Find("TextPanel/ColorPlace").GetComponent<TMP_InputField>().text = "";
        EditParametersPanel.transform.Find("TextPanel/TextPlace").GetComponent<TMP_InputField>().text = "";
        EditParametersPanel.transform.Find("TextPanel/Italic").GetComponent<Toggle>().isOn = false;
        EditParametersPanel.transform.Find("TextPanel/Bold").GetComponent<Toggle>().isOn = false;
        

    }
    
    public void HandleUIClick()
{
    if(Input.GetMouseButtonDown(0))
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        
        // Only process if we have results
        if(results.Count == 0) return;
        
        // Get the top-most result (first in the list is top-most in GraphicRaycaster)
        RaycastResult topResult = results[0];
        ObjectID objectID = topResult.gameObject.GetComponent<ObjectID>();
        
        if(objectID == null)
        {
            // Check if we clicked on a child of an ObjectID element
            objectID = topResult.gameObject.GetComponentInParent<ObjectID>();
            if(objectID == null) return;
        }

        ProcessObjectSelection(objectID);
    }
}

private void ProcessObjectSelection(ObjectID objectID)
{
    if(EditParametersPanel.transform.Find("IDPlace").GetComponent<TMP_InputField>().text != objectID._id)
        clearData();
        
    EditParametersPanel.transform.Find("IDPlace").GetComponent<TMP_InputField>().text = objectID._id;
    GameObject uiObject = ObjectID.GetObjectByID(objectID._id);
    prefabName = objectID._prefabName;
    
    if(uiObject == null) return;
    if (uiObject != null)
                {
                    if(objectID._prefabName == "TextBlockPrefab"){
                        EditParametersPanel.transform.Find("TransparencyPlace")?.gameObject.SetActive(false);
                        EditParametersPanel.transform.Find("Transparency")?.gameObject.SetActive(false);
                        EditParametersPanel.transform.Find("IMG")?.gameObject.SetActive(false);
                        EditParametersPanel.transform.Find("IMGPlace")?.gameObject.SetActive(false);
                    } else {
                        EditParametersPanel.transform.Find("TransparencyPlace")?.gameObject.SetActive(true);
                        EditParametersPanel.transform.Find("Transparency")?.gameObject.SetActive(true);
                        EditParametersPanel.transform.Find("IMG")?.gameObject.SetActive(true);
                        EditParametersPanel.transform.Find("IMGPlace")?.gameObject.SetActive(true);
                    }
                    
                    RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        // Download width and height
                        float width = rectTransform.rect.width;
                        float height = rectTransform.rect.height;
                        EditParametersPanel.transform.Find("WidthPlace").GetComponent<TMP_InputField>().text = width.ToString("F2"); 
                        EditParametersPanel.transform.Find("HeightPlace").GetComponent<TMP_InputField>().text = height.ToString("F2"); 
                        EditParametersPanel.transform.Find("XPlace").GetComponent<TMP_InputField>().text = rectTransform.anchoredPosition.x.ToString("F2"); 
                        EditParametersPanel.transform.Find("YPlace").GetComponent<TMP_InputField>().text = rectTransform.anchoredPosition.y.ToString("F2"); 
                        EditParametersPanel.transform.Find("TextPanel")?.gameObject.SetActive(true);
                        
                        switch (objectID._prefabName)
                        {
                            case "TextBlockPrefab":
                                EditParametersPanel.transform.Find("TextPanel/TextPlace").GetComponent<TMP_InputField>().text = 
                                    uiObject.GetComponent<TextMeshProUGUI>().text;
                                EditParametersPanel.transform.Find("TextPanel/FontSizePlace").GetComponent<TMP_InputField>().text =
                                    uiObject.GetComponent<TextMeshProUGUI>().fontSize.ToString("F0");
                                EditParametersPanel.transform.Find("TextPanel/ColorPlace").GetComponent<TMP_InputField>().text =
                                    ColorUtility.ToHtmlStringRGB(uiObject.GetComponent<TextMeshProUGUI>().color);
                                break;
                            case "Button":
                                EditParametersPanel.transform.Find("TextPanel/TextPlace").GetComponent<TMP_InputField>().text = 
                                    uiObject.GetComponentInChildren<TMP_Text>().text;
                                EditParametersPanel.transform.Find("TextPanel/FontSizePlace").GetComponent<TMP_InputField>().text =
                                    uiObject.GetComponentInChildren<TMP_Text>().fontSize.ToString("F0");  
                                EditParametersPanel.transform.Find("TextPanel/ColorPlace").GetComponent<TMP_InputField>().text =
                                    ColorUtility.ToHtmlStringRGB(uiObject.GetComponentInChildren<TMP_Text>().color);

                                break;
                            case "InputField":
                                EditParametersPanel.transform.Find("TextPanel/TextPlace").GetComponent<TMP_InputField>().text = 
                                    uiObject.GetComponent<TMP_InputField>().text;
                                EditParametersPanel.transform.Find("TextPanel/FontSizePlace").GetComponent<TMP_InputField>().text = 
                                    uiObject.GetComponent<TMP_InputField>().pointSize.ToString("F0");
                                EditParametersPanel.transform.Find("TextPanel/ColorPlace").GetComponent<TMP_InputField>().text = 
                                    ColorUtility.ToHtmlStringRGB(uiObject.GetComponent<TMP_InputField>().textComponent.color);

                                break;
                            default:
                                EditParametersPanel.transform.Find("TextPanel")?.gameObject.SetActive(false);
                                break;
                        }
                    }
                    if(uiObject.GetComponent<Image>()!= null){
                        double alpha;
                        alpha = uiObject.GetComponent<Image>().color.a;
                        alpha = alpha * 100;
                        EditParametersPanel.transform.Find("TransparencyPlace").GetComponent<TMP_InputField>().text = (alpha).ToString("F2");
                        EditParametersPanel.transform.Find("IMGPlace").GetComponent<TMP_InputField>().text = uiObject.GetComponent<Image>().sprite.name;
                    } else if(uiObject.GetComponent<RawImage>()!=null){
                        double alpha;
                        alpha = uiObject.GetComponent<RawImage>().color.a;
                        alpha = alpha * 100;
                        EditParametersPanel.transform.Find("TransparencyPlace").GetComponent<TMP_InputField>().text = (alpha).ToString("F2");
                        //EditParametersPanel.transform.Find("IMGPlace").GetComponent<TMP_InputField>().text = uiObject.GetComponent<RawImage>().sprite.name;
            }
        }
    }
        

    void updateObjectParameters()
    {
        string id = EditParametersPanel.transform.Find("IDPlace").GetComponent<TMP_InputField>().text;
        GameObject uiObject = ObjectID.GetObjectByID(id);
        if (uiObject != null)
        {
            ObjectID objectid = uiObject.GetComponent<ObjectID>();
            RectTransform rectTransform = uiObject.GetComponent<RectTransform>();

            if(SmartDragHandler.isDragging)
            {
                EditParametersPanel.transform.Find("XPlace").GetComponent<TMP_InputField>().text = rectTransform.anchoredPosition.x.ToString("F2"); 
            EditParametersPanel.transform.Find("YPlace").GetComponent<TMP_InputField>().text = rectTransform.anchoredPosition.y.ToString("F2"); 
            }
            

            rectTransform.anchoredPosition = new Vector2(
                float.Parse(EditParametersPanel.transform.Find("XPlace").GetComponent<TMP_InputField>().text),
                 float.Parse(EditParametersPanel.transform.Find("YPlace").GetComponent<TMP_InputField>().text));
            //update position
            

            //update width and height
            if (rectTransform != null)
            {
               
                float width = float.Parse((EditParametersPanel.transform.Find("WidthPlace").GetComponent<TMP_InputField>().text=="") ? "0" : EditParametersPanel.transform.Find("WidthPlace").GetComponent<TMP_InputField>().text);
                float height = float.Parse((EditParametersPanel.transform.Find("HeightPlace").GetComponent<TMP_InputField>().text == "") ? "0" : EditParametersPanel.transform.Find("HeightPlace").GetComponent<TMP_InputField>().text);
               
                rectTransform.sizeDelta = new Vector2(width, height);
            }

            //update transparency
            if(uiObject.GetComponent<Image>()!= null){
                if (float.TryParse(EditParametersPanel.transform.Find("TransparencyPlace").GetComponent<TMP_InputField>().text, out float alphaValue))
                {
                    alphaValue = alphaValue/100;
                    Image image = uiObject.GetComponent<Image>();
                    image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Clamp01(alphaValue));
                }
                if(uiObject.GetComponent<Image>().sprite.name != EditParametersPanel.transform.Find("IMGPlace").GetComponent<TMP_InputField>().text && !EditParametersPanel.transform.Find("IMGPlace").GetComponent<TMP_InputField>().isFocused)
                {   
                    CardAreaSaver cardAreaSaver = new CardAreaSaver();
                    cardAreaSaver.SetImage(uiObject, EditParametersPanel.transform.Find("IMGPlace").GetComponent<TMP_InputField>().text);
                }
             } 
            //else if (float.TryParse(EditParametersPanel.transform.Find("TransparencyPlace").GetComponent<TMP_InputField>().text, out float alphaValue))
            // {
            //     alphaValue = alphaValue/100;
            //     RawImage image = uiObject.GetComponent<RawImage>();
            //     image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Clamp01(alphaValue));
            // }
            Color newColor;
            //update text
            if(uiObject.GetComponent<TextMeshProUGUI>() != null)
            {
                uiObject.GetComponent<TextMeshProUGUI>().text = EditParametersPanel.transform.Find("TextPanel/TextPlace").GetComponent<TMP_InputField>().text;
                uiObject.GetComponent<TextMeshProUGUI>().fontSize = float.Parse(EditParametersPanel.transform.Find("TextPanel/FontSizePlace").GetComponent<TMP_InputField>().text);

                //uploading sytle
                FontStyles style = FontStyles.Normal;
                if (EditParametersPanel.transform.Find("TextPanel/Italic").GetComponent<Toggle>().isOn)
                    style |= FontStyles.Italic;
                if (EditParametersPanel.transform.Find("TextPanel/Bold").GetComponent<Toggle>().isOn)
                    style |= FontStyles.Bold;
                uiObject.GetComponent<TextMeshProUGUI>().fontStyle = style;
               
               
                ColorUtility.TryParseHtmlString("#" + EditParametersPanel.transform.Find("TextPanel/ColorPlace").GetComponent<TMP_InputField>().text, out newColor);
                uiObject.GetComponent<TextMeshProUGUI>().color = newColor;
            }else if (uiObject.GetComponent<TMP_InputField>() != null)
            {
                //uiObject.GetComponent<TMP_InputField>().onValueChanged.AddListener(newVal => EditParametersPanel.transform.Find("TextPanel/TextPlace").GetComponent<TMP_InputField>().text = newVal);
                //EditParametersPanel.transform.Find("TextPanel/TextPlace").GetComponent<TMP_InputField>().onValueChanged.AddListener(newVal => uiObject.GetComponent<TMP_InputField>().text = newVal);
                
                if (!uiObject.GetComponent<TMP_InputField>().isFocused)
                {
                    uiObject.GetComponent<TMP_InputField>().text = EditParametersPanel.transform.Find("TextPanel/TextPlace").GetComponent<TMP_InputField>().text;
                } else {
                    EditParametersPanel.transform.Find("TextPanel/TextPlace").GetComponent<TMP_InputField>().text = uiObject.GetComponent<TMP_InputField>().text;
                }
                uiObject.GetComponent<TMP_InputField>().pointSize = float.Parse(EditParametersPanel.transform.Find("TextPanel/FontSizePlace").GetComponent<TMP_InputField>().text);

                FontStyles style = FontStyles.Normal;
                if (EditParametersPanel.transform.Find("TextPanel/Italic").GetComponent<Toggle>().isOn)
                    style |= FontStyles.Italic;
                if (EditParametersPanel.transform.Find("TextPanel/Bold").GetComponent<Toggle>().isOn)
                    style |= FontStyles.Bold;
                uiObject.GetComponent<TMP_InputField>().textComponent.fontStyle = style;

                ColorUtility.TryParseHtmlString("#" + EditParametersPanel.transform.Find("TextPanel/ColorPlace").GetComponent<TMP_InputField>().text, out newColor);
                uiObject.GetComponentInChildren<TMP_InputField>().textComponent.color = newColor;



            } else if (objectid._prefabName == "Button")
            {
                uiObject.GetComponentInChildren<TMP_Text>().text = EditParametersPanel.transform.Find("TextPanel/TextPlace").GetComponent<TMP_InputField>().text;
                
                uiObject.GetComponentInChildren<TMP_Text>().fontSize = float.Parse(EditParametersPanel.transform.Find("TextPanel/FontSizePlace").GetComponent<TMP_InputField>().text);
                ColorUtility.TryParseHtmlString("#" + EditParametersPanel.transform.Find("TextPanel/ColorPlace").GetComponent<TMP_InputField>().text, out newColor);
                
                uiObject.GetComponentInChildren<TMP_Text>().color = newColor;
                FontStyles style = FontStyles.Normal;
                if (EditParametersPanel.transform.Find("TextPanel/Italic").GetComponent<Toggle>().isOn)
                    style |= FontStyles.Italic;
                if (EditParametersPanel.transform.Find("TextPanel/Bold").GetComponent<Toggle>().isOn)
                    style |= FontStyles.Bold;
                uiObject.GetComponentInChildren<TMP_Text>().fontStyle = style;
                ColorUtility.TryParseHtmlString("#" + EditParametersPanel.transform.Find("TextPanel/ColorPlace").GetComponent<TMP_InputField>().text, out newColor);
                uiObject.GetComponentInChildren<TMP_Text>().color = newColor;
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

    public void savePrefab(){
        savingCustomObjects savingCustomObjects = new savingCustomObjects();
        savingCustomObjects.prefab = prefabName;
        if(savingCustomObjects.prefab != "Image")
        {
            //savingCustomObjects.imageName = EditParametersPanel.transform.Find("IMGPlace").GetComponent<TMP_InputField>().text;
            savingCustomObjects.isBold = EditParametersPanel.transform.Find("TextPanel/Bold").GetComponent<Toggle>().isOn;
            savingCustomObjects.isItalic = EditParametersPanel.transform.Find("TextPanel/Italic").GetComponent<Toggle>().isOn;
            savingCustomObjects.fontSize = EditParametersPanel.transform.Find("TextPanel/FontSizePlace").GetComponent<TMP_InputField>().text;
            savingCustomObjects.fontColor = EditParametersPanel.transform.Find("TextPanel/ColorPlace").GetComponent<TMP_InputField>().text;
            savingCustomObjects.text = "ENTER TEXT";
        }
        if(savingCustomObjects.prefab != "TextBlockPrefab")
        {
            savingCustomObjects.imageName = EditParametersPanel.transform.Find("IMGPlace").GetComponent<TMP_InputField>().text;
            savingCustomObjects.transparency = EditParametersPanel.transform.Find("TransparencyPlace").GetComponent<TMP_InputField>().text;
        }
        savingCustomObjects.Width = EditParametersPanel.transform.Find("WidthPlace").GetComponent<TMP_InputField>().text;
        savingCustomObjects.Height = EditParametersPanel.transform.Find("HeightPlace").GetComponent<TMP_InputField>().text;
        ObjectPlacementSystem ops = FindObjectOfType<ObjectPlacementSystem>();
        if (ops != null)
        {
            ops.onCustomPrefabSaveButtonClick(savingCustomObjects);
        }
        else
        {
            Debug.LogError("ObjectPlacementSystem not found in the scene!");
        }
    }

    public void destroyObject(){
        CardAreaSaver cardAreaSaver = new CardAreaSaver();
        cardAreaSaver.deleteObject(EditParametersPanel.transform.Find("IDPlace").GetComponent<TMP_InputField>().text);
    }

}

