using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemType; // Type of collectible, e.g., "Coin", "Potion", etc.

    // Implement specific behavior for each collectible type in derived classes.
    public virtual void Collect()
    {
        // Default behavior when collected.
        Debug.Log("Collected a " + itemType);
    }

    public virtual string GetItemType()
    {
        return itemType;
    }
}
