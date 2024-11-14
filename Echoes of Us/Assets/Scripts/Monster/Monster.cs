using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour
{
    // States
    private enum State
    {
        Wandering,
        MonsterLadder,
        HuntingNoise,
        HuntingPlayer,
        BreakingWall,
        PushingBox,
    }
    private State monsterState;

    private SpriteRenderer spriteRenderer;

    // Duration that monster will hunt the noise
    [SerializeField][Range(1f, 10f)] private float huntNoiseDuration = 1f;
    private float huntNoiseTimer;

    // Duration that monster will hunt the player
    [SerializeField][Range(0.1f, 3f)] private float huntPlayerDuration = 0.5f;
    private float huntPlayerTimer;

    // Hunting Range to find the Player
    [SerializeField][Range(1f, 10f)] private float huntRadius = 8f;

    // Chase Time
    [SerializeField] [Range(0f, 10f)] private float chaseTime = 3f;
    private float chaseTimer;

    // Normal Speed
    [SerializeField][Range(1f, 5f)] private float normalSpeed = 2.5f;

    // Chasing Speed
    [SerializeField][Range(1f, 8f)] private float chaseSpeed = 4f;

    // Specified Node System that the monster will take
    [SerializeField] private NodeSystem nodeSystem;

    // Frequency of scanning for Pathfinding graphs
    [SerializeField][Range(0.1f, 2f)] private float scanGraphFrequency = 0.5f;
    private float scanTimer;

    // Audio speed for stepping while hunting
    [SerializeField][Range(1f, 3f)] private float huntWalkSoundSpeed = 1.5f;

    // Rigidbody
    private Rigidbody2D rB;

    // AI Destination Setter
    private AIDestinationSetter aIDestinationSetter;

    // AI Pathfinding
    private AIPath aIPath;

    // Current target node
    private Transform targetNode;

    // Hunting Patrol Nodes (for Hunting Noise)
    private Transform patrolNode1;
    private Transform patrolNode2;

    // Player Object
    private PlayerMovement player;

    // Raycast variables for detection
    RaycastHit2D visionBlockCheck;
    Vector2 visionDirection;

    private Transform monsterSpriteTransform;
    private Animator monsterAnimator;
    private float prevPos;

    // Monster sounds
    private AudioSource stepSound;
    private AudioSource roarSound;

    
    // Start is called before the first frame update
    void Start()
    {
        //Get child's Sprite renderer
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Get the rigidbody and make sure it is kinematic
        rB = GetComponent<Rigidbody2D>();
        rB.isKinematic = true;

        // Get the AI Destination Setter for Pathfinding and then receive its first target
        aIDestinationSetter = GetComponent<AIDestinationSetter>();
        targetNode = nodeSystem.getEndNode();
        aIDestinationSetter.target = targetNode;

        // Get the Pathfinder and set the monster speed
        aIPath = GetComponent<AIPath>();
        aIPath.SearchPath();
        aIPath.maxSpeed = normalSpeed;

        // Set initial state of the monster
        monsterState = State.Wandering;

        // Set initial hunt timer
        huntNoiseTimer = huntNoiseDuration;
        huntPlayerTimer = huntPlayerDuration;

        // Get Player object
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();

        // Guard gainst 0 hunt duration
        if (huntNoiseDuration <= 0)
        {
            Debug.LogError("Invadlid Hunt Duration set on monster! Hunt Duration must be more than 0!");
            return;
        }

        // Guard gainst 0 hunt player duration
        if (huntPlayerDuration <= 0)
        {
            Debug.LogError("Invadlid Hunt Player Duration set on monster! Hunt Player Duration must be more than 0!");
            return;
        }

        // Guard gainst 0 hunt player radius
        if (huntRadius <= 0)
        {
            Debug.LogError("Invadlid Hunt Radius set on monster! Hunt Radius must be more than 0!");
            return;
        }

        monsterSpriteTransform = this.gameObject.transform.Find("Sprite").gameObject.transform;
        monsterAnimator = monsterSpriteTransform.GetComponent<Animator>();
        prevPos = 0;

        // Set scan timer
        scanTimer = scanGraphFrequency;

        // Set chase timer
        chaseTimer = chaseTime;

        // Set audio sources
        stepSound = GetComponent<AudioSource>();
        roarSound = this.transform.Find("RoarObject").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (monsterState)
        {
            case State.Wandering:
                //Decrease to normal speed
                aIPath.maxSpeed = normalSpeed;

                //Reset sprite to default colour
                ChangeColour(Color.white);
                // Check if the destination has reached, get its next target
                if (aIPath.reachedEndOfPath)
                {
                    targetNode = nodeSystem.getNextEndNode(targetNode);
                    aIDestinationSetter.target = targetNode;
                    aIPath.SearchPath();
                }

                // If player is in vision range, hunt the player
                if (isPlayerInRange() && !player.IsHiding)
                {
                    beginHunting();
                }
                break;
            case State.HuntingNoise:
                //Increase to chase speed
                aIPath.maxSpeed = chaseSpeed;

                //Set sprite to red
                ChangeColour(Color.red);
                // Check if hunt timer is <=0
                if (huntNoiseTimer <= 0)
                {
                    huntNoiseTimer = huntNoiseDuration;

                    //When hunting is over, give a new end node and reset state to wander
                    targetNode = nodeSystem.getEndNode();
                    aIDestinationSetter.target = targetNode;
                    monsterState = State.Wandering;
                    break;
                }

                // If player is in vision range, hunt the player
                if (isPlayerInRange() && !player.IsHiding)
                {
                    beginHunting();
                }

                //decrease hunt timer
                huntNoiseTimer -= Time.deltaTime;

                // Check if the destination has reached, get its next target between the two patrolling nodes
                if (aIPath.reachedEndOfPath)
                {
                    targetNode = aIDestinationSetter.target;
                    if (targetNode.position.Equals(patrolNode1.position))
                    {
                        aIDestinationSetter.target = patrolNode2;
                    }
                    else
                    {
                        aIDestinationSetter.target = patrolNode1;
                    }
                    aIPath.SearchPath();
                }
                break;
            case State.HuntingPlayer:
                //Increase to chase speed
                aIPath.maxSpeed = chaseSpeed;

                aIDestinationSetter.target = player.transform;

                //Set sprite to red
                ChangeColour(Color.red);

                // If the hunt timer has been reached
                if (huntPlayerTimer <= 0)
                {
                    // If the player is no longer in range, make the monster go back to wandering
                    if (!isPlayerInRange())
                    {
                        if(chaseTimer <= 0f)
                        {
                            stopHunting();
                        }
                        chaseTimer -= Time.deltaTime;
                    }
                    else if(player.IsHiding)
                    {
                        stopHunting();
                    }
                    else
                    {
                        // Reset hunt timer
                        huntPlayerTimer = huntPlayerDuration;

                        chaseTimer = chaseTime;
                    }

                    // Recalculate path
                    aIPath.SearchPath();
                }
                else
                {
                    // Countdown hunt timer
                    huntPlayerTimer -= Time.deltaTime;
                }
                break;
        }
        WalkingAnimation();

        // If the scan timer has reached zero, scan all Pathfinding graphs
        if (scanTimer <= 0)
        {
            // Recalculate all graphs
            AstarPath.active.Scan();

            scanTimer = scanGraphFrequency;
        }
        else
        {
            scanTimer -= Time.deltaTime;
        }
    }

    // Get the monster to follow the noise maker based on the maker's position and the patrolling nodes
    public void followNoiseMaker(Transform noiseMakerNode, Transform endNode1, Transform endNode2)
    {
        // Set destination to the noise maker and calculate the path
        aIDestinationSetter.target = noiseMakerNode;
        aIPath.SearchPath();

        // Set the monster state to hunting the noise if the player is not in range or is hiding
        if (!isPlayerInRange() || player.IsHiding)
        {
            monsterState = State.HuntingNoise;
        }

        // Receive the patrolling nodes
        patrolNode1 = endNode1;
        patrolNode2 = endNode2;

        //Reset hunt timer when hearing noise
        huntNoiseTimer = huntNoiseDuration;
    }

    // Check whether the monster is within vision range with the player
    private bool isPlayerInRange() {
        // Set direction from where the monster shall look at the player
        visionDirection = player.transform.position - this.transform.position;

        // Add layerMask to only detect walls, ground, and ceilings
        int layerMask = (1 << LayerMask.NameToLayer("WallGround"));

        // Gather the raycast data so that it checks whether the player is in vision or not
        visionBlockCheck = Physics2D.Raycast(this.transform.position, visionDirection.normalized, visionDirection.magnitude, layerMask);

        // If the player is within radius and in vision, then player is in range
        return visionDirection.magnitude <= huntRadius && visionBlockCheck.collider == null;
    }

    // Do this function when monster is about to hunt the player
    private void beginHunting() {
        // Increase to chase speed
        //aIPath.maxSpeed = chaseSpeed;

        // Recalculate path with the player as the target
        aIDestinationSetter.target = player.transform;
        aIPath.SearchPath();

        // Play roar sound and increase walking sound speed
        roarSound.Play();
        stepSound.pitch = huntWalkSoundSpeed;

        // Switch states
        monsterState = State.HuntingPlayer;
    }

    // Do this function when monster is about to stop hunting the player
    private void stopHunting() {
        // Decrease to normal speed
        //aIPath.maxSpeed = normalSpeed;

        // Set path back to a random end node
        targetNode = nodeSystem.getEndNode();
        aIDestinationSetter.target = targetNode;

        // Decrease walking sound speed to normal
        stepSound.pitch = 1f;

        // Switch states
        monsterState = State.Wandering;
    }

    private void ChangeColour(Color newColour)
    {
        //Change sprite to new colour
        spriteRenderer.color = newColour;
    }

    private void WalkingAnimation()
    {
        if(transform.position.x < prevPos)
        {
            prevPos = transform.position.x;
            transform.eulerAngles = new Vector3(0, 0, 0);
            monsterAnimator.SetInteger("monsterAniState", 1);
        }
        else if(transform.position.x > prevPos)
        {
            prevPos = transform.position.x;
            transform.eulerAngles = new Vector3(0, 180, 0);
            monsterAnimator.SetInteger("monsterAniState", 1);
        }
    }

    public int GetState()
    {
        if (monsterState == State.Wandering)
        {
            return 0;
        }
        else if (monsterState == State.MonsterLadder)
        {
            return 1;
        }
        else if (monsterState == State.HuntingNoise)
        {
            return 2;
        }
        else if (monsterState == State.HuntingPlayer)
        {
            return 3;
        }
        else if (monsterState == State.BreakingWall)
        {
            return 4;
        }
        else if (monsterState == State.PushingBox)
        {
            return 5;
        }
        return -1;
    }
}
