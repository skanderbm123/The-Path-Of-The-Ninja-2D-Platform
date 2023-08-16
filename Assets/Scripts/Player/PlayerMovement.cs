using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 8f;

    [SerializeField] private float jumpingPower = 28f;
    [SerializeField] private float doubleJumpingPower = 22f;
    [SerializeField] private float coyoteTime = 0.5f;
    [SerializeField] private float jumpBufferTime = 0.5f;

    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;

    private float horizontal;
    private float vertical;
    private bool isFacingRight = true;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool doubleJump;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 36f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.7f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        Flip();
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        rigidbody.velocity = new Vector2(horizontal * speed, rigidbody.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            doubleJump = false;
        }
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f)
        {
            if (coyoteTimeCounter > 0f || doubleJump)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, doubleJump ? doubleJumpingPower : jumpingPower);
                jumpBufferCounter = 0f;
                doubleJump = !doubleJump;
            }
        }

        if (context.canceled && rigidbody.velocity.y > 0f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (canDash) {
            StartCoroutine(EnableDash());
        }
    }

    private IEnumerator EnableDash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rigidbody.gravityScale;

        rigidbody.gravityScale = 0f;
        float horizontalDashPower = transform.localScale.x * horizontal * dashingPower * (isFacingRight ? 1 : -1);
        float verticalDashPower = transform.localScale.y * vertical * (dashingPower/1.5f);

        rigidbody.velocity = new Vector2(horizontalDashPower, verticalDashPower);

        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;

        rigidbody.gravityScale = originalGravity;

        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
        //Debug.Log("is horizontal value readed: " + horizontal);
        //Debug.Log("is vertical value readed: " + vertical);
    }
}