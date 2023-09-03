using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Health healthBar;
    public int maxHealth = 100;
    private int currentHealth;

    [SerializeField] public bool ghostMode;

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
    }

    public void Heal(int amount)
    {
        healthBar.SetHealth(amount);
        // You can add visual feedback or other logic for healing here.
    }

    // Method to deduct health points.
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        // Check if the entity is dead.
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Custom method for handling death (e.g., play death animation, disable GameObject, etc.).
    private void Die()
    {
        // Implement death behavior here.
        Debug.Log(gameObject.name + " has died.");
        MenuManager.Instance.ShowGameOverMenu();
        // Example: gameObject.SetActive(false);
    }

    // Additional player-specific health-related methods can be added as needed.
}
