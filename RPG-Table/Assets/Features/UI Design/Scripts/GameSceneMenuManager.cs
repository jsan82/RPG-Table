using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneMenuManager : MonoBehaviour
{
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
        Debug.Log("Brush button clicked.");
    }

    public void ButtonRuler()
    {
        // Implement ruler button here
        Debug.Log("Ruler button clicked.");
    }

    public void ButtonDice()
    {
        // Implement dice button here
        Debug.Log("Dice button clicked.");
    }

    public void ButtonList()
    {
        // Implement list button here
        Debug.Log("List button clicked.");
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
}