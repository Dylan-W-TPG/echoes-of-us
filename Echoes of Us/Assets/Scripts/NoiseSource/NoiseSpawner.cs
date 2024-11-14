using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NoiseSpawner : MonoBehaviour
{
    [SerializeField] private GameObject noisePrefab;
    [SerializeField] private bool displayWhenOnScreen = false;
    [SerializeField] private bool startActive = false;
    [SerializeField][Range(2, 10f)] private float distanceFromPlayer;
    [SerializeField] private float spawnInterval = 2.5f;
    [SerializeField][Range(0f, 5f)] private float moveSpeed = 2f;
    [SerializeField][Range(1f, 10f)] private float shrinkRate = 0.1f;
    private Transform playerTransform;
    private AudioSource audioSource;

    //For using NoiseMachine methods
    [SerializeField] private NoiseMachineHandler noiseMachineHandler;

    //Tracking if on or off
    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        //guard if noiseMachineHandler not set
        if (noiseMachineHandler == null)
        {
            Debug.LogError("Noise Machine Handler field not set, in NoiseMachine Prefab!");
            return;
        }

        //Gets the transform of Obj that has "Player" tag
        try
        {
            GameObject playerObject = GameObject.FindWithTag("Player");

            //check if player was found
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                throw new System.NullReferenceException("No GameObject with 'Player' Tag found");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error while finding player: " + e.Message);
        }

        //Get audio source + Error checking
        try
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource.clip == null)
            {
                Debug.Log("AudioSoure on NoiseSpawner doesn't have attached Audio clip");
            }
        }
        catch (Exception e)
        {
            Debug.Log("An error has occurred while accessing Audio Source component: " + e.Message);
        }

        if (startActive)
        {
            //Start playing audio loop
            StartCoroutine(SpawnNoise(spawnInterval, noisePrefab, distanceFromPlayer, this.transform, playerTransform, moveSpeed, shrinkRate, displayWhenOnScreen, audioSource, noiseMachineHandler));
        }

        isActive = startActive;
    }

    void Update()
    {
        //null check
        if (playerTransform == null || noisePrefab == null)
        {
            Debug.Log("Missing field NoiseSource");
            return;
        }
    }

    private IEnumerator SpawnNoise(float interval, GameObject noisePrefab, float distanceFromPlayer, Transform parentTransform, Transform playerTransform, float moveSpeed, float shrinkRate, bool displayWhenOnScreen, AudioSource audioSource, NoiseMachineHandler noiseMachineHandler)
    {
        yield return new WaitForSeconds(interval);
        //play Sound
        audioSource.Play();

        //Tell Noise Machine Handler that noise is being played
        noiseMachineHandler.PlayedNoise();

        // Calculate the direction from the player to the spawner
        Vector3 directionToNoiseSpawner = (parentTransform.position - playerTransform.position).normalized;

        // Calculate the spawn position a set distance from the player in the direction of the NoiseSpawner
        Vector3 spawnPosition = playerTransform.position + directionToNoiseSpawner * distanceFromPlayer;

        //Calculate rotation to face Player
        Vector3 playerDirection = (playerTransform.position - spawnPosition).normalized;
        float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg - 90;
        Quaternion spawnAngle = Quaternion.Euler(0, 0, angle);

        //Spawn new object & Set its parameters
        GameObject newNoise = Instantiate(noisePrefab, spawnPosition, spawnAngle);
        newNoise.GetComponent<NoiseMakerGraphic>().SetNoiseParameters(playerTransform, moveSpeed, shrinkRate, displayWhenOnScreen, parentTransform);

        StartCoroutine(SpawnNoise(interval, noisePrefab, distanceFromPlayer, parentTransform, playerTransform, moveSpeed, shrinkRate, displayWhenOnScreen, audioSource, noiseMachineHandler));
    }

    public void NoiseStart()
    {
        isActive = true;
        //Start playing audio loop
        StartCoroutine(SpawnNoise(spawnInterval, noisePrefab, distanceFromPlayer, this.transform, playerTransform, moveSpeed, shrinkRate, displayWhenOnScreen, audioSource, noiseMachineHandler));
    }

    public void NoiseStop()
    {
        isActive = false;
        //Stop playing AUdio Loop
        StopAllCoroutines();
    }

    //Toggles on/off when run
    public void NoiseToggle()
    {
        if (isActive)
        {
            NoiseStop();
        }
        else
        {
            NoiseStart();
        }
    }

    //Returns if noisemaker is running or not
    public bool GetState()
    {
        return isActive;
    }
}
