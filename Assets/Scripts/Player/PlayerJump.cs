using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    public bool isWallSliding { get; set; }
    public bool isJumpCut { get; set; }
    public bool canDoubleJump { get; set; }
    public float wallJumpingDirection { get; set; }
    public float _wallJumpFreezeTime { get; set; }
    public int _lastWallJumpDir { get; set; }
    #region Jump Settings
    public InputActionReference jump;
    #endregion

    #region Timers
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float wallJumpingCounter;
    #endregion

    #region COMPONENTS
    public PlayerData Data;
    private PlayerFlip playerFlip;
    private PlayerMovement playerMovement;
    private PlayerEnvironment playerEnvironment;
    private new Rigidbody2D rigidbody;
    private Animator animator;
    #endregion

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerFlip = GetComponent<PlayerFlip>();
        playerMovement = GetComponent<PlayerMovement>();
        playerEnvironment = GetComponent<PlayerEnvironment>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        StartWallSliding();
        JumpChecks();
        JumpAnimationStateChecks();
        if (isWallSliding)
        {
            Slide();
        }
        ResetTimersAndJumps();

    }

    private void JumpAnimationStateChecks()
    {
        if (IsGrounded())
        {

            animator.SetBool("isJumping", false);
            animator.SetBool("isJumpFalling", false);
            animator.SetBool("isWallSliding", false);
        }
        else if (Data.isWallSliding)
        {
            animator.SetBool("isWallSliding", true);
            animator.SetBool("isJumpFalling", false);
            animator.SetBool("isJumping", false);
        }
        else if (Data.isJumpFalling)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isJumpFalling", true);
            animator.SetBool("isWallSliding", false);
        }
        else if (!IsGrounded())
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isJumpFalling", true);

        }
    }

    private void FixedUpdate()
    {
        GravityChecks();

    }

    #region JUMP
    private void Jump()
    {
        if (!isWallSliding)
        {

            if (!jump.action.IsPressed()) // Jump cut during jump buffer, didn't find a better way for now :(
            {

                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Data.jumpHeight);
                float force = Data.jumpForce;
                if (rigidbody.velocity.y < 0)
                    force -= rigidbody.velocity.y;

                rigidbody.AddForce(Vector2.up * force / 2, ForceMode2D.Impulse);
            }
            else if (Data.isJumping)
            {

                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Data.jumpHeight);

                float force = Data.jumpForce;
                if (rigidbody.velocity.y < 0)
                    force -= rigidbody.velocity.y;

                rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            }
        }
    }

    private void JumpCut()
    {
        if (rigidbody.velocity.y > 0f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * 0.5f);
        }
    }

    private void JumpChecks()
    {
        if (Data.isJumping && rigidbody.velocity.y < 0)
        {
            Data.isJumping = false;
            if (!Data.isWallJumping && !Data.isWallSliding)
            {
                Data.isJumpFalling = true;
            }


        }
        if (Data.isWallJumping && (Time.time - _wallJumpFreezeTime > Data.wallJumpFreezeTime))
        {
            Data.isWallJumping = false;
        }

        if (!IsGrounded() && !Data.isJumping && !Data.isWallJumping)
        {
            isJumpCut = false;

        }

        // Jump
        if (isWallSliding)
        {
            Data.isWallJumping = true;
            Data.isJumping = false;
            isJumpCut = false;

            _wallJumpFreezeTime = Time.time;
            WallJump();
        }
        else if (IsGrounded() && jumpBufferCounter > 0f)
        {
            coyoteTimeCounter = 0f;
            Data.isJumping = true;
            Data.isWallJumping = false;

            Jump();

            // Reset double jump after a regular jump
            canDoubleJump = true;
        }
        else if (jump.action.WasPerformedThisFrame())
        {
            if (IsGrounded() || coyoteTimeCounter > 0f || canDoubleJump)
            {
                Data.isJumping = true;
                Data.isWallJumping = false;
                if (!IsGrounded() && canDoubleJump && coyoteTimeCounter < 0f)
                {
                    canDoubleJump = false;
                    Data.isJumping = true;
                    Data.isJumpFalling = false;
                    // Perform double jump here
                    Jump();
                }
                else
                {
                    coyoteTimeCounter = 0f;
                    Jump();
                }
            }
            else
            {
                // Buffer the jump input
                jumpBufferCounter = Data.jumpInputBufferTime;
            }
        }
        else if (jump.action.WasReleasedThisFrame() && IsGrounded())
        {
            coyoteTimeCounter = 0f;
            Data.isJumping = true;
            Data.isWallJumping = false;
            isJumpCut = true;
            JumpCut();
        }
    }

    private void GravityChecks()
    {
        if (!Data.isDashing)
        {
            if (isWallSliding && rigidbody.velocity.y < 0)
            {
                SetGravityScale(0);
            }
            else if (isJumpCut)
            {
                //Higher gravity if jump button released
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Max(rigidbody.velocity.y, -Data.maxFallSpeed));
            }
            else if ((Data.isJumping || Data.isWallJumping || Data.isJumpFalling) && Mathf.Abs(rigidbody.velocity.y) < Data.jumpHangTimeThreshold)
            {
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (rigidbody.velocity.y < 0)
            {
                //Higher gravity if falling
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Max(rigidbody.velocity.y, -Data.maxFallSpeed));
            }
            else
            {
                //Default gravity if standing on a platform or moving upwards
                SetGravityScale(Data.gravityScale);
            }
        }
    }

    #endregion

    #region WALL INTERACTIONS (JUMP & SLIDE)

    public bool CanSlide()
    {
        if (IsWalled() && (rigidbody.velocity.y < 0f) && !Data.isDashing && !IsGrounded())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StartWallSliding()
    {
        if (CanSlide() && (playerMovement.horizontal != 0f))
        {
            isWallSliding = true;
            Data.isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
            Data.isWallSliding = false;
        }
    }

    private void Slide()
    {
        if (!IsWalledFromFront())
        {
            playerFlip.setIsFacingRight(!playerFlip.GetIsFacingRight());
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        float speedDif = Data.slideSpeed - rigidbody.velocity.y;
        float movement = speedDif * Data.slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        rigidbody.AddForce(movement * Vector2.up);
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            Data.isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = Data.wallJumpFreezeTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }


        if (jump.action.WasPerformedThisFrame() && wallJumpingCounter > 0f)
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

            // Allow double jump after wall jump
            canDoubleJump = true;
        }
    }

    private void StopWallJumping()
    {
        Data.isWallJumping = false;
    }

    #endregion

    #region OTHERS
    private bool IsGrounded()
    {
        return playerEnvironment.IsGrounded();
    }

    private bool IsWalled()
    {
        return playerEnvironment.IsWalled();
    }

    private bool IsWalledFromFront()
    {
        return playerEnvironment.IsWalledFromFront();
    }

    private void ResetTimersAndJumps()
    {

        if (IsGrounded() && jumpBufferCounter < 0f)
        {
            isJumpCut = false;
        }

        if (IsGrounded())
        {
            //Data.isJumping = false;
            Data.isJumpFalling = false;
            Data.isWallJumping = false;
            isWallSliding = false;
            coyoteTimeCounter = Data.coyoteTime;

            // Reset double jump
            canDoubleJump = true;
            return;
        }


        coyoteTimeCounter -= Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;
    }

    public void SetGravityScale(float scale)
    {
        rigidbody.gravityScale = scale;
    }
    #endregion
}
