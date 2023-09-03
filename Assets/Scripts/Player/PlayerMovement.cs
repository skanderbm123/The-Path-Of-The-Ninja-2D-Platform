using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region COMPONENTS
    public PlayerData Data;
    private PlayerFlip playerFlip;
    private PlayerJump playerJump;
    private PlayerEnvironment playerEnvironment;
    private new Rigidbody2D rigidbody;
    #endregion

    #region Timers
    private float dashTimeCounter;
    #endregion

    [SerializeField] private TrailRenderer trail;

    public float horizontal { get; set; }
    public float vertical { get; set; }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerJump = GetComponent<PlayerJump>();
        playerFlip = GetComponent<PlayerFlip>();
        playerEnvironment = GetComponent<PlayerEnvironment>();

        Respawned();
    }

    private void Respawned()
    {
        Data.isDashing = false;
        Data.isJumping = false;
        Data.isWallJumping = false;
        Data.isJumpFalling = false;
        Data.canDash = true;
    }

    private void Update()
    {
        DashChecks();

        if (!Data.canDash)
        {
            GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        else
        {
            if (playerFlip.GetIsFacingRight())
            {
                GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    private void FixedUpdate()
    {
        RunChecks();
    }

    private void RunChecks()
    {
        if (!Data.isWallJumping && IsGrounded())
        {
            playerFlip.Flip(horizontal);
        }

        if (!Data.isDashing)
        {
            if (!Data.isWallJumping)
            {
                Run(1);
            }
        }
        /* else if (_isDashAttacking)
         {
             Run(Data.dashEndRunLerp);
         }*/
    }

    #region RUN
    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    private void Run(float lerpAmount)
    {
        float targetSpeed = horizontal * Data.runMaxSpeed;
        targetSpeed = Mathf.Lerp(rigidbody.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;
        if (playerEnvironment.IsGrounded())
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        if ((!playerEnvironment.IsGrounded() || Data.isWallJumping || Data.isJumpFalling) && Mathf.Abs(rigidbody.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        if (Data.doConserveMomentum && Mathf.Abs(rigidbody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rigidbody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f) // && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }
        #endregion

        float speedDif = targetSpeed - rigidbody.velocity.x;
        float movement = speedDif * accelRate;
        rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }
    #endregion

    #region DASH
    private void DashChecks()
    {
        if (Data.isDashing)
        {
            return;
        }

        if (Data.canDash)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }

        if (dashTimeCounter < 0f)
        {
            Data.canDash = true;
        }
        else
        {
            dashTimeCounter -= Time.deltaTime;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (Data.canDash)
        {
            Sleep(Data.dashSleepTime);
            Vector2 moveInput = new Vector2(horizontal, vertical);
            Vector2 dirDash;
            if (moveInput != Vector2.zero)
                dirDash = moveInput;
            else
                dirDash = playerFlip.GetIsFacingRight() ? Vector2.right : Vector2.left;

            StartCoroutine(nameof(EnableDash), dirDash);
        }
    }

    private IEnumerator EnableDash(Vector2 dirDash)
    {
        Data.canDash = false;
        Data.isDashing = true;

        SetGravityScale(0);
        float startTime = Time.time;
        trail.emitting = true;
        while (Time.time - startTime <= Data.dashAttackTime)
        {
            rigidbody.velocity = dirDash.normalized * Data.dashSpeed;
            yield return null;
        }

        startTime = Time.time;

        SetGravityScale(Data.gravityScale);
        rigidbody.velocity = Data.dashEndSpeed * dirDash.normalized;

        while (Time.time - startTime <= Data.dashEndTime)
        {
            yield return null;
        }
        trail.emitting = false;
        dashTimeCounter = Data.dashRefillTime;
        Data.isDashing = false;
    }
    #endregion

    public void SetGravityScale(float scale)
    {
        rigidbody.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    private bool IsGrounded()
    {
        return playerEnvironment.IsGrounded();
    }
}
