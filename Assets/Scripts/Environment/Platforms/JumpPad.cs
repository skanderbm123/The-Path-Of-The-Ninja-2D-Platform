using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float bounce = 20f;
    [SerializeField] private bool isVertical = false;
    [SerializeField] private bool isLeft = false;
    [SerializeField] private bool isRight = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 force = Vector2.zero;

            if (isVertical)
            {
                force = Vector2.up * bounce;
            }
            else if (isLeft)
            {
                // Create a parabolic force by combining left and up vectors
                force = Vector2.left * bounce + Vector2.up * bounce;
            }
            else if (isRight)
            {
                // Create a parabolic force by combining right and up vectors
                force = Vector2.right * bounce + Vector2.up * bounce;
            }
            else
            {
                force = -Vector2.up * bounce;
            }

            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
