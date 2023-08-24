using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerJump : MonoBehaviour
{


    #region Jump Settings

    private InputAction.CallbackContext jumpInput;

    #endregion

    #region Private Variables

    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float wallJumpingCounter;

    private bool doubleJump;
    private bool isWallSliding;

    private float wallJumpingDirection;

    public PlayerData Data;
    private PlayerFlip playerFlip;
    private PlayerMovement playerMovement;
    private PlayerEnvironment playerEnvironment;
    private new Rigidbody2D rigidbody;
    #endregion


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerFlip = GetComponent<PlayerFlip>();
        playerMovement = GetComponent<PlayerMovement>();
        playerEnvironment = GetComponent<PlayerEnvironment>();
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        jumpInput = context;
        Jump(context);
    }

    private void Update()
    {
        if (Data.isDashing)
        {
            return;
        }

        if (IsGrounded())
        {
            coyoteTimeCounter = Data.coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        WallSlide();
        //WallClimb();
    }

    private bool IsGrounded()
    {
        return playerEnvironment.IsGrounded();
    }

    private bool IsWalled()
    {
        return playerEnvironment.IsWalled();
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && playerMovement.GetHorizontal() != 0f)
        {
            isWallSliding = true;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y, -Data.slideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isWallSliding)
        {
            WallJump();
        }

        if (IsGrounded())
        {
            doubleJump = false;
        }
        if (context.performed)
        {
            jumpBufferCounter = Data.jumpInputBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && !isWallSliding)
        {
            if (coyoteTimeCounter > 0f || doubleJump)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, doubleJump ? Data.doublejumpHeight : Data.jumpHeight);
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
            Data.isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = Data.wallJumpTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (jumpInput.performed && wallJumpingCounter > 0f)
        {
            Data.isWallJumping = true;
            rigidbody.velocity = new Vector2(wallJumpingDirection * Data.wallJumpForce.x, Data.wallJumpForce.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                playerFlip.setIsFacingRight(!playerFlip.GetIsFacingRight());
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
            Invoke(nameof(StopWallJumping), Data.wallJumpRunLerp);
        }
    }

    private void WallClimb()
    {
        if (IsWalled() && !IsGrounded() && playerMovement.GetVertical() != 0f)
        {
            // add value for wall climb
            rigidbody.velocity = new Vector2(0, playerMovement.GetVertical() * 20f);
        }
    }

    private void StopWallJumping()
    {
        Data.isWallJumping = false;
    }

    internal bool IsWallJumping()
    {
        return Data.isWallJumping;
    }
}
