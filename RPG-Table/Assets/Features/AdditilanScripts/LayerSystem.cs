using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the layer system for 2D and 3D game modes
/// </summary>
/// <remarks>
/// Handles:
/// - Layer toggling between Token/Prop/Map layers
/// - 2D/3D mode switching
/// - Raycast target management for 2D
/// - Collider management for 3D
/// - Camera control activation
/// - UI element visibility
/// </remarks>
public class LayerSystem : MonoBehaviour
{
    public Toggle tokenLayerToggle;
    public Toggle propLayerToggle;
    public Toggle mapLayerToggle;
    public GameObject token2DLayer;
    public GameObject prop2DLayer;
    public GameObject map2DLayer;

    public GameObject GameManager;
    public GameObject panel;
    public GameObject drawingLayer;
    public GameObject token3DLayer;
    public GameObject prop3DLayer;
    public GameObject map3DLayer;
    public GameObject camera;
    public GameObject table;

    public GameObject drawingButton;
    public GameObject HammerButton;


    public string _GAME_MODE;

    public GameObject _CURRENT_LAYER;
    // Start is called before the first frame update\

    private bool isChangingToggles = false;


    /// <summary>
    /// Initializes toggle listeners
    /// </summary>
    void Start()
    {
        // Set up the toggle listeners
        tokenLayerToggle.onValueChanged.AddListener(OnTokenToggleChanged);
        propLayerToggle.onValueChanged.AddListener(OnPropToggleChanged);
        mapLayerToggle.onValueChanged.AddListener(OnMapToggleChanged);
    }

