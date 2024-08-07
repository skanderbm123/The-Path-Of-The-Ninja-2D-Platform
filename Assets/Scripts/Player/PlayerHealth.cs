using System.Collections;
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
    private bool isInputBlocked = false; // Track if player input is blocked.

    // Knockback parameters
    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.5f;

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
    }

    public void RestoreFullHeal()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    // Method to deduct health points.
    public void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        if (!Data.isInvulnerable)
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);

            // Apply knockback based on the damage source position
            ApplyKnockback(damageSourcePosition);
            StartCoroutine(BlockPlayerInput(knockbackDuration));

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

    private void ApplyKnockback(Vector2 damageSourcePosition)
    {
        // Calculate the direction from the damage source to the player
        Vector2 knockbackDirection = (rigidbody.position - damageSourcePosition).normalized;

        // Apply the knockback force to the player
        rigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }

    private IEnumerator BlockPlayerInput(float duration)
    {
        isInputBlocked = true;

        yield return new WaitForSeconds(duration);

        isInputBlocked = false;
    }

    private IEnumerator Invulnerability()
    {
        Data.isInvulnerable = true;
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = Color.red;
            yield return new WaitForSecondsRealtime(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSecondsRealtime(iFramesDuration / (numberOfFlashes * 2));
        }

        Data.isInvulnerable = false;
    }

    private IEnumerator DamageSequence(float duration, float timeScale)
    {
        if (isSlowMo)
            yield break;

        Time.timeScale = timeScale;
        isSlowMo = true;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1.0f;
        isSlowMo = false;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        foreach (Behaviour component in components)
            component.enabled = false;
        gameObject.SetActive(false);
        MenuManager.Instance.ShowGameOverMenu();
    }
}
