using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData Data;
    private PlayerJump playerJump;
    private PlayerFlip playerFlip;
    private PlayerEnvironment playerEnvironment;
    private new Rigidbody2D rigidbody;

    [SerializeField] private TrailRenderer trail;

    private float horizontal;
    private float vertical;

    #region Getters and Setters

    internal float GetHorizontal()
    {
        return horizontal;
    }

    internal float GetVertical()
    {
        return vertical;
    }

    #endregion

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerJump = GetComponent<PlayerJump>();
        playerFlip = GetComponent<PlayerFlip>();
        playerEnvironment = GetComponent<PlayerEnvironment>();
    }

    private void Update()
    {
        if (Data.isDashing)
        {
            return;
        }   

        if (IsGrounded() && Data.dashRefillTime < 0f)
        {
            Data.canDash = true;
        } else 
        {
            Data.dashRefillTime -= Time.deltaTime;
        }

        if (!Data.isWallJumping)
        {
            playerFlip.Flip(horizontal);
        }


        if (Data.canDash)
        {
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    private void FixedUpdate()
    {
        if (Data.isDashing)
        {
            return;
        }

        if (Data.canDash)
        {
            GetComponent<Renderer>().material.color = new Color(0, 1, 1);
        }


        if (!Data.isWallJumping)
        {
            Run(1);
        }
    }

    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = horizontal * Data.runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(rigidbody.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        /*if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else*/
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
   /*     if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(rigidbody.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }*/
        #endregion

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(rigidbody.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rigidbody.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f ) // && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rigidbody.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        rigidbody.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    private bool IsGrounded()
    {
        return playerEnvironment.IsGrounded();
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (Data.canDash)
        {
            StartCoroutine(EnableDash());
        }
    }

    private IEnumerator EnableDash()
    {
        Data.canDash = false;
        Data.isDashing = true;
        Data.dashRefillTime = Data.dashSleepTime;
        float originalGravity = rigidbody.gravityScale;

        rigidbody.gravityScale = 0f;

        float horizontalDashPower = transform.localScale.x * Mathf.Abs(horizontal) * Data.dashAmount;
        float verticalDashPower = transform.localScale.y * vertical * (Data.dashAmount / 1.5f);

        if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
        {
            horizontalDashPower = transform.localScale.x * Data.dashAmount;
        }

        rigidbody.velocity = new Vector2(horizontalDashPower, verticalDashPower);
        GetComponent<SpriteRenderer>().color = Color.cyan;
        trail.emitting = true;
        yield return new WaitForSeconds(Data.dashAttackTime);
        trail.emitting = false;

        rigidbody.gravityScale = originalGravity;

        Data.isDashing = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
         horizontal = context.ReadValue<Vector2>().x;
         vertical = context.ReadValue<Vector2>().y;
    }
}
