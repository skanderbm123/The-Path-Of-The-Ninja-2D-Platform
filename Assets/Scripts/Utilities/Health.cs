using UnityEngine;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    public Slider healthSlider;
    public Gradient gradient;
    public Image healthFill;


    private void Update()
    {
        healthFill.color = gradient.Evaluate(healthSlider.normalizedValue);
    }

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        healthFill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        healthSlider.value = health;
        healthFill.color = gradient.Evaluate(healthSlider.normalizedValue);
    }
}
