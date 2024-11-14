using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinishPoint : MonoBehaviour, IInteractable
{
    [SerializeField] string levelName;
    private bool playerExists = false;
    /*
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //SceneController.Instance.NextLevel();
            SceneController.Instance.LoadScene(levelName);
        }
    }
    */
    
    void Start()
    {
        //Shamelessly nicked this try catch from Matthews code - all credit to them
        try
        {
            GameObject playerObject = GameObject.FindWithTag("Player");

            //check if player was found
            if (playerObject != null)
            {
                playerExists = true;
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
        //SceneController.Instance.LoadScene(levelName);
        //Debug.Log("Player Exists!");
        if (playerExists)
        {
            Debug.Log("Player Exists!");
            SceneController.Instance.LoadScene(levelName);
        }
        
    }
    
}
