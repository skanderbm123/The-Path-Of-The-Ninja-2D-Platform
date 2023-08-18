using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    private InputAction.CallbackContext jumpInput;
    private float jumpingPower = 28f;
    private float doubleJumpingPower = 22f;
    private float coyoteTime = 0.5f;
    private float jumpBufferTime = 0.5f;

    [Header("Wall Slide and Jump Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    private float wallSlidingSpeed = 2f;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(22f, 33f);

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool doubleJump;

    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCounter;

    private PlayerFlip playerFlip;
    private PlayerMovement playerMovement;

    [SerializeField] private new Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerFlip = GetComponent<PlayerFlip>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        jumpInput = context;
        Jump(context);
    }


    // Update is called once per frame
    void Update()
    {
        if (playerMovement.IsDashing())
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

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            playerFlip.Flip(playerMovement.GetHorizontal());
        }
    }

    private void FixedUpdate()
    {
        if (playerMovement.IsDashing())
        {
            return;
        }

        if (!isWallJumping)
        {
            rigidbody.velocity = new Vector2(playerMovement.GetHorizontal() * playerMovement.GetSpeed(), rigidbody.velocity.y);
        }

        rigidbody.velocity = new Vector2(playerMovement.GetHorizontal() * playerMovement.GetSpeed(), rigidbody.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && playerMovement.GetHorizontal() != 0f)
        {
            isWallSliding = true;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void Jump(InputAction.CallbackContext context)
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

            if (jumpBufferCounter > 0f && !isWallSliding)
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

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (jumpInput.performed && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rigidbody.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                playerFlip.setIsFacingRight(playerFlip.GetIsFacingRight());
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    internal bool IsWallJumping()
    {
        return isWallJumping;
    }
}
