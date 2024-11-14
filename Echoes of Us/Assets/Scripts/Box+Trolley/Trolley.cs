using System.Collections.Generic;
using UnityEngine;

public class Trolley : MonoBehaviour
{
    private Transform playerTransform;
    private enum TrolleyState
    {
        Default,
        PlayerMoving,
    }
    private TrolleyState state;

    //List with all boxes touching the trolley
    private List<Box> boxesInContact = new List<Box>();

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        foreach (var box in boxesInContact)
        {
            //Debug.Log(box.ToString());
        }
    }
    //Toggles between being a child of player or not, done when the Interact() method on the TrolleyHandle child is called.
    public void ToggleMoveable()
    {
        switch (state)
        {
            //If trolley is stationary, then change it to be a child of the player
            case TrolleyState.Default:
                this.transform.SetParent(playerTransform);
                state = TrolleyState.PlayerMoving;
                //Sets boxes as children to move with trolley
                CarryAllBoxes();
                break;
            //If trolley is a child of player, remove it as a child
            case TrolleyState.PlayerMoving:
                this.transform.parent = null;
                state = TrolleyState.Default;
                //Removes boxes as children to move indapendently
                RemoveBoxChildren();
                break;

        }
    }
    //Adds a box to the list when it collides with the trolley
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Start with: " + collision.gameObject.name);
        if (collision.gameObject.TryGetComponent<Box>(out var box))
        {
            boxesInContact.Add(box);
            Debug.Log(box.name + " added to List");
        }
    }

    //Removes a box from the list when it stops colliding with the trolley
    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Collision End with: " + collision.gameObject.name);
        if (collision.gameObject.TryGetComponent<Box>(out var box))
        {
            boxesInContact.Remove(box);
        }
    }

    //Sets all boxes in List boxesInContact as children of the Trolley
    private void CarryAllBoxes()
    {
        foreach (Box box in boxesInContact)
        {
            box.CarryWithTrolley(transform);
        }
    }

    //Removes all boxes as children from the trolley
    private void RemoveBoxChildren()
    {
        foreach (Box box in boxesInContact)
        {
            box.RemoveFromTrolley();
        }
    }

}