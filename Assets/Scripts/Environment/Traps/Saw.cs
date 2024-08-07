using UnityEngine;
using System.Collections;

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
        if (collision.gameObject.CompareTag("Player"))
        {
            // Assuming the player has a health component, you can damage the player here.
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null && !playerHealth.Data.isInvulnerable)
            {
                // Apply knockback to the player.
                Vector2 knockbackDirection = (collision.gameObject.transform.position - transform.position).normalized;
                // Apply the modified knockback force.
                Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRigidbody != null)
                {
                    playerRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                    playerHealth.TakeDamage(damageAmount, transform.position); // Adjust 'damageAmount' as needed.
                }
            }
        }
    }
}
