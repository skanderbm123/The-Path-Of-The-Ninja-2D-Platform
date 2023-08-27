using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    public bool isWallSliding { get; set; }
    public bool isJumpFalling { get; set; }
    public bool isJumpCut { get; set; }
    public float wallJumpingDirection { get; set; }

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
    private float _wallJumpFreezeTime;
    private int _lastWallJumpDir;
    #endregion

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerFlip = GetComponent<PlayerFlip>();
        playerMovement = GetComponent<PlayerMovement>();
        playerEnvironment = GetComponent<PlayerEnvironment>();
    }

    private void Update()
    {
        WallSlide();
        JumpChecks();

    }

    private void FixedUpdate()
    {
        ResetTimersAndJumps();
        GravityChecks();
    }

    #region JUMP
    private void Jump()
    {
        if (!isWallSliding)
        {
            if (Data.isJumping || jumpAvailables > 0)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpAvailables == Data.extraJumps ? Data.jumpHeight : (Data.jumpHeight * 1.2f));

                float force = Data.jumpForce;
                if (rigidbody.velocity.y < 0)
                    force -= rigidbody.velocity.y;

                rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
                --jumpAvailables;
            }
        }
    }

    private void JumpChecks()
    {
        if (Data.isJumping && rigidbody.velocity.y < 0) // add check aussi pour double jump
        {
            //Data.isJumping = false; this bcomes false then

            if (!Data.isWallJumping)
                isJumpFalling = true;

        }

        if (Data.isWallJumping && (Time.time - _wallJumpFreezeTime > Data.wallJumpFreezeTime))
        {
            Data.isWallJumping = false;
        }

        if (IsGrounded() && !Data.isJumping && !Data.isWallJumping)
        {
            isJumpCut = false;
            
            if (!Data.isJumping)
                isJumpFalling = false; // a voir il va ou
        }

       
            //Debug.Log(jumpAvailables);
            //Jump
            if (isWallSliding)
            {
                Data.isWallJumping = true;
                Data.isJumping = false;
                isJumpCut = false;
                isJumpFalling = false; // a voir il va ou

                _wallJumpFreezeTime = Time.time;
                WallJump();
            }
            else if (jump.action.WasPressedThisFrame() && ((IsGrounded() && !Data.isJumping) || jumpBufferCounter > 0 || coyoteTimeCounter > 0))
            {
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
                Data.isJumping = true;
                Data.isWallJumping = false;
                isJumpCut = false;
                isJumpFalling = false;
                Jump();
            } 
            else if (jump.action.WasReleasedThisFrame())
            {
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;
                Data.isJumping = true;
                Data.isWallJumping = false;
                isJumpCut = true;
                isJumpFalling = false; // a voir il va ou
                JumpCut();
            }
        
    }

    private void JumpCut()
    {
        if (rigidbody.velocity.y > 0f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * 0.5f);
        }
    }

    private void GravityChecks()
    {
        if (!Data.isDashing)
        {
            if (isWallSliding)
            {
                SetGravityScale(0);
            }
            else if (isJumpCut)
            {
                //Higher gravity if jump button released
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Max(rigidbody.velocity.y, -Data.maxFallSpeed));
            }
            else if ((Data.isJumping || Data.isWallJumping || isJumpFalling) && Mathf.Abs(rigidbody.velocity.y) < Data.jumpHangTimeThreshold)
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
    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && playerMovement.horizontal != 0f)
        {
            isWallSliding = true;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y, -Data.slideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }


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

        if (jump.action.WasPressedThisFrame() && wallJumpingCounter > 0f)
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

    private void ResetTimersAndJumps()
    {
        if (IsGrounded())
        {
            Data.isJumping = false;
            Data.isWallJumping = false;
            isJumpCut = false;
            isWallSliding = false;
            jumpBufferCounter = Data.jumpInputBufferTime;
            coyoteTimeCounter = Data.coyoteTime;
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
