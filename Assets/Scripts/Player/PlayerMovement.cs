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
    private Animator animator;
    #endregion

    #region Timers
    private float dashTimeCounter;
    private float idleTimer = 0f;
    private const float idleThreshold = 8f;
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
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        DashChecks();
        RunAnimationStateChecks();
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

    private void RunAnimationStateChecks()
    {
        if (!IsGrounded())
        {
            animator.SetBool("isRunning", false);
            SetAllIdleAnimationsFalse();
            idleTimer = 0f;
        }
        else
        {
            // Check if the player is running.
            if (horizontal != 0)
            {
                animator.SetBool("isRunning", true);
                SetAllIdleAnimationsFalse();
            }
            else if (IsGrounded())
            {
                // The player is not running and has not moved yet.
                animator.SetBool("isRunning", false);
                // Increment the idle timer.
                idleTimer += Time.deltaTime;

                // Set isIdle1 for the first 4 seconds, then isIdle2 for the next 4 seconds.
                if (idleTimer < 8f)
                {
                    animator.SetBool("isIdle1", true);
                    animator.SetBool("isIdle2", false);
                    animator.SetBool("isIdle3", false);
                }
                else if (idleTimer < 16f)
                {
                    animator.SetBool("isIdle1", false);
                    animator.SetBool("isIdle2", true);
                    animator.SetBool("isIdle3", false);
                }
                else
                {
                    // After 8 seconds, set isIdle3 to true until the player moves.
                    animator.SetBool("isIdle1", false);
                    animator.SetBool("isIdle2", false);
                    animator.SetBool("isIdle3", true);
                }
            }
        }
    }

    private void SetAllIdleAnimationsFalse()
    {
        animator.SetBool("isIdle1", false);
        animator.SetBool("isIdle2", false);
        animator.SetBool("isIdle3", false);
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

        if (IsGrounded() || (dashTimeCounter < 0f && IsGrounded()))
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
                dirDash = moveInput.normalized; // Normalize the input for consistent force.
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
