using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadEditorScene()
    {
        // Load the editor scene (assuming it's the second scene in the build settings)
        SceneManager.LoadScene("EditorScene");
    }
    
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
        
        // If running in the editor, stop playing the scene
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
