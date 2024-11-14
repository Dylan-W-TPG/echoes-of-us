using UnityEngine;

public class VentInteractionHandler : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform otherVent;
    private Transform playerTransform;
    private PlayerMovement playerScript;
    private AudioSource audioSource;

    //animation variables
    private Animator ventAnimator;
    private Animator otherVentAnimator;

     void Start()
    {
        ventAnimator = transform.Find("Sprite").gameObject.transform.GetComponent<Animator>();
        otherVentAnimator = otherVent.Find("Sprite").gameObject.transform.GetComponent<Animator>();

        //Gets the transform of Obj that has "Player" tag
        try
        {
            GameObject playerObject = GameObject.FindWithTag("Player");

            //check if player was found
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
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

        //Get audio source from parent
        audioSource = GetComponentInParent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in VentParent");
        }
    }

    public void Interact()
    {
        ventAnimator.SetTrigger("ventOpen");
        otherVentAnimator.SetTrigger("ventOpen");
        //Teleports player to other vent
        playerTransform.position = otherVent.position;
        playerScript.SetState(1);
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
