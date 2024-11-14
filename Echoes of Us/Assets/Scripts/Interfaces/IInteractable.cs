using UnityEngine;

public interface IInteractable
{
    void Interact()
    {
    /*Remember to set playerState using {player}.SetState
    SetState(0) == OnGround,
    SetState(1) == Jumping,
    SetState(2) == Interacting,
    SetState(3) == Hiding,
    State is Interacting by default.
    */
    }
}
