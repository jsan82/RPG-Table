using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

// Main controller for handling main menu interactions in the game
public class MainMenuController : MonoBehaviour
{
    private SFXManager _sfxManager; // Reference to the sound effects manager

    private void Start()
    {
        // Load player settings when the game starts
        SettingsManager.LoadSettings();

        // Find the SFXManager instance in the scene
        _sfxManager = FindObjectOfType<SFXManager>();

        // Start playing looped background audio (e.g., menu music)
        _sfxManager.PlayLooped();
    }

    // Triggered when the "Host" button is pressed
    public void StartGame()
    {
        // Load the main gameplay scene
        SceneManager.LoadScene("GameScene");

        // Play button click sound effect
        _sfxManager.Play(SFXType.BUTTON_CLICK);

        // Stop the looped menu background audio
        _sfxManager.StopLooped();
    }

    // Triggered when the "Join" button is pressed
    public void JoinGame()
    {
        // Placeholder for future multiplayer joining logic
        Debug.Log("Join game menu opened.");

        // Play button click sound effect
        _sfxManager.Play(SFXType.BUTTON_CLICK);

        // Stop the looped menu background audio
        _sfxManager.StopLooped();
    }

    // Triggered when the "Card Editor" button is pressed
    public void CardEditor()
    {
        // Load the scene where players can edit cards
        SceneManager.LoadScene("EditorScene");

        // Play button click sound effect
        _sfxManager.Play(SFXType.BUTTON_CLICK);

        // Stop the looped menu background audio
        _sfxManager.StopLooped();
    }

    // Triggered when the "Options" button is pressed
    public void OpenOptions()
    {
        // Placeholder for options menu logic
        Debug.Log("Options menu opened.");

        // Play button click sound effect
        _sfxManager.Play(SFXType.BUTTON_CLICK);
    }

    // Triggered when the "Exit" button is pressed
    public void ExitGame()
    {
        // Quit the application
        Application.Quit();

        // Log the exit action (visible in editor only)
        Debug.Log("Game exited.");
    }
}
