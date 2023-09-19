using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Health healthBar;
    public PlayerData Data;
    private new Rigidbody2D rigidbody;

    public int maxHealth = 4;
    [Range(0, 100)] private int currentHealth;

    [SerializeField] public bool ghostMode;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    [SerializeField] private Behaviour[] components;

    private SpriteRenderer spriteRend;
    public bool isSlowMo = false; // Track if the slow motion is done.

    // Singleton instance reference.
    public static PlayerHealth Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(currentHealth);

        spriteRend = GetComponent<SpriteRenderer>();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        healthBar.SetHealth(amount);
        // You can add visual feedback or other logic for healing here.
    }

    public void RestoreFullHeal()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    // Method to deduct health points.
    public void TakeDamage(int damage)
    {
        if (!Data.isInvulnerable)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            if (currentHealth > 0)
            {
                StartCoroutine(Invulnerability());
                StartCoroutine(DamageSequence(0.07f, 0f));
            }
            else
            {
                rigidbody.velocity = Vector3.zero;
                StartCoroutine(DamageSequence(2f, 0.3f));
            }
        }
    }

    private IEnumerator Invulnerability()
    {
        Data.isInvulnerable = true; // Set the player as invulnerable during this time.
        for (int i = 0; i < numberOfFlashes; i++)
        {

            spriteRend.color = Color.red;
            yield return new WaitForSecondsRealtime(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSecondsRealtime(iFramesDuration / (numberOfFlashes * 2));
        }

        Data.isInvulnerable = false; // Reset invulnerability when the time is up.
    }

    private IEnumerator DamageSequence(float duration, float timeScale)
    {
        if (isSlowMo)
            yield break; // Exit if already in slow motion.

        Time.timeScale = timeScale;
        isSlowMo = true;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1.0f;
        isSlowMo = false;

        // Check if the entity is dead.
        if (currentHealth <= 0 && !ghostMode)
        {
            Die();
        }
    }

    private void Die()
    {
        // Implement death behavior here.
        Debug.Log(gameObject.name + " has died.");
        // Die animation
        foreach (Behaviour component in components)
            component.enabled = false;
        gameObject.SetActive(false);
        MenuManager.Instance.ShowGameOverMenu();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trap") && !Data.isInvulnerable)
        {
            // Trigger the knockback effect.
            Data.isKnockbackActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            // End the knockback state when exiting the hazard zone.
            Data.isKnockbackActive = false;
        }
    }

    // Additional player-specific health-related methods can be added as needed.
}
