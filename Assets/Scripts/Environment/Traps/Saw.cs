using UnityEngine;

public class Saw : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private float knockbackForce = 5.0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // This method should be called when the first animation finishes.
    public void SawEnabled()
    {
        // Set the boolean to true.
        animator.SetBool("isEnabled", true);

        // Enable the box collider (assuming your GameObject has a Collider2D component).
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Assuming the player has a health component, you can damage the player here.
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.isInvulnerable)
            {
                playerHealth.TakeDamage(damageAmount); // Adjust 'damageAmount' as needed.

                // Apply knockback to the player.
                Rigidbody2D playerRigidbody = collision.GetComponent<Rigidbody2D>();
                if (playerRigidbody != null)
                {
                    // Calculate the direction away from the saw and apply force.
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    playerRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
