using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Manages the UI menu in the game scene, including brush, hammer, dice, and layer settings
/// </summary>
public class GameSceneMenuManager : MonoBehaviour
{
    /// <summary>Reference to the sound effects manager</summary>
    private SFXManager _sfxManager;

    /// <summary>Brush Settings</summary>
    /// <summary>UI panel for brush settings</summary>
    public GameObject brushSettingsPanel;
    /// <summary>Input field for hex color</summary>
    public TMP_InputField hexColorInput;
    /// <summary>Slider for brush size</summary>
    public Slider brushSizeSlider;

    /// <summary>Tracks if brush settings are visible</summary>
    private bool isBrushSettingsVisible = false;

    /// <summary>Hammer Settings</summary>
    /// <summary>UI panel for hammer settings</summary>
    public GameObject hammerSettingsPanel;
    /// <summary>Slider for hammer size</summary>
    public Slider hammerSizeSlider;

    /// <summary>Panel for hammer texture selection</summary>
    public GameObject hammerTexturePanel;
    /// <summary>Panel for skybox selection</summary>
    public GameObject hammerSkyboxPanel;

    /// <summary>Tracks if hammer settings are visible</summary>
    private bool isHammerSettingsVisible = false;

    /// <summary>Dice Settings</summary>
    /// <summary>UI panel for dice settings</summary>
    public GameObject diceSettingsPanel;

    /// <summary>Tracks if dice settings are visible</summary>
    private bool isDiceSettingsVisible = false;

    /// <summary>Layer Selector</summary>
    /// <summary>UI panel for layer visibility toggles</summary>
    public GameObject layerSelectorPanel;

    /// <summary>Toggle for grid layer</summary>
    public Toggle toggleGrid;
    /// <summary>Toggle for tokens layer</summary>
    public Toggle toggleTokens;
    /// <summary>Toggle for fog of war layer</summary>
    public Toggle toggleFogOfWar;
    /// <summary>Toggle for environment layer</summary>
    public Toggle toggleEnvironment;
    /// <summary>Toggle for background layer</summary>
    public Toggle toggleBackground;

    /// <summary>Tracks if layer selector is visible</summary>
    private bool isLayerSelectorVisible = false;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        /// <summary>Find and assign the SFX manager</summary>
        _sfxManager = FindObjectOfType<SFXManager>();

