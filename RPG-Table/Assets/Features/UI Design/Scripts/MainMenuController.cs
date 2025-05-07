using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
public class MainMenuController : MonoBehaviour
{   
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); 
    }

    public void JoinGame()
    {
        // Implement join game functionality here
        Debug.Log("Join game menu opened.");
    }

    public void CardEditor()
    {
        SceneManager.LoadScene("EditorScene"); 
    }


    public void OpenOptions()
    {
        // Implement options menu functionality here
        Debug.Log("Options menu opened.");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited.");
    }

    void Start(){
        SettingsManager.LoadSettings(); // Load settings when the game starts
    }
}


