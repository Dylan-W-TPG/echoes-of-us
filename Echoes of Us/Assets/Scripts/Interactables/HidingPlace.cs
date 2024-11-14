using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HidingPlace : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip lockerEnter;
    [SerializeField] private AudioClip lockerLeave;
    private bool inLocker = false;

    private AudioSource audioSource;

    private Animator lockerAnimator;

    void Start()
    {
        lockerAnimator = transform.Find("Sprite").gameObject.transform.GetComponent<Animator>();
        //Get AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in HidingPlace");
        }
    }
    public void Interact()
    {
        PlayerMovement player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        player.SetState(3);
        audioSource.PlayOneShot(lockerEnter);
        inLocker = true;
    }

    //This feels like an expensive operation and I've got no idea how to reduce it - ep
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
        SpriteRenderer playerSprite = player.transform.Find("Sprite").gameObject.transform.GetComponent<SpriteRenderer>();

        //Is the player interacting while within the trigger space
        //(I have to do a seperate check because the door has no idea it's being interacted with otherwise :I)
        if(playerScript.GetState() == 3) 
        {
            lockerAnimator.SetBool("doorOpen",false);
            playerSprite.enabled = false;
        }
        else
        {
            lockerAnimator.SetBool("doorOpen",true);
            playerSprite.enabled = true;
            if (inLocker)
            {
                audioSource.PlayOneShot(lockerLeave);
                inLocker = false;
            }
        }
    }

}