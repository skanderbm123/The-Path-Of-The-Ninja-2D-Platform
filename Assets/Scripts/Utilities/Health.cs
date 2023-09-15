using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider healthSlider;
    public Gradient gradient;
    public Image healthFill;
    public Image healthBorder;
    [SerializeField] public Sprite[] healthImage;

    private float maxHealth;

    private void Update()
    {
        healthFill.color = gradient.Evaluate(healthSlider.normalizedValue);
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        healthFill.color = gradient.Evaluate(1f);

        // Set initial health image
        UpdateHealthImage();
    }

    public void SetHealth(float health)
    {
        healthSlider.value = health;
        healthFill.color = gradient.Evaluate(healthSlider.normalizedValue);

        // Update health image when health changes
        UpdateHealthImage();
    }

    private void UpdateHealthImage()
    {
        // Calculate the health percentage
        float healthPercentage = healthSlider.value / maxHealth;

        // Calculate the inverted health sprite index based on the percentage
        int healthSpriteIndex = Mathf.Clamp(Mathf.FloorToInt((1 - healthPercentage) * (healthImage.Length - 1)), 0, healthImage.Length - 1);

        // Set the health border image sprite based on the calculated index
        healthBorder.sprite = healthImage[healthSpriteIndex];
    }
}
