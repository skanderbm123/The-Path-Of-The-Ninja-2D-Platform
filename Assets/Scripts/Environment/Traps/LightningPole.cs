using UnityEngine;

public class LightningPole : MonoBehaviour
{
    private Animator animator;
    private BoxCollider2D boxCollider;

    [Header("Timers")]
    [SerializeField] private float activationTime = 2.0f;         // Time for the trap to become active.
    [SerializeField] private float deadlyTime = 5.0f;             // Time the trap remains active and deadly.
    [SerializeField] private float deactivationTime = 2.0f;       // Time for the trap to deactivate and reset.
    [SerializeField] private float restartTime = 5.0f;            // Time before restarting the first animation.

    [Header("Damage")]
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private float knockbackForce = 5.0f;

    private bool isActive = false;  // Is the trap currently active?
    private bool isOver = false;    // Is the trap's cycle complete?

    private float initialActivationTime;
    private float initialDeadlyTime;
    private float initialDeactivationTime;
    private float initialRestartTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        // Store the initial values.
        initialActivationTime = activationTime;
        initialDeadlyTime = deadlyTime;
        initialDeactivationTime = deactivationTime;
        initialRestartTime = restartTime;

        // Start the first animation when the object is enabled.
        animator.SetBool("isActive", true);
    }

    private void Update()
    {
        if (isOver)
        {
            // Count down to restart the first animation.
            restartTime -= Time.deltaTime;

            if (restartTime <= 0.0f)
            {
                // Reset the trap for the next activation.
                isActive = false;
                isOver = false;
                restartTime = initialRestartTime;
                activationTime = initialActivationTime;
                deadlyTime = initialDeadlyTime;
                deactivationTime = initialDeactivationTime;

                // Set the animator parameter to restart the first animation.
                animator.SetBool("isActive", true);
                animator.SetBool("isIdle", false); // Set isIdle to true at the end of the cycle.

            }
        }
        else if (!isActive)
        {
            // Trap is inactive, count down to activation.
            activationTime -= Time.deltaTime;

            if (activationTime <= 0.0f)
            {
                // Activate the trap.
                isActive = true;
                boxCollider.enabled = true;
                // Set the animator parameter to start the second animation.
                animator.SetBool("isActive", true);
            }
        }
        else if (isActive && deadlyTime > 0.0f)
        {
            // Trap is active and deadly, count down the deadly time.
            deadlyTime -= Time.deltaTime;

            if (deadlyTime <= 0.0f)
            {
                // Deactivate the trap.
                boxCollider.enabled = false; // Disable the box collider as a trigger.

                // Set the animator parameter to start the final animation.
                animator.SetBool("isActive", false);
                animator.SetBool("isOver", true);
            }
        }
        else
        {
            // Trap is in the deactivation phase, count down to reset.
            deactivationTime -= Time.deltaTime;

            if (deactivationTime <= 0.0f)
            {
                // Reset the trap for the next activation.
                isOver = true;

                // Set the animator parameter to indicate the cycle is over.
                animator.SetBool("isOver", false);
                animator.SetBool("isIdle", true); // Set isIdle to true at the end of the cycle.
            }
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
