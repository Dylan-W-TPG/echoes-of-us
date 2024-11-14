using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class nextLevelTrigger : MonoBehaviour
{
    [SerializeField] private string NextSceneName;

    //Audio init
    [SerializeField] private AudioClip nextLevelSound;
    private AudioSource audioSource;

    private void Start()
    {
        //AudioSource set
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in NextLevelTrigger");
        }
        if (NextSceneName == null)
        {
            Debug.LogError("NO SCENE ATTACHED TO NEXT LEVEL TRIGGER!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SceneController sceneController = GameObject.Find("GameManager").GetComponent<SceneController>();
            sceneController.LoadScene(NextSceneName);
            audioSource.PlayOneShot(nextLevelSound);
        }
    }
}
