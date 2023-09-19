using UnityEngine;

public class Flowers : MonoBehaviour
{
    public float flipInterval = 1.5f;
    private float nextFlipTime;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Set the initial flip time
        spriteRenderer = GetComponent<SpriteRenderer>();
        nextFlipTime = Time.time + flipInterval;
    }

    private void Update()
    {
        // Check if it's time to flip the sprite
        if (Time.time >= nextFlipTime)
        {
            // Flip the sprite horizontally
            spriteRenderer.flipX = !spriteRenderer.flipX;

            // Update the next flip time
            nextFlipTime = Time.time + flipInterval;
        }
    }
}
