using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

// Manages the UI menu in the game scene, including brush, hammer, dice, and layer settings
public class GameSceneMenuManager : MonoBehaviour
{
    private SFXManager _sfxManager; // Reference to the sound effects manager

    // Brush Settings
    public GameObject brushSettingsPanel; // UI panel for brush settings
    public TMP_InputField hexColorInput;  // Input field for hex color
    public Slider brushSizeSlider;        // Slider for brush size

    private bool isBrushSettingsVisible = false; // Tracks if brush settings are visible

    // Hammer Settings
    public GameObject hammerSettingsPanel; // UI panel for hammer settings
    public Slider hammerSizeSlider;        // Slider for hammer size

    public GameObject hammerTexturePanel;  // Panel for hammer texture selection
    public GameObject hammerSkyboxPanel;   // Panel for skybox selection

    private bool isHammerSettingsVisible = false; // Tracks if hammer settings are visible

    // Dice Settings
    public GameObject diceSettingsPanel; // UI panel for dice settings

    private bool isDiceSettingsVisible = false; // Tracks if dice settings are visible

    // Layer Selector
    public GameObject layerSelectorPanel; // UI panel for layer visibility toggles

    public Toggle toggleGrid;         // Toggle for grid layer
    public Toggle toggleTokens;       // Toggle for tokens layer
    public Toggle toggleFogOfWar;     // Toggle for fog of war layer
    public Toggle toggleEnvironment;  // Toggle for environment layer
    public Toggle toggleBackground;   // Toggle for background layer

    private bool isLayerSelectorVisible = false; // Tracks if layer selector is visible

    void Start()
    {
        // Find and assign the SFX manager
        _sfxManager = FindObjectOfType<SFXManager>();

        // Automatically attach sound effects to all buttons
        FindAllButtonsInScene();
    }

    public void ButtonChat()
    {
        // Placeholder for chat functionality
        Debug.Log("Chat button clicked.");
    }

    public void ButtonExit()
    {
        // Return to the main menu scene
        SceneManager.LoadScene("MainMenu");
    }

    public void ButtonPhoto()
    {
        // Placeholder for photo functionality
        Debug.Log("Photo button clicked.");
    }

    public void ButtonJournal()
    {
        // Placeholder for journal functionality
        Debug.Log("Journal button clicked.");
    }

    public void ButtonSettings()
    {
        // Placeholder for settings functionality
        Debug.Log("Settings button clicked.");
    }

    public void ButtonBrush()
    {
        // Toggle visibility of brush settings panel
        isBrushSettingsVisible = !isBrushSettingsVisible;
        brushSettingsPanel.SetActive(isBrushSettingsVisible);

        // Hide other panels
        isHammerSettingsVisible = false;
        isDiceSettingsVisible = false;
        isLayerSelectorVisible = false;
        hammerSettingsPanel.SetActive(false);
        diceSettingsPanel.SetActive(false);
        layerSelectorPanel.SetActive(false);

        Debug.Log("Brush Panel toggled: " + isBrushSettingsVisible);
    }

    public void ButtonRuler()
    {
        // Placeholder for ruler functionality
        Debug.Log("Ruler button clicked.");
    }

    public void TextureButton()
    {
        // Show texture selection panel for hammer
        hammerTexturePanel.SetActive(true);
    }

    public void SkyboxButton()
    {
        // Show skybox selection panel
        hammerSkyboxPanel.SetActive(true);
    }

    public void ButtonHammer()
    {
        // Toggle visibility of hammer settings panel
        isHammerSettingsVisible = !isHammerSettingsVisible;
        hammerSettingsPanel.SetActive(isHammerSettingsVisible);

        // Hide other panels
        isBrushSettingsVisible = false;
        isDiceSettingsVisible = false;
        isLayerSelectorVisible = false;
        brushSettingsPanel.SetActive(false);
        diceSettingsPanel.SetActive(false);
        layerSelectorPanel.SetActive(false);

        Debug.Log("Hammer button toggled: " + isHammerSettingsVisible);
    }

    public void ButtonDice()
    {
        // Toggle visibility of dice settings panel
        isDiceSettingsVisible = !isDiceSettingsVisible;
        diceSettingsPanel.SetActive(isDiceSettingsVisible);

        // Hide other panels
        isBrushSettingsVisible = false;
        isHammerSettingsVisible = false;
        isLayerSelectorVisible = false;
        brushSettingsPanel.SetActive(false);
        hammerSettingsPanel.SetActive(false);
        layerSelectorPanel.SetActive(false);

        Debug.Log("Dice button toggled: " + isDiceSettingsVisible);
    }

    public void ButtonList()
    {
        // Toggle visibility of layer selector panel
        isLayerSelectorVisible = !isLayerSelectorVisible;
        layerSelectorPanel.SetActive(isLayerSelectorVisible);

        // Hide other panels
        isBrushSettingsVisible = false;
        isHammerSettingsVisible = false;
        isDiceSettingsVisible = false;
        brushSettingsPanel.SetActive(false);
        hammerSettingsPanel.SetActive(false);
        diceSettingsPanel.SetActive(false);

        Debug.Log("Layer selector toggled: " + isLayerSelectorVisible);
    }

    public void ButtonSend()
    {
        // Placeholder for send functionality
        Debug.Log("Send button clicked.");
    }

    public void ButtonLoadArt()
    {
        // Placeholder for load art functionality
        Debug.Log("Load Art button clicked.");
    }

    public void ButtonAddCharacter()
    {
        // Placeholder for add character functionality
        Debug.Log("Add Character button clicked.");
    }

    public void ShowPanel(GameObject panel)
    {
        // Show the specified panel
        panel.SetActive(true);
    }

    public void HidePanel(GameObject panel)
    {
        // Hide the specified panel
        panel.SetActive(false);
    }

    public Color GetBrushColor()
    {
        // Try to parse hex color input; fallback to white if invalid
        if (ColorUtility.TryParseHtmlString(hexColorInput.text, out Color color))
            return color;
        else
        {
            Debug.LogWarning("Invalid hex color. Using white as fallback.");
            return Color.white;
        }
    }

    public float GetBrushSize()
    {
        // Get current value of brush size slider
        return brushSizeSlider.value;
    }

    public void FindAllButtonsInScene()
    {
        // Find all Button objects in the scene, including inactive ones
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();

        List<GameObject> buttonGameObjects = new List<GameObject>();

        foreach (Button button in allButtons)
        {
            // Only handle scene-resident buttons, not prefabs
            if (button.hideFlags == HideFlags.None)
            {
                // Add special sound for the dice throw button
                if (button.gameObject.name == "ButtonThrow")
                {
                    button.onClick.AddListener(() => _sfxManager.Play(SFXType.DICE_ROLL));
                }
                // Default sound for all other buttons
                else
                {
                    button.onClick.AddListener(() => _sfxManager.Play(SFXType.BUTTON_CLICK));
                }
            }
        }

        Debug.Log($"Found {buttonGameObjects.Count} buttons in the scene.");
    }
}
