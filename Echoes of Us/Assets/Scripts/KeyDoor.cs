using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class KeyDoor : MonoBehaviour, IInteractable
{
    //Audio init
    [SerializeField] private AudioClip openSound;
    [SerializeField] private SpriteRenderer keyDoorSprite;
    private AudioSource audioSource;
    //public KeyDoor numOfKeys;
    [SerializeField] int keysExpectedMin = 2;
    [SerializeField] int keysExpectedMax = 2;
    private bool opened = false;
    private bool playerExists = false;
    private PlayerMovement playerScript;
    private SpriteRenderer spriteRenderer;
    private Collider2D cldr;
    private int Layer;
    // Start is called before the first frame update
    void Start()
    {
        //AudioSource set
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in KeyDoor");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        cldr = GetComponent<Collider2D>();
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
        if (playerExists)
        {
            
            if (SceneController.Instance.numOfKeys == keysExpectedMin || SceneController.Instance.numOfKeys == keysExpectedMax)
            {
                //numOfKeys++;
                playerScript.SetState(1);
                Debug.Log("Open!");
                spriteRenderer.enabled = false;
                cldr.enabled = false;
                opened = true;
                keyDoorSprite.enabled = false;
                Layer = LayerMask.NameToLayer("Default");
                audioSource.PlayOneShot(openSound);
                gameObject.layer = Layer;
            }
        }
    }
}
