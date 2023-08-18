using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 12.5f;
    [SerializeField] private new Rigidbody2D rigidbody;

    [Header("Dash Settings")]
    [SerializeField] private TrailRenderer trail;
    private float dashingPower = 32f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.7f;

    private float horizontal;
    private float vertical;

    private bool canDash = true;
    private bool isDashing;

    private PlayerFlip playerFlip;
    private PlayerJump playerJump;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerFlip = GetComponent<PlayerFlip>();
        playerJump = GetComponent<PlayerJump>();
    }

   

    private void Update()
    {
        if (isDashing)
        {
            return;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        if (!playerJump.IsWallJumping())
        {
            rigidbody.velocity = new Vector2(horizontal * speed, rigidbody.velocity.y);
        }

        rigidbody.velocity = new Vector2(horizontal * speed, rigidbody.velocity.y);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (canDash)
        {
            StartCoroutine(EnableDash());
        }
    }

    private IEnumerator EnableDash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rigidbody.gravityScale;

        rigidbody.gravityScale = 0f;

        float horizontalDashPower = transform.localScale.x * Mathf.Abs(horizontal) * dashingPower;
        float verticalDashPower = transform.localScale.y * vertical * (dashingPower / 1.5f);

        if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
        {
            horizontalDashPower = transform.localScale.x * dashingPower ;
        }
        
        rigidbody.velocity = new Vector2(horizontalDashPower, verticalDashPower);

        trail.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        trail.emitting = false;

        rigidbody.gravityScale = originalGravity;

        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    internal float GetHorizontal()
    {
        return horizontal;
    }

    internal float GetVertical()
    {
        return vertical;
    }

    internal float GetSpeed()
    {
        return speed;
    }

    internal bool IsDashing()
    {
        return isDashing;
    }
}