        /// <summary>Automatically attach sound effects to all buttons</summary>
        FindAllButtonsInScene();
    }

    /// <summary>
    /// Placeholder for chat functionality
    /// </summary>
    public void ButtonChat()
    {
        Debug.Log("Chat button clicked.");
    }

    /// <summary>
    /// Return to the main menu scene
    /// </summary>
    public void ButtonExit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Placeholder for photo functionality
    /// </summary>
    public void ButtonPhoto()
    {
        Debug.Log("Photo button clicked.");
    }

    /// <summary>
    /// Placeholder for journal functionality
    /// </summary>
    public void ButtonJournal()
    {
        Debug.Log("Journal button clicked.");
    }

    /// <summary>
    /// Placeholder for settings functionality
    /// </summary>
    public void ButtonSettings()
    {
        Debug.Log("Settings button clicked.");
    }

    /// <summary>
    /// Toggle visibility of brush settings panel
    /// </summary>
    public void ButtonBrush()
    {
        isBrushSettingsVisible = !isBrushSettingsVisible;
        brushSettingsPanel.SetActive(isBrushSettingsVisible);

        /// <summary>Hide other panels</summary>
        isHammerSettingsVisible = false;
        isDiceSettingsVisible = false;
        isLayerSelectorVisible = false;
        hammerSettingsPanel.SetActive(false);
        diceSettingsPanel.SetActive(false);
        layerSelectorPanel.SetActive(false);

        Debug.Log("Brush Panel toggled: " + isBrushSettingsVisible);
    }

    /// <summary>
    /// Placeholder for ruler functionality
    /// </summary>
    public void ButtonRuler()
    {
        Debug.Log("Ruler button clicked.");
    }

    /// <summary>
    /// Show texture selection panel for hammer
    /// </summary>
    public void TextureButton()
    {
        hammerTexturePanel.SetActive(true);
    }

    /// <summary>
    /// Show skybox selection panel
    /// </summary>
    public void SkyboxButton()
    {
        hammerSkyboxPanel.SetActive(true);
    }

    /// <summary>
    /// Toggle visibility of hammer settings panel
    /// </summary>
    public void ButtonHammer()
    {
        isHammerSettingsVisible = !isHammerSettingsVisible;
        hammerSettingsPanel.SetActive(isHammerSettingsVisible);

        /// <summary>Hide other panels</summary>
        isBrushSettingsVisible = false;
        isDiceSettingsVisible = false;
        isLayerSelectorVisible = false;
        brushSettingsPanel.SetActive(false);
        diceSettingsPanel.SetActive(false);
        layerSelectorPanel.SetActive(false);

        Debug.Log("Hammer button toggled: " + isHammerSettingsVisible);
    }

    /// <summary>
    /// Toggle visibility of dice settings panel
    /// </summary>
    public void ButtonDice()
    {
        isDiceSettingsVisible = !isDiceSettingsVisible;
        diceSettingsPanel.SetActive(isDiceSettingsVisible);

        /// <summary>Hide other panels</summary>
        isBrushSettingsVisible = false;
        isHammerSettingsVisible = false;
        isLayerSelectorVisible = false;
        brushSettingsPanel.SetActive(false);
        hammerSettingsPanel.SetActive(false);
        layerSelectorPanel.SetActive(false);

        Debug.Log("Dice button toggled: " + isDiceSettingsVisible);
    }

    /// <summary>
    /// Toggle visibility of layer selector panel
    /// </summary>
    public void ButtonList()
    {
        isLayerSelectorVisible = !isLayerSelectorVisible;
        layerSelectorPanel.SetActive(isLayerSelectorVisible);

        /// <summary>Hide other panels</summary>
        isBrushSettingsVisible = false;
        isHammerSettingsVisible = false;
        isDiceSettingsVisible = false;
        brushSettingsPanel.SetActive(false);
        hammerSettingsPanel.SetActive(false);
        diceSettingsPanel.SetActive(false);

        Debug.Log("Layer selector toggled: " + isLayerSelectorVisible);
    }

    /// <summary>
    /// Placeholder for send functionality
    /// </summary>
    public void ButtonSend()
    {
        Debug.Log("Send button clicked.");
    }

    /// <summary>
    /// Placeholder for load art functionality
    /// </summary>
    public void ButtonLoadArt()
    {
        Debug.Log("Load Art button clicked.");
    }

    /// <summary>
    /// Placeholder for add character functionality
    /// </summary>
    public void ButtonAddCharacter()
    {
        Debug.Log("Add Character button clicked.");
    }

    /// <summary>
    /// Show the specified panel
    /// </summary>
    /// <param name="panel">The panel to show</param>
    public void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    /// <summary>
    /// Hide the specified panel
    /// </summary>
    /// <param name="panel">The panel to hide</param>
    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// Get the current brush color
    /// </summary>
    /// <returns>The brush color or white if invalid</returns>
    public Color GetBrushColor()
    {
        /// <summary>Try to parse hex color input; fallback to white if invalid</summary>
        if (ColorUtility.TryParseHtmlString(hexColorInput.text, out Color color))
            return color;
        else
        {
            Debug.LogWarning("Invalid hex color. Using white as fallback.");
            return Color.white;
        }
    }

    /// <summary>
    /// Get the current brush size
    /// </summary>
    /// <returns>The current value of brush size slider</returns>
    public float GetBrushSize()
    {
        return brushSizeSlider.value;
    }

    /// <summary>
    /// Find all Button objects in the scene and attach sound effects
    /// </summary>
    public void FindAllButtonsInScene()
    {
        /// <summary>Find all Button objects in the scene, including inactive ones</summary>
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();

        List<GameObject> buttonGameObjects = new List<GameObject>();

        foreach (Button button in allButtons)
        {
            /// <summary>Only handle scene-resident buttons, not prefabs</summary>
            if (button.hideFlags == HideFlags.None)
            {
                /// <summary>Add special sound for the dice throw button</summary>
                if (button.gameObject.name == "ButtonThrow")
                {
                    button.onClick.AddListener(() => _sfxManager.Play(SFXType.DICE_ROLL));
                }
                /// <summary>Default sound for all other buttons</summary>
                else
                {
                    button.onClick.AddListener(() => _sfxManager.Play(SFXType.BUTTON_CLICK));
                }
            }
        }

        Debug.Log($"Found {buttonGameObjects.Count} buttons in the scene.");
    }
}