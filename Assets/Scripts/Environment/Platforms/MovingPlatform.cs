using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private bool playerOnPlatform = false; // Flag to track if the player is on the platform.

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!playerOnPlatform && collision.gameObject.GetComponent<PlayerMovement>().horizontal == 0f)
            {
                // Make the player's transform a child of the platform's transform.
                collision.transform.SetParent(transform);
                playerOnPlatform = true; // Set the flag to true.
            }
            // Check conditions during continuous collision
            if (playerOnPlatform && collision.gameObject.GetComponent<PlayerMovement>().horizontal != 0f)
            {
                collision.transform.SetParent(null);
                playerOnPlatform = false; // Set the flag to false.
            }
        }
    }

    // This method is called when another Collider2D exits the trigger zone.
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Unparent the player's transform from the platform's transform.
            collision.transform.SetParent(null);
            playerOnPlatform = false; // Set the flag to false.
        }
    }
}
