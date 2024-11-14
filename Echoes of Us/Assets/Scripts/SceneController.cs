using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    //private Key key;

    public int numOfKeys;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            //Destorys itself
            Destroy(gameObject);
        }
    }
    /*
    private void Start()
    {
        if (key.collected == true)
        {
            Debug.Log("Hello");
        }
    }
    */
    public void ResetKeys()
    {
        numOfKeys = 0;
    }
    //Run this method if we want to go through scenes in their build order - this can be found under file > Build Settings...
    public void NextLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex-1);
    }
    
    //Run this method if we want to go to a specific scene
    public void LoadScene(string SceneName)
    {
        //SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync(SceneName);
    }

    public void Reload()
    {
        //Loads the current scene again
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
