using System.Collections;
using UnityEngine;

public class Bub : ObjectStateTracker
{
    public float jumpForce = 10f;
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;

    private Animator animator;
    private bool isDead = false;
    private bool isFacingRight = false; // Track the enemy's facing direction
    private int currentPatrolPointIndex = 0;

    [SerializeField] private int damageAmount = 1; // Damage amount when hitting the player
    [SerializeField] private BoxCollider2D killBox;
    [SerializeField] private BoxCollider2D hurtBox;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool initialActiveState;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // Store the initial state of the GameObject
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialActiveState = gameObject.activeSelf;
    }

    private void Update()
    {
        if (!isDead)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        Vector3 targetPosition = patrolPoints[currentPatrolPointIndex].position;
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        // Move towards the current patrol point
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // If reached the patrol point, change to the next one
        if (distanceToTarget < 0.1f)
        {
            currentPatrolPointIndex = (currentPatrolPointIndex + 1) % patrolPoints.Length;
        }

        // Flip the enemy's sprite based on movement direction
        if (targetPosition.x > transform.position.x && !isFacingRight)
        {
            Flip();
        }
        else if (targetPosition.x < transform.position.x && isFacingRight)
        {
            Flip();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
            return;

        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if the player collided with the "kill box" collider
            if (collision.otherCollider == killBox)
            {
                // Play hurt and death animation
                animator.SetBool("isHurt", true);
                animator.SetBool("isDead", true);
                isDead = true;
                GetComponent<BoxCollider2D>().enabled = false;

                // Apply a vertical force to the player for jumping
                Rigidbody2D playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRB != null)
                {
                    playerRB.velocity = new Vector2(playerRB.velocity.x, jumpForce);
                }

                // Deactivate the enemy GameObject after a delay (for death animation, etc.)
                StartCoroutine(DeactivateAfterDelay(0.8f));
            }
            // Check if the player collided with the "hurt box" collider
            else if (collision.otherCollider == hurtBox)
            {
                // Damage the player and apply knockback
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // Pass the position of the enemy as the damage source
                    playerHealth.TakeDamage(damageAmount, transform.position);
                }
            }
        }
    }

    private void Flip()
    {
        // Switch the direction the enemy is facing
        isFacingRight = !isFacingRight;

        // Flip the enemy's sprite
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    // Method to reset the enemy to its initial state
    public override void ResetToInitialState()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        isDead = false;
        animator.SetBool("isHurt", false);
        animator.SetBool("isDead", false);
        GetComponent<BoxCollider2D>().enabled = true;
        gameObject.SetActive(initialActiveState);
    }
}
