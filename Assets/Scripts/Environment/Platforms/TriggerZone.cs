using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    // This method is called when another Collider2D enters the trigger zone.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger zone is the player.
        if (other.CompareTag("Player"))
        {
            // Make the player's transform a child of the platform's transform.
            other.transform.SetParent(transform);
        }
    }

    // This method is called when another Collider2D exits the trigger zone.
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object that exited the trigger zone is the player.
        if (other.CompareTag("Player"))
        {
            // Unparent the player's transform from the platform's transform.
            other.transform.SetParent(null);
        }
    }
}
