using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NoiseMachineHandler : MonoBehaviour, IInteractable
{
    [SerializeField] NoiseSpawner noiseSpawner;
    [SerializeField] bool canManualTurnOff = false;
    [SerializeField] bool turnOffAfterDelay = true;
    [SerializeField] float offDelay = 3f;
    private float timer;
    private PlayerMovement player;
    private Monster monster;

    //For Monster Functionality
    [Header("Search Nodes")]
    [SerializeField]
    [Tooltip("Nodes that the monster will move between when searching around machine")]
    private Transform node1;
    [SerializeField]
    [Tooltip("Nodes that the monster will move between when searching around machine")]
    private Transform node2;

    private SpriteRenderer spriteRenderer;

    //animation variables
    private Animator nmkrAnimator;

    void Start()
    {
        //Check if nodes are set
        if (node1 == null || node2 == null) 
        {
            Debug.LogError("Missing Nodes on NoiseMaker");
            return;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (noiseSpawner.GetState())
        {
            spriteRenderer.color = Color.red;
        }
        else
        {
            spriteRenderer.color = Color.blue;
        }
        timer = offDelay;

        nmkrAnimator = transform.Find("Sprite").gameObject.transform.GetComponent<Animator>();
        
        //Gets the Monster Script of Obj that has "Monster" tag
        try
        {
            GameObject monsterObject = GameObject.FindWithTag("Monster");

            //check if monster was found
            if (monsterObject != null)
            {
                monster = monsterObject.GetComponent<Monster>();
            }
            else
            {
                throw new System.NullReferenceException("No GameObject with 'Monster' Tag found");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error while finding Monster: " + e.Message);
        }
        //Gets the transform of Obj that has "Player" tag
        try
        {
            GameObject playerObject = GameObject.FindWithTag("Player");

            //check if player was found
            if (playerObject != null)
            {
                player = playerObject.GetComponent<PlayerMovement>();
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

    void Update()
    {
        //if machine turns off after a delay, and the machine is on
        if (turnOffAfterDelay && noiseSpawner.GetState())
        {
            //reduce timer
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                noiseSpawner.NoiseToggle();
                nmkrAnimator.SetBool("nmkrToggle", false);
            }
        }
        //if machine is turned off reset timer
        if (!noiseSpawner.GetState())
        {
            timer = offDelay;
        }
        UpdateColour();
    }

    public void Interact()
    {
        //if machine is off, turn on
        if (!noiseSpawner.GetState())
        {
            noiseSpawner.NoiseToggle();
            nmkrAnimator.SetBool("nmkrToggle", true);
        }
        //Can turn off machine if it is on
        else if (canManualTurnOff && noiseSpawner.GetState())
        {
            noiseSpawner.NoiseToggle();
            nmkrAnimator.SetBool("nmkrToggle", false);
        }
        player.SetState(1);
    }

    private void UpdateColour()
    {
        //change machine colour depending on state
        if (noiseSpawner.GetState())
        {
            spriteRenderer.color = Color.red;
        }
        else
        {
            spriteRenderer.color = Color.blue;
        }
    }

    public bool MachineIsOn()
    {
        return noiseSpawner.GetState();
    }
    
    //Run when NoiseSpawner spawns noise and plays audio
    public void PlayedNoise()
    {
        //Makes monser move towards noise machien
        monster.followNoiseMaker(this.transform, node1, node2);
    }
}
