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
    private float cooldownTimer = 0f;
    [SerializeField] private float cooldownDuration = 1f; // Adjust this to set the cooldown duration in seconds.

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (interact.action.triggered && !isInteracting && cooldownTimer <= 0f)
        {
            if (currentTeleporter != null)
            {
                transform.position = currentTeleporter.GetComponent<Portal>().GetDestination().position;
                isInteracting = true;
                cooldownTimer = cooldownDuration; // Start the cooldown timer.
            }
        }
        else if (!interact.action.triggered)
        {
            isInteracting = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Portal"))
        {
            currentTeleporter = collision.gameObject;
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
