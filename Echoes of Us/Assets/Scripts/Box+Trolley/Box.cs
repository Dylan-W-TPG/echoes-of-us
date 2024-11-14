using Newtonsoft.Json.Bson;
using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    //[SerializeField] bool isMonsterHostile = false;
    private Collider2D cldr;
    private Rigidbody2D rb;
    private Monster monster;

    private enum BoxState
    {
        Idle,
        CarriedByTrolley,
        PlayerPushing,
        MonsterPushing,
    }
    private BoxState state;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cldr = GetComponent<Collider2D>();
        GameObject monsterObject = GameObject.FindWithTag("Monster");
        monster = monsterObject.GetComponent<Monster>();
    }

    void Update()
    {
        switch (state)
        {
            case BoxState.Idle:
                rb.isKinematic = false;
                cldr.enabled = true;
                break;
            case BoxState.CarriedByTrolley:
                rb.isKinematic = true;
                break;
            case BoxState.PlayerPushing:
                //Not Moveable
                cldr.enabled = true;
                rb.isKinematic = true;
                break;
            case BoxState.MonsterPushing:
                if (monster.GetState() == 2 || monster.GetState() == 3)
                {
                    //Moveable
                    cldr.enabled = true;
                    rb.isKinematic = false;
                }
                else
                {
                    //Walk through it
                    //cldr.enabled = false;
                    //rb.isKinematic = true;
                    cldr.enabled = true;
                    rb.isKinematic = true;
                }
                break;
        }
    }

    //Called by Trolley when carried by a moving trolley
    public void CarryWithTrolley(Transform trolleyTransform)
    {
        this.transform.SetParent(trolleyTransform);
        state = BoxState.CarriedByTrolley;
    }

    public void RemoveFromTrolley()
    {
        this.transform.parent = null;
        state = BoxState.Idle;
    }

    public bool IsCarried()
    {
        return state == BoxState.CarriedByTrolley;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            state = BoxState.PlayerPushing;
        } else if(collision.tag == "Monster")
        {
            state = BoxState.MonsterPushing;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        state = BoxState.Idle;
    }
}
