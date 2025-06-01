using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
public class MainMenuController : MonoBehaviour
{
    private SFXManager _sfxManager;

    private void Start()
    {
        SettingsManager.LoadSettings(); // Load settings when the game starts
        _sfxManager = FindObjectOfType<SFXManager>();
        _sfxManager.PlayLooped();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
        _sfxManager.Play(SFXType.BUTTON_CLICK);
        _sfxManager.StopLooped();
    }

    public void JoinGame()
    {
        // Implement join game functionality here
        Debug.Log("Join game menu opened.");
        _sfxManager.Play(SFXType.BUTTON_CLICK);
        _sfxManager.StopLooped();
    }

    public void CardEditor()
    {
        SceneManager.LoadScene("EditorScene");
        _sfxManager.Play(SFXType.BUTTON_CLICK);
        _sfxManager.StopLooped();
    }


    public void OpenOptions()
    {
        // Implement options menu functionality here
        Debug.Log("Options menu opened.");
        _sfxManager.Play(SFXType.BUTTON_CLICK);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited.");
    }
}


