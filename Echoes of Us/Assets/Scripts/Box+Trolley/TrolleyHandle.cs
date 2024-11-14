using UnityEngine;

public class TrolleyHandle : MonoBehaviour, IInteractable
{
    private Trolley parentScript;

    void Start()
    {
        parentScript = GetComponentInParent<Trolley>();
    }
    public void Interact()
    {
        Debug.Log("TROLLEY INTERACT");
        PlayerMovement player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        //Set player state to moving trolley
        player.SetState(4);
        ToggleMove();
    }    
    private void ToggleMove()
    {
        parentScript.ToggleMoveable();
    }
}
