using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    #region Interact Settings
    public InputActionReference interact;
    #endregion

    private GameObject currentTeleporter;
    private bool isInteracting = false;
    private float cooldownPortalTimer = 0f;
    [SerializeField] private float cooldownPortalDuration = 1f; // Adjust this to set the cooldown duration in seconds.

    void Update()
    {
        if (cooldownPortalTimer > 0f)
        {
            cooldownPortalTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            currentTeleporter = collision.gameObject;
            if (interact.action.triggered && !isInteracting && cooldownPortalTimer <= 0f)
            {
                if (currentTeleporter != null)
                {
                    Vector3 destinationPosition = currentTeleporter.GetComponent<Portal>().GetDestination().position;

                    // Only change the x and y, keep the current z position
                    transform.position = new Vector3(destinationPosition.x, destinationPosition.y, transform.position.z);

                    isInteracting = true;
                    cooldownPortalTimer = cooldownPortalDuration; // Start the cooldown timer.
                }
            }
            else if (!interact.action.triggered)
            {
                isInteracting = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            if (collision.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
            }
        }
    }
}
