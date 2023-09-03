using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] waypoints;  // An array of waypoints the platform will move between.
    public float speed = 2.0f;    // The speed at which the platform moves.
    public float lerpTime = 2.0f; // Time it takes to interpolate between waypoints.
    private int currentWaypoint = 0;
    private int direction = 1;     // Direction of movement, 1 for forward, -1 for backward.
    private bool playerOnPlatform = false; // Flag to track if the player is on the platform.
    private new Rigidbody2D rigidbody;
    private float lerpStartTime;
    private Vector2 startPosition;
    private Vector2 targetPosition;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic; // Initially, the platform is kinematic.
        lerpStartTime = Time.time;
        startPosition = transform.position;
        targetPosition = waypoints[currentWaypoint].position;
    }

    private void Update()
    {
        if (waypoints.Length == 0)
            return;

        float lerpProgress = (Time.time - lerpStartTime) / lerpTime;

        // Use Mathf.Lerp to interpolate between waypoints.
        transform.position = Vector2.Lerp(startPosition, targetPosition, lerpProgress);

        // Check if the platform has reached the current waypoint.
        if (lerpProgress >= 1.0f)
        {
            currentWaypoint += direction;

            // Check if we've reached the end of the waypoints array.
            if (currentWaypoint >= waypoints.Length || currentWaypoint < 0)
            {
                direction *= -1; // Reverse the direction.
                currentWaypoint += direction; // Move to the next waypoint.
            }

            // Update lerp start time and positions for the next interpolation.
            lerpStartTime = Time.time;
            startPosition = transform.position;
            targetPosition = waypoints[currentWaypoint].position;
        }
    }

    private void FixedUpdate()
    {
        // Apply a force to the platform to move it.
        if (rigidbody != null && playerOnPlatform)
        {
            float horizontalForce = direction * (speed / lerpTime); // Adjust force for lerp time.
            rigidbody.AddForce(new Vector2(horizontalForce, 0f));
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!playerOnPlatform && collision.gameObject.GetComponent<PlayerMovement>().horizontal == 0f)
            {
                // Make the player's transform a child of the platform's transform.
                collision.transform.SetParent(transform);
                playerOnPlatform = true; // Set the flag to true.
            }
            // Check conditions during continuous collision
            if (playerOnPlatform && collision.gameObject.GetComponent<PlayerMovement>().horizontal != 0f)
            {
                collision.transform.SetParent(null);
                playerOnPlatform = false; // Set the flag to false.
            }
        }
    }

    // This method is called when another Collider2D exits the trigger zone.
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Unparent the player's transform from the platform's transform.
            collision.transform.SetParent(null);
            playerOnPlatform = false; // Set the flag to false.
        }
    }
}