    /// <summary>
    /// Handles layer management and mode switching
    /// </summary>
    /// <remarks>
    /// Manages:
    /// - 2D/3D mode visual states
    /// - Raycast target enabling/disabling
    /// - Collider enabling/disabling
    /// - UI element visibility
    /// - Current layer tracking
    /// </remarks>
    void Update()
    {
        if (_GAME_MODE == "2D")
        {
            camera.GetComponent<FreeCameraController>().enabled = false;
            token2DLayer.SetActive(true);
            prop2DLayer.SetActive(true);
            map2DLayer.SetActive(true);
            token3DLayer.SetActive(false);
            prop3DLayer.SetActive(false);
            table.SetActive(true);
            drawingLayer.SetActive(true);
            HammerButton.SetActive(false);
            drawingButton.SetActive(true);
            //map3DLayer.SetActive(false);
            // Ensure only one layer is active at a time
            if (!GameManager.GetComponent<MapBrushDrawer>().isDrawing)
            {
                drawingLayer.GetComponent<RawImage>().raycastTarget = false;
                if (tokenLayerToggle.isOn)
                {
                    _CURRENT_LAYER = token2DLayer;
                    foreach (Transform child in token2DLayer.transform)
                    {
                        if (child.gameObject.GetComponent<RawImage>() != null)
                        {
                            child.gameObject.GetComponent<RawImage>().raycastTarget = true;
                        }
                    }
                    foreach (Transform child in prop2DLayer.transform)
                    {
                        if (child.gameObject.GetComponent<RawImage>() != null)
                        {
                            child.gameObject.GetComponent<RawImage>().raycastTarget = false;
                        }
                    }
                    foreach (Transform child in map2DLayer.transform)
                    {
                        if (child.gameObject.GetComponent<RawImage>() != null)
                        {
                            child.gameObject.GetComponent<RawImage>().raycastTarget = false;
                        }
                    }
                    token2DLayer.GetComponent<Image>().raycastTarget = true;
                    prop2DLayer.GetComponent<Image>().raycastTarget = false;
                    map2DLayer.GetComponent<Image>().raycastTarget = false;

                }
                else if (propLayerToggle.isOn)
                {
                    _CURRENT_LAYER = prop2DLayer;
                    foreach (Transform child in prop2DLayer.transform)
                    {
                        if (child.gameObject.GetComponent<RawImage>() != null)
                        {
                            child.gameObject.GetComponent<RawImage>().raycastTarget = true;
                        }
                    }
                    foreach (Transform child in prop2DLayer.transform)
                    {
                        if (child.gameObject.GetComponent<RawImage>() != null)
                        {
                            child.gameObject.GetComponent<RawImage>().raycastTarget = true;
                        }
                    }
                    foreach (Transform child in map2DLayer.transform)
                    {
                        if (child.gameObject.GetComponent<RawImage>() != null)
                        {
                            child.gameObject.GetComponent<RawImage>().raycastTarget = false;
                        }
                    }
                    token2DLayer.GetComponent<Image>().raycastTarget = false;
                    prop2DLayer.GetComponent<Image>().raycastTarget = true;
                    map2DLayer.GetComponent<Image>().raycastTarget = false;
                }
                else if (mapLayerToggle.isOn)
                {
                    _CURRENT_LAYER = map2DLayer;
                    foreach (Transform child in map2DLayer.transform)
                    {
                        if (child.gameObject.GetComponent<RawImage>() != null)
                        {
                            child.gameObject.GetComponent<RawImage>().raycastTarget = true;
                        }
                    }
                    foreach (Transform child in token2DLayer.transform)
                    {
                        if (child.gameObject.GetComponent<RawImage>() != null)
                        {
                            child.gameObject.GetComponent<RawImage>().raycastTarget = false;
                        }
                    }
                    foreach (Transform child in prop2DLayer.transform)
                    {
                        if (child.gameObject.GetComponent<RawImage>() != null)
                        {
                            child.gameObject.GetComponent<RawImage>().raycastTarget = false;
                        }
                    }
                    token2DLayer.GetComponent<Image>().raycastTarget = false;
                    prop2DLayer.GetComponent<Image>().raycastTarget = false;
                    map2DLayer.GetComponent<Image>().raycastTarget = true;
                }
            }
            else
            {
                map2DLayer.GetComponent<Image>().raycastTarget = false;
                token2DLayer.GetComponent<Image>().raycastTarget = false;
                prop2DLayer.GetComponent<Image>().raycastTarget = false;
                drawingLayer.GetComponent<RawImage>().raycastTarget = true;

            }
        }
        else if (_GAME_MODE == "3D")
        {
            camera.GetComponent<FreeCameraController>().enabled = true;
            token2DLayer.SetActive(false);
            prop2DLayer.SetActive(false);
            map2DLayer.SetActive(false);
            token3DLayer.SetActive(true);
            prop3DLayer.SetActive(true);
            map3DLayer.SetActive(true);
            table.SetActive(false);
            drawingLayer.SetActive(false);
            HammerButton.SetActive(true);
            drawingButton.SetActive(false);
            if (tokenLayerToggle.isOn)
            {
                _CURRENT_LAYER = token3DLayer;
                foreach (Transform child in token3DLayer.transform)
                {
                    if (child.gameObject.GetComponent<BoxCollider>() != null)
                    {
                        child.gameObject.GetComponent<BoxCollider>().enabled = true;
                    }
                }
                foreach (Transform child in prop3DLayer.transform)
                {
                    if (child.gameObject.GetComponent<BoxCollider>() != null)
                    {
                        child.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                foreach (Transform child in map3DLayer.transform)
                {
                    if (child.gameObject.GetComponent<BoxCollider>() != null)
                    {
                        child.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
            }
            else if (propLayerToggle.isOn)
            {
                _CURRENT_LAYER = prop3DLayer;
                foreach (Transform child in prop3DLayer.transform)
                {
                    if (child.gameObject.GetComponent<BoxCollider>() != null)
                    {
                        child.gameObject.GetComponent<BoxCollider>().enabled = true;
                    }
                }
                foreach (Transform child in token3DLayer.transform)
                {
                    if (child.gameObject.GetComponent<BoxCollider>() != null)
                    {
                        child.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                foreach (Transform child in map3DLayer.transform)
                {
                    if (child.gameObject.GetComponent<BoxCollider>() != null)
                    {
                        child.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
            }
            else if (mapLayerToggle.isOn)
            {
                _CURRENT_LAYER = map3DLayer;
                foreach (Transform child in map3DLayer.transform)
                {
                    if (child.gameObject.GetComponent<BoxCollider>() != null)
                    {
                        child.gameObject.GetComponent<BoxCollider>().enabled = true;
                    }
                }
                foreach (Transform child in token3DLayer.transform)
                {
                    if (child.gameObject.GetComponent<BoxCollider>() != null)
                    {
                        child.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                foreach (Transform child in prop3DLayer.transform)
                {
                    if (child.gameObject.GetComponent<BoxCollider>() != null)
                    {
                        child.gameObject.GetComponent<BoxCollider>().enabled = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handles token layer toggle changes
    /// </summary>
    /// <param name="isOn">Whether the toggle is being turned on</param>
    private void OnTokenToggleChanged(bool isOn)
    {
        if (!isOn || isChangingToggles) return;

        isChangingToggles = true;
        propLayerToggle.isOn = false;
        mapLayerToggle.isOn = false;
        isChangingToggles = false;

        // Your token layer activation logic here

    }

    /// <summary>
    /// Handles prop layer toggle changes
    /// </summary>
    /// <param name="isOn">Whether the toggle is being turned on</param>
    private void OnPropToggleChanged(bool isOn)
    {
        if (!isOn || isChangingToggles) return;

        isChangingToggles = true;
        tokenLayerToggle.isOn = false;
        mapLayerToggle.isOn = false;
        isChangingToggles = false;


    }

    /// <summary>
    /// Handles map layer toggle changes
    /// </summary>
    /// <param name="isOn">Whether the toggle is being turned on</param>
    private void OnMapToggleChanged(bool isOn)
    {
        if (!isOn || isChangingToggles) return;

        isChangingToggles = true;
        tokenLayerToggle.isOn = false;
        propLayerToggle.isOn = false;
        isChangingToggles = false;


    }

}

/// <summary>
/// Simple component for tagging game objects with their asset names
/// </summary>
public class AssetName : MonoBehaviour
{
    /// <summary>The name of the asset this GameObject represents</summary>
    public string assetName;

    
}