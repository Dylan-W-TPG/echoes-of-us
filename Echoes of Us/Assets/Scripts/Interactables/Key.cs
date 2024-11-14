using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Key : MonoBehaviour, IInteractable
{
    //Audio init
    [SerializeField] private AudioClip pickUpSound;
    [SerializeField] private SpriteRenderer keySprite;
    private AudioSource audioSource;
    //public KeyDoor numOfKeys;
    private bool playerExists = false;
    private bool collected = false;
    private PlayerMovement playerScript;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update

    /*
    public static Key Instance;

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
    */

    void Start()
    {
        //AudioSource set
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in Key");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        //Gets the transform of Obj that has "Player" tag
        try
        {
            GameObject playerObject = GameObject.FindWithTag("Player");

            //check if player was found
            if (playerObject != null)
            {
                playerExists = true;
                playerScript = playerObject.GetComponent<PlayerMovement>();
            }
            else
            {
                throw new System.NullReferenceException("No GameObject with 'Player' Tag found");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error while finding player: " + e.Message);
        }
    }

    public void Interact()
    {
        if(playerExists)
        {
            if(!collected)
            {
                SceneController.Instance.numOfKeys++;
                //numOfKeys++;
                playerScript.SetState(1);
                Debug.Log(SceneController.Instance.numOfKeys);
                collected = true;
                audioSource.PlayOneShot(pickUpSound);
                spriteRenderer.color = Color.red;
                keySprite.enabled = false;
            }
        }
    }
}
