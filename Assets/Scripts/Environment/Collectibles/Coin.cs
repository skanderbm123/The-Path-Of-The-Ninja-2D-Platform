using UnityEngine;

public class Coin : Collectible
{
    // Constructor to set the item type for coins.
    public Coin()
    {
        itemType = "Coin";
    }

    // Implement behavior for collecting coins (e.g., increase score).
    public override void Collect()
    {
        // Add your coin collection logic here.
        Debug.Log("Collected a " + itemType);

        // Example: Increment the player's score.
        // GameManager.Instance.IncreaseScore(1);
    }
}
