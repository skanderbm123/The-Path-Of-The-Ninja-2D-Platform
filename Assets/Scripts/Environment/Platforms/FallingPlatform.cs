using System.Collections;
using UnityEngine;

public class FallingPlatform : ObjectStateTracker
{
    public float fallDelay = 0.6f; // Time in seconds to wait before falling
    public float destroyDelay = 3.0f; // Time in seconds to destroy the platform after falling
    public LayerMask collisionLayer; // Layer mask to determine what the platform should collide with

    private bool canFall = false; // Indicates whether the platform can fall
    private Rigidbody2D rb;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool initialActiveState;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static; // Initially, the platform is static.

        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialActiveState = gameObject.activeSelf;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is from the top (negative y direction) and is with an object tagged as "Player"
        if (collision.contacts[0].normal.y < 0 && collision.gameObject.CompareTag("Player"))
        {
            canFall = true;
            // Start a timer to make the platform fall after the delay
            Invoke("StartFalling", fallDelay);
        }
        // Check if the collision is with an object on the specified collisionLayer
        if (collisionLayer == (collisionLayer | (1 << collision.gameObject.layer)) && canFall)
        {
            // Allow the platform to pass through the object by disabling its collider
            rb.bodyType = RigidbodyType2D.Dynamic; // Allow the platform to fall.
            GetComponent<Collider2D>().enabled = false;
            Invoke("DeactivateAfterDelay", 0f);
        }
    }

    private void StartFalling()
    {
        if (canFall)
        {
            // Implement the falling behavior here
            rb.bodyType = RigidbodyType2D.Dynamic; // Allow the platform to fall.

            // Destroy the platform after a certain time
            StartCoroutine(DeactivateAfterDelay(destroyDelay));
        }
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    public override void ResetToInitialState()
    {
        // Reset position, rotation, and active state
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        gameObject.SetActive(initialActiveState);

        // Reset Rigidbody2D properties
        rb.bodyType = RigidbodyType2D.Static;  // Reset to initial body type
        rb.velocity = Vector2.zero;  // Reset any applied velocity
        rb.angularVelocity = 0f;     // Reset any applied angular velocity

        // Re-enable colliders
        GetComponent<Collider2D>().enabled = true;

        // Reset custom flags or state variables
        canFall = false;

        // Optionally, cancel any pending Invokes or Coroutines to prevent unintended behavior
        CancelInvoke("StartFalling");
        StopAllCoroutines();
    }

}
