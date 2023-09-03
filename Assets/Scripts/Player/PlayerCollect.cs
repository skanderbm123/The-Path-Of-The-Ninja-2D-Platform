using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollect : MonoBehaviour
{
    private Dictionary<string, int> collectedItems = new Dictionary<string, int>();

    private void Start()
    {
        // Initialize the collected items dictionary with item types and counts.
        collectedItems.Add("Coin", 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collectible"))
        {
            Collectible collectible = collision.GetComponent<Collectible>();
            if (collectible != null)
            {
                Collect(collectible);
            }
        }
    }

    private void Collect(Collectible collectible)
    {
        string itemType = collectible.GetItemType();

        if (collectedItems.ContainsKey(itemType))
        {
            // Increment the count of collected items for this type.
            collectedItems[itemType]++;
            Debug.Log($"Collected a {itemType}, Count: {collectedItems[itemType]}");

            // Disable the collider to prevent further interactions.
            collectible.GetComponent<Collider2D>().enabled = false;

            // Start a coroutine to move the collectible upward before destroying it.
            StartCoroutine(FloatingCollectible(collectible.transform));

            // Handle specific collectible behavior (e.g., increase score, heal, etc.).
            collectible.Collect();
        }
    }

    private IEnumerator FloatingCollectible(Transform collectibleTransform)
    {
        float duration = 0.1f; // Adjust the duration as needed.
        float elapsedTime = 0f;
        Vector3 initialPosition = collectibleTransform.position;
        Vector3 targetPosition = initialPosition + Vector3.up * 1.5f; // Adjust the height as needed.

        while (elapsedTime < duration)
        {
            collectibleTransform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the collectible reaches the target position.
        collectibleTransform.position = targetPosition;

        // Destroy the collectible after a short delay.
        yield return new WaitForSeconds(0.1f); // Adjust the delay as needed.
        Destroy(collectibleTransform.gameObject);
    }
}
