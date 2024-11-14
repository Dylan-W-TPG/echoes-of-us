using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    Transform cam;
    Vector3 camStartPos;
    Material mat;
    float distanceX;
    [Range(0f,5f)] public float speed = 0.2f;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        cam = Camera.main.transform;
        camStartPos = cam.position;
        speed = speed * 0.005f;
    }

    void Update()
    {
        //Distance determins if the background moves left or right (+ / -)
        distanceX = cam.position.x - camStartPos.x;
        //Moves the image
        mat.SetTextureOffset("_MainTex", new Vector2(distanceX, 0) * speed);
    }
}
