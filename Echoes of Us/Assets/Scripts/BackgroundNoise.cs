using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundNoise : MonoBehaviour
{
    [SerializeField] private AudioClip monsterChase;
    [SerializeField] private AudioClip ambient;
    private Monster monster;
    private AudioSource source;
    private bool ambientPlayOnce = true;
    private bool monsterPlayOnce = false;
    // Start is called before the first frame update

    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
        GameObject monsterObject = GameObject.FindWithTag("Monster");
        monster = monsterObject.GetComponent<Monster>();
    }

    public void PlayBackground()
    {
        ambientPlayOnce = false;
        monsterPlayOnce = true;
        source.Stop();
        source.loop = true;
        source.clip = ambient;
        source.Play();
    }
    public void PlayMonster()
    {
        ambientPlayOnce = true;
        monsterPlayOnce = false;
        source.Stop();
        source.loop = true;
        source.clip = monsterChase;
        source.time = 1f;
        source.Play();
    }

    void Update()
    {
        if (monster.GetState() == 2 || monster.GetState() == 3)
        {
            if(monsterPlayOnce)
            {
                PlayMonster();
            }
        } else if(ambientPlayOnce)
        {
            PlayBackground();
        }
    }
}
