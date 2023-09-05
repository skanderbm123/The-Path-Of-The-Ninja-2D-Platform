using UnityEngine;

public class Moving : MonoBehaviour
{
    public Transform[] waypoints;  // An array of waypoints the platform will move between.
    public float speed = 2.0f;    // The speed at which the platform moves.
    public float waitTime = 1.0f; // Time to wait at each waypoint.

    private int currentWaypoint = 0;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool isWaiting = false;
    private float waitStartTime;

    private void Start()
    {
        startPosition = transform.position;
        SetTargetWaypoint(0);
    }

    private void Update()
    {
        if (waypoints.Length == 0)
            return;

        if (!isWaiting)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (transform.position == targetPosition)
            {
                // Reached the current waypoint, start waiting.
                isWaiting = true;
                waitStartTime = Time.time;
            }
        }
        else
        {
            // Check if the wait time has passed.
            if (Time.time - waitStartTime >= waitTime)
            {
                // Finished waiting, move to the next waypoint.
                isWaiting = false;
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                SetTargetWaypoint(currentWaypoint);
            }
        }
    }

    private void SetTargetWaypoint(int index)
    {
        targetPosition = waypoints[index].position;
    }
}
