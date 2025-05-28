using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    void Start()
    {
        // Domyślnie ustawiamy monitor 0 (pierwszy)
        int savedMonitor = PlayerPrefs.GetInt("PreferredMonitor", 1);

        // Sprawdzamy, czy wybrany monitor istnieje
        if (savedMonitor >= 0 && savedMonitor < Display.displays.Length)
        {
            Display.displays[savedMonitor].Activate();
        }
        else
        {
            Debug.LogWarning("Wybrany monitor nie istnieje, używam domyślnego (0)");
            Display.displays[0].Activate();
        }
    }

    // Metoda do zmiany monitora (np. z menu opcji)
    public void SetPreferredMonitor(int monitorIndex)
    {
        if (monitorIndex >= 0 && monitorIndex < Display.displays.Length)
        {
            PlayerPrefs.SetInt("PreferredMonitor", monitorIndex);
            PlayerPrefs.Save();
            Debug.Log("Zapisano preferowany monitor: " + monitorIndex);
            
            // Wymagany restart gry, aby zmiana zadziałała
            // Możesz dodać komunikat dla gracza
        }
    }
}