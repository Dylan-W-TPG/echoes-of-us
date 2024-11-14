using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private UIManager uiManager;

    // Creation of Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            // Destroys itself
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Ensure UIManager is assigned properly
        uiManager = GetComponent<UIManager>();

        if (uiManager == null)
        {
            // Try to find the UIManager in the scene if it's not found on the same GameObject
            uiManager = FindObjectOfType<UIManager>();

            if (uiManager == null)
            {
                Debug.LogError("UIManager component not found on the same GameObject or in the scene.");
            }
            else
            {
                Debug.LogWarning("UIManager was not found on the same GameObject, but was found elsewhere in the scene.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Run on the first frame of the Player GameObject
    public void PlayerStart()
    {
        Time.timeScale = 1.0f;
        SceneController.Instance.ResetKeys();
    }

    public void GameOver()
    {
        Debug.Log("GameOver called");
        if (uiManager != null)
        {
            uiManager.EnableDeathScreen();
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("UIManager is not assigned.");
        }
        
    }
}
