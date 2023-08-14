using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    [SerializeField] private float coyoteTime = 0.5f;
    [SerializeField] private float jumpBufferTime = 0.5f;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;

    private float horizontal;
    private float vertical;
    private bool isFacingRight = true;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isJumping;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

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
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpingPower);
            jumpBufferCounter = 0f;

            StartCoroutine(JumpCooldown());
        }

        if (context.canceled && rigidbody.velocity.y > 0f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.4f);
        isJumping = false;
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
        rigidbody.velocity = new Vector2(transform.localScale.x * dashingPower, transform.localScale.y * vertical * dashingPower);
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
        Debug.Log("is horizontal value readed: " + horizontal);
        Debug.Log("is vertical value readed: " + vertical);

    }
}