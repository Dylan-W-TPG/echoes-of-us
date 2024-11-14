using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    // Player Settings
    [Range(0.1f, 1f)] [SerializeField] private float speed = 0.5f;
    [Range(2f, 5f)] [SerializeField] private float maxSpeed = 5f;
    [Range(1f, 50f)] [SerializeField] private float jumpForce = 6f;

    // Player Inputs
    private PlayerActions actions;
    private InputAction movementAction;
    private InputAction jumpAction;
    private InputAction interactAction;
    private InputAction flashAction;
    private InputAction ladderAction;

    //Trolley Child
    private Trolley trolley;

    // Jump States
    private enum PlayerState
    {
        OnGround,
        Jumping,
        Interacting,
        Hiding,
        MovingTrolley,
        ClimbingLadder,
    }
    private PlayerState playerState;

    //animation variables
    private Transform playerSpriteTransform;
    private Animator playerAnimator;
    private float prevAccel;

    //when player is in range on an interactable = true
    private bool canInteract = false;

    // when player is in hiding
    private bool isHiding = false;
    public bool IsHiding
    {
        get
        {
            return isHiding;
        }
    }

    // Rigidbody
    private Rigidbody2D rB;

    private float acceleration;
    private float ladderAcceleration;

    // Audio Source and Clips
    private AudioSource audioSource;
    [SerializeField] private AudioClip ladderClimbAudio;

    void Awake()
    {
        actions = new PlayerActions();

        movementAction = actions.movement.move;
        ladderAction = actions.movement.ladder;

        jumpAction = actions.movement.jump;
        jumpAction.started += OnJump;
        interactAction = actions.movement.interact;
        interactAction.started += OnInteract;
        interactAction.canceled += OffInteract;
        flashAction = actions.movement.flashlight;
        flashAction.started += OnFlash;

    }

    void OnEnable()
    {
        movementAction.Enable();
        jumpAction.Enable();
        interactAction.Enable();
        flashAction.Enable();
        ladderAction.Enable();
    }

    void OnDisable()
    {
        movementAction.Disable();
        jumpAction.Disable();
        interactAction.Disable();
        flashAction.Disable();
        ladderAction.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        //AudioSource set
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in Player");
        }
        GameManager.Instance.PlayerStart();
        // Get rigidbody
        rB = GetComponent<Rigidbody2D>();

        // Get sprite transform and animator
        playerSpriteTransform = this.gameObject.transform.Find("Sprite").gameObject.transform;
        playerAnimator = playerSpriteTransform.GetComponent<Animator>();
        prevAccel = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerState)
        {
            // If player is on ground
            case PlayerState.OnGround:
                //Reset disabled Actions
                jumpAction.Enable();
                //Resets rigidbody to default state to allow movement
                rB.constraints = RigidbodyConstraints2D.FreezeRotation;
                // Input movements only happen when the player is on ground
                acceleration = movementAction.ReadValue<Vector2>().x;
                WalkingAnimation();
                isHiding = false;
                break;
            // If player is jumping
            case PlayerState.Jumping:
                //Resets rigidbody to default state to allow movement
                rB.constraints = RigidbodyConstraints2D.FreezeRotation;
                // Stop the player from moving horizontally when it hits the wall.
                acceleration = 0f;
                playerAnimator.SetInteger("playerAniState", 2);
                break;
            // When the player is interacting
            case PlayerState.Interacting:
                break;
            case PlayerState.Hiding:
                isHiding = true;
                rB.constraints = RigidbodyConstraints2D.FreezeAll;
                break;
            case PlayerState.MovingTrolley:
                //movemeng while holding trolley & Disable jump
                acceleration = movementAction.ReadValue<Vector2>().x;
                jumpAction.Disable();
                //Geting instance of the Trolley so it can detatch it later
                trolley = GetComponentInChildren<Trolley>();
                break;
            //When player is climbing ladder
            case PlayerState.ClimbingLadder:
                rB.constraints = RigidbodyConstraints2D.FreezePositionX;
                acceleration = 0f;
                break;

        }
    }

    void FixedUpdate()
    {
        // If the player is at the player threshold, stop the force acceleration
        if (rB.velocity.magnitude <= maxSpeed)
        {
            // Make the player move based on inputs
            rB.AddForce(Vector2.right * speed * acceleration, ForceMode2D.Impulse);
            rB.AddForce(Vector2.up * speed * ladderAcceleration, ForceMode2D.Impulse);
        }
    }

    //When player dies, disables the game object, and call the GameOver method in GameManager
    private void PlayerDie()
    {
        Debug.Log("YOU DIED");
        GameManager.Instance.GameOver();
        gameObject.SetActive(false);
    }

    // If the player collides with an object
    void OnCollisionEnter2D(Collision2D collision)
    {
        // If the player lands back to the ground, return state to OnGround
        if (collision.collider.tag.Equals("Ground") || collision.collider.tag.Equals("Trolley"))
        {
            playerState = PlayerState.OnGround;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Safe catch if player is still not in the correct state when on the ground
        if (playerState == PlayerState.Jumping && collision.collider.tag.Equals("Ground"))
        {
            playerState = PlayerState.OnGround;
        }
    }

    //When player enters the collision box of a trigger (goes through it)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Interactable"))
        {
            canInteract = true;
        }
        //Player dies when not hiding, and collides with monster
        if (collision.tag.Equals("Monster") && playerState != PlayerState.Hiding)
        {
            PlayerDie();
        }
    }
    //when player stats in collision box of a trigger
    private void OnTriggerStay2D(Collider2D collision)
    {
        canInteract = true;
        //When player is in State Interacting, Interact with it
        if (playerState == PlayerState.Interacting && collision.tag.Equals("Interactable"))
        {
            if (collision.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
            }
        }
        //Player dies when not hiding, and collides with monster
        if (collision.tag.Equals("Monster") && playerState != PlayerState.Hiding)
        {
            Debug.Log("YOU DIED");
            PlayerDie();
        }
        if (collision.tag.Equals("Ladder"))
        {
            ladderAcceleration = ladderAction.ReadValue<Vector2>().y;
            if (ladderAcceleration != 0)
            {
                playerState = PlayerState.ClimbingLadder;
                //ladder audio
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = ladderClimbAudio;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
        }
    }

    //When player leaves the collision box of a trigger <-- Warning, will cause issues with overlapping triggers, so... don't do that please 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Interactable"))
        {
            canInteract = false;
        }
        if (collision.tag.Equals("Ladder"))
        {
            //Stop vertical movement and re-enable left/right acceleration
            ladderAcceleration = 0f;
            playerState = PlayerState.OnGround;
            //stops ladder audio
            if (audioSource.clip == ladderClimbAudio)
            {
                audioSource.loop = false;
                audioSource.Stop();
            }
        }
    }

    // When the player presses the jump key
    void OnJump(InputAction.CallbackContext context)
    {
        // If the player is still on air, do nothing
        if (playerState != PlayerState.Jumping)
        {
            // Add jumping force to make the player jump
            rB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            playerState = PlayerState.Jumping;
        }
    }

    //When Interact key is pressed
    private void OnInteract(InputAction.CallbackContext context)
    {
        //Changes player State when colliding with interactable obj and on ground to Interacting State
        if (playerState == PlayerState.OnGround && canInteract)
        {
            playerState = PlayerState.Interacting;
        }
        
    }

    //When interact key is released
    private void OffInteract(InputAction.CallbackContext context)
    {
        //changes state when interaction button is released, and state is Interacting or hiding
        if (playerState == PlayerState.Interacting || playerState == PlayerState.Hiding && context.canceled)
        {
            playerState = PlayerState.OnGround;
        }
        //Detaches trolley, and empties variable
        if (playerState == PlayerState.MovingTrolley && context.canceled)
        {
            playerState = PlayerState.OnGround;
            trolley.ToggleMoveable();
            trolley = null;
        }
    }

    private void OnFlash(InputAction.CallbackContext context)
    {
        Debug.Log("OnFlash");
        PlayerDie();
    }

    //flips walking animation to correct direction and sets to idle if accel=0
    private void WalkingAnimation()
    {
        if(rB.velocity.x < prevAccel && rB.velocity.x!=0)
        {
            prevAccel = acceleration;
            playerSpriteTransform.eulerAngles = new Vector3(0, 0, 0);
            playerAnimator.SetInteger("playerAniState", 1);
        }
        else if(rB.velocity.x > prevAccel && rB.velocity.x!=0)
        {
            prevAccel = acceleration;
            playerSpriteTransform.eulerAngles = new Vector3(0, 180, 0);
            playerAnimator.SetInteger("playerAniState", 1);
        }
        else if(rB.velocity.x == 0)
        {
            playerAnimator.SetInteger("playerAniState", 0);
        }  
    }

    public void SetState(int stateCode)
    {
        if (stateCode == 0)
        {
            playerState = PlayerState.OnGround;
        }
        else if (stateCode == 1)
        {
            playerState = PlayerState.Jumping;
        }
        else if (stateCode == 2)
        {
            playerState = PlayerState.Interacting;
        }
        else if (stateCode == 3)
        {
            playerState = PlayerState.Hiding;
        }
        else if (stateCode == 4)
        {
            playerState = PlayerState.MovingTrolley;
        }
    }

    //returns player state as an int
    public int GetState()
    {
        if (playerState == PlayerState.OnGround)
        {
            return 0;
        }
        else if (playerState == PlayerState.Jumping)
        {
            return 1;
        }
        else if (playerState == PlayerState.Interacting)
        {
            return 2;
        }
        else if (playerState == PlayerState.Hiding)
        {
            return 3;
        }
        else if (playerState == PlayerState.MovingTrolley)
        {
            return 4;
        }
        return -1;
    }
}
