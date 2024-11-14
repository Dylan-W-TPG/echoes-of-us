using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NoiseMakerGraphic : MonoBehaviour
{
    private Transform playerTransform;
    private float moveSpeed;
    private float shrinkRate;
    private bool displayWhenSourceInView;
    private Transform parentTransform;
    SpriteRenderer spriteRenderer;

    public void SetNoiseParameters(Transform player, float speed, float rate, bool displaySetting, Transform parentTransform_)
    {
        playerTransform = player;
        moveSpeed = speed;
        shrinkRate = rate;
        displayWhenSourceInView = displaySetting;
        parentTransform = parentTransform_;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        Vector2 sourceViewportPos = Camera.main.WorldToViewportPoint(parentTransform.position);

        //Don't display graphic if source is within view
        Debug.Log(sourceViewportPos);
        Debug.Log(displayWhenSourceInView);
        if (!displayWhenSourceInView)
        {
            if (sourceViewportPos.x > 0 && sourceViewportPos.x < 1 &&
            sourceViewportPos.y > 0 && sourceViewportPos.y < 1)
            {
                spriteRenderer.enabled = false;
            }
            else
            {
                spriteRenderer.enabled = true;
            }
        }
    }

    void Update()
    {
        // Move towards the player
        transform.position = Vector3.MoveTowards(this.transform.position, playerTransform.position , moveSpeed * Time.deltaTime);

        // Shrink over time
        ShrinkOverTime();
    }

    void ShrinkOverTime()
    {
        // Shrink the noise graphic over time
        transform.localScale -= new Vector3(shrinkRate, shrinkRate, 0f) * Time.deltaTime;

        // Destroy the noise graphic once it's small enough
        if (transform.localScale.x <= 0.1f)
        {
            Destroy(gameObject);
        }

    }
}
