using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    //[SerializeField] bool isMonsterHostile = false;
    private bool wallBroken = false;
    private Collider2D cldr;
    private SpriteRenderer wallSprite;
    [SerializeField] Sprite brokenWallSprite;
    private Monster monster;
    private int Layer;
    private GameObject gameobject;
    [SerializeField] private AudioClip glassSmash;
    private AudioSource audioSource;

    //animation variables
    private Animator wallAnimator;

    void Start()
    {
        cldr = GetComponent<Collider2D>();
        wallSprite = GetComponent<SpriteRenderer>();
        GameObject monsterObject = GameObject.FindWithTag("Monster");
        monster = monsterObject.GetComponent<Monster>();
        //animation
        wallAnimator = transform.Find("Sprite").gameObject.transform.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in Breakable Wall");
        }
    }

    void Update()
    {
        if (monster.GetState() == 2 || monster.GetState() == 3)
        {
            if (!wallBroken)
            {
                Layer = LayerMask.NameToLayer("Default");
            }
        } else
        {
            if(!wallBroken)
            {
                Layer = LayerMask.NameToLayer("WallGround");
            }
        }
        gameObject.layer = Layer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !wallBroken)
        {
            CantWalkThrough();
        }
        if (collision.tag == "Monster" && !wallBroken)
        {
            if (monster.GetState() == 2 || monster.GetState() == 3)
            {
                Break();
            }
            else
            {
                CantWalkThrough();
            }
        }
    }

    private void CantWalkThrough()
    {
        cldr.enabled = true;
    }

    private void Break()
    {
        cldr.enabled = false;
        wallBroken = true;
        /*
        wallSprite.sprite = brokenWallSprite;
        //Adjust these values to position the broken wall sprite (its default floating in the middle of the game object)
        wallSprite.transform.position = new Vector3(0.5f, 1, 0);
        */
        Layer = LayerMask.NameToLayer("Default");
        gameObject.layer = Layer;
        audioSource.PlayOneShot(glassSmash);

        wallAnimator.SetTrigger("breakWall");
    }

}
