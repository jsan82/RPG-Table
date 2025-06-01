using UnityEngine;

/// <summary>
/// Manages multi-display configuration for the application
/// </summary>
/// <remarks>
/// Handles:
/// - Remembering preferred display between sessions
/// - Validating display selection
/// - Activating chosen display on startup
/// </remarks>
public class DisplayManager : MonoBehaviour
{
    /// <summary>
    /// Initializes the preferred display from saved settings
    /// </summary>
    /// <remarks>
    /// - Loads saved monitor preference from PlayerPrefs
    /// - Falls back to primary display (0) if invalid
    /// - Requires display activation to be called in Start()
    /// </remarks>
    void Start()
    {

        int savedMonitor = PlayerPrefs.GetInt("PreferredMonitor", 1);


        if (savedMonitor >= 0 && savedMonitor < Display.displays.Length)
        {
            Display.displays[savedMonitor].Activate();
        }
        else
        {
            Display.displays[0].Activate();
        }
    }

    
    /// <summary>
    /// Changes and saves the preferred display setting
    /// </summary>
    /// <param name="monitorIndex">Index of the display to use (0-based)</param>
    /// <remarks>
    /// - Validates display index exists
    /// - Saves preference to PlayerPrefs
    /// - Note: Requires application restart to take effect
    /// </remarks>
    public void SetPreferredMonitor(int monitorIndex)
    {
        if (monitorIndex >= 0 && monitorIndex < Display.displays.Length)
        {
            PlayerPrefs.SetInt("PreferredMonitor", monitorIndex);
            PlayerPrefs.Save();


        }
    }
}