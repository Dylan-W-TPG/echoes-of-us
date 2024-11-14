using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField][Range(-20f, 20f)] private float vertFollowOffset = 0;
    [SerializeField][Range(-20f, 20f)] private float horzFollowOffset = 0;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float followDistance = 2.0f;

    private Transform playerPos;
    private PlayerMovement player;
    private Rigidbody2D playerRb;
    private Vector3 initialOffset;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    private enum CameraState
    {
        Follow,
        Free,
    }
    private CameraState state;

    // Start is called before the first frame update
    void Start()
    {
        state = CameraState.Follow;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerRb = player.GetComponent<Rigidbody2D>();
        playerPos = player.transform;
        initialOffset = transform.position - playerPos.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case CameraState.Follow:
                FollowPlayer();
                break;
            case CameraState.Free:
                //Unused for now. For camera scripted movement etc.
                break;
        }
    }

    void FollowPlayer()
    {
        // Calculate the target position with the initial offset and desired position
        Vector3 desiredPosition = player.transform.position + initialOffset + new Vector3(horzFollowOffset, vertFollowOffset, 0);

        // Add additional follow distance in the direction of the player's movement on the x-axis
        Vector3 followOffset = new Vector3(playerRb.velocity.normalized.x * followDistance, 0, 0);

        // Calculate the target position based on player movement
        targetPosition = Vector3.SmoothDamp(transform.position, desiredPosition + followOffset, ref velocity, smoothSpeed);

        // Set the camera's position to the target position, maintaining the z position
        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
    }
}
