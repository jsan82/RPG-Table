using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameSceneMenuManager : MonoBehaviour
{
    // Brush Settings
    public GameObject brushSettingsPanel;
    public TMP_InputField hexColorInput;
    public Slider brushSizeSlider;

    private bool isBrushSettingsVisible = false;

    // Hammer Settings
    public GameObject hammerSettingsPanel;
    public Slider hammerSizeSlider;

    private bool isHammerSettingsVisible = false;

    // Dice Settings
    public GameObject diceSettingsPanel;

    private bool isDiceSettingsVisible = false;

    // Layer Selector
    public GameObject layerSelectorPanel;

    public Toggle toggleGrid;
    public Toggle toggleTokens;
    public Toggle toggleFogOfWar;
    public Toggle toggleEnvironment;
    public Toggle toggleBackground;

    private bool isLayerSelectorVisible = false;

    public void ButtonChat()
    {
        // Implement chat button here
        Debug.Log("Chat button clicked.");
    }

    public void ButtonExit(){
        SceneManager.LoadScene("MainMenu"); 
    
    }
    public void ButtonPhoto()
    {
        // Implement photo button here
        Debug.Log("Photo button clicked.");
    }

    public void ButtonJournal()
    {
        // Implement journal button here
        Debug.Log("Journal button clicked.");
    }

    public void ButtonSettings()
    {
        // Implement settings button here
        Debug.Log("Settings button clicked.");
    }

    public void ButtonBrush()
    {
        // Implement brush button here
        isBrushSettingsVisible = !isBrushSettingsVisible;
        brushSettingsPanel.SetActive(isBrushSettingsVisible);
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
        // Implement ruler button here
        Debug.Log("Ruler button clicked.");
    }

    public void ButtonHammer()
    {
        // Implement hammer button here
        isHammerSettingsVisible = !isHammerSettingsVisible;
        hammerSettingsPanel.SetActive(isHammerSettingsVisible);
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
        // Implement dice button here
        isDiceSettingsVisible = !isDiceSettingsVisible;
        diceSettingsPanel.SetActive(isDiceSettingsVisible);
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
        // Implement list button here
        isLayerSelectorVisible = !isLayerSelectorVisible;
        layerSelectorPanel.SetActive(isLayerSelectorVisible);
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
        // Implement send button here
        Debug.Log("Send button clicked.");
    }

    public void ButtonLoadArt()
    {
        // Implement load art button here
        Debug.Log("Load Art button clicked.");
    }

    public void ButtonAddCharacter()
    {
        // Implement add character button here
        Debug.Log("Add Character button clicked.");
    }

    public void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public Color GetBrushColor()
    {
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
        return brushSizeSlider.value;
    }
}