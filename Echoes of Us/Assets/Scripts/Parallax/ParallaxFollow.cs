using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxFollow : MonoBehaviour
{
    //Note: the parallax background is set to center itself to the player. To change height etc. scale the planes/images

    private Vector3 offset = new Vector3(0f, 0f, 20f); //Always sets the z position to 20 in world space (behind everything else)
    private float smoothTime = 0.0f; //0.2f
    private Vector3 velocity = Vector3.zero;
    private Vector3 startPos;
    private Transform player;
    private Transform cam;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Vector3 startPos = transform.position; //Start position of the parallax background
        cam = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = cam.position + offset;
        /*
        Created a Vec3 smoothedPosition just to use the y-pos because SmoothDamp() accepts Vec3 only.
        It makes the parallax background trail behind the player with delay
        */
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        transform.position = new Vector3(cam.position.x, smoothedPosition.y, offset.z);
    }
}
