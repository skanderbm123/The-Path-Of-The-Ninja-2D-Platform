using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 0.6f; // Time in seconds to wait before falling
    public float destroyDelay = 3.0f; // Time in seconds to destroy the platform after falling
    public LayerMask collisionLayer; // Layer mask to determine what the platform should collide with

    private bool canFall = false; // Indicates whether the platform can fall
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static; // Initially, the platform is static.
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
            Invoke("DestroyPlatform", 0f);
        }
    }

    private void StartFalling()
    {
        if (canFall)
        {
            // Implement the falling behavior here
            rb.bodyType = RigidbodyType2D.Dynamic; // Allow the platform to fall.

            // Destroy the platform after a certain time
            Destroy(gameObject, destroyDelay);
        }
    }

    private void DestroyPlatform()
    {
        Destroy(gameObject);
    }
}
