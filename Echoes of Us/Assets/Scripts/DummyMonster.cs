using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class DummyMonster : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    private Transform defaultStart;
    private Transform defaultEnd;

    private Transform target;   // Current target position

    void Start()
    {
        defaultStart = startPoint.transform; 
        defaultEnd = endPoint.transform;
        // Set initial target to endPoint
        target = endPoint.transform;
    }

    void Update()
    {
        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, target.position, movementSpeed * Time.deltaTime);

        // If reached the target, switch the target
        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            if (target == startPoint)
                target = endPoint.transform;
            else
                target = startPoint.transform;
        }
    }

    // Method to change the movement points dynamically
    public void ChangeMovementPoints(Transform newStartPoint, Transform newEndPoint)
    {
        startPoint = newStartPoint;
        endPoint = newEndPoint;

        target = endPoint;
    }

    public void ResetMovementPoints()
    {
        startPoint = defaultStart;
        endPoint = defaultEnd;
    }
}
