using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public Health healthBar;
    public int maxHealth = 100;
    [Range(0, 100)] private int currentHealth;

    [SerializeField] public bool ghostMode;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;
    public bool isInvulnerable = false; // Track if the player is invulnerable.

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
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        Debug.Log("here");
        if (currentHealth > 0)
        {
            StartCoroutine(Invulnerability());
        }
        else
        {
            // Check if the entity is dead.
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator Invulnerability()
    {
        isInvulnerable = true; // Set the player as invulnerable during this time.
        Physics2D.IgnoreLayerCollision(10, 11, true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }

        Physics2D.IgnoreLayerCollision(10, 11, false);
        isInvulnerable = false; // Reset invulnerability when the time is up.
    }

    // Custom method for handling death (e.g., play death animation, disable GameObject, etc.).
    private void Die()
    {
        // Implement death behavior here.
        Debug.Log(gameObject.name + " has died.");
        // Die animation
        this.gameObject.SetActive(false);
        MenuManager.Instance.ShowGameOverMenu();

        // Example: gameObject.SetActive(false);
    }

    // Additional player-specific health-related methods can be added as needed.
}
