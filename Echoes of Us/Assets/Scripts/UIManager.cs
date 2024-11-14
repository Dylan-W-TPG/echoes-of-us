using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameObject deathScreen;

    private void Awake()
    {
        try
        {
            GameObject eventSystem = GameObject.Find("EventSystem");

            // Check if the GameObject is found
            if (eventSystem == null)
            {
                throw new System.NullReferenceException("No GameObject named 'EventSystem' found in the scene. An EventSystem is needed for UI Interaction elements to work");
            }

            deathScreen = GameObject.Find("DeathScreen");

            if (deathScreen == null)
            {
                throw new System.NullReferenceException("No GameObject named 'DeathScreen' found in the scene.");
            }
        }
        catch (System.Exception e)
        {
            // Catch any exceptions and print the error message
            Debug.LogError("Error: " + e.Message);
        }
    }

    // Check if there is an event system to allow UI buttons to work
    private void Start()
    {
       deathScreen.SetActive(false);
    }

    // When run, toggles DeathPanel/Death Screen
    public void ToggleDeathScreen()
    {
        deathScreen.SetActive(!deathScreen.activeSelf);
    }

    public void EnableDeathScreen()
    {
        deathScreen.SetActive(true);
    }

    public void DisableDeathScreen()
    {
        deathScreen.SetActive(false);
    }

    public void OnRetryPressed()
    {
        DisableDeathScreen();
        SceneController.Instance.Reload();
    }

    public void OnMenuPressed()
    {
        Debug.Log("Menu Button");
        // Needs implementation
    }
}
