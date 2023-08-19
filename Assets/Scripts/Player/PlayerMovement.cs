using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 12.5f;
    private new Rigidbody2D rigidbody;

    [Header("Dash Settings")]
    [SerializeField] private TrailRenderer trail;
    private float dashingPower = 32f;
    private float dashingTime = 0.23f;
    private float dashingCooldown = 1f;

    private float horizontal;
    private float vertical;

    private bool canDash = true;
    private bool isDashing;
    private float dashingCooldownCounter;

    private PlayerJump playerJump;
    private PlayerEnvironment playerEnvironment;

    #region Getters and Setters

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


    #endregion

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerJump = GetComponent<PlayerJump>();
        playerEnvironment = GetComponent<PlayerEnvironment>();
    }

    private void Update()
    {
        if (isDashing)
        {
            return;
        }   

        if (IsGrounded() && dashingCooldownCounter < 0f)
        {
            canDash = true;
        } else 
        {
            dashingCooldownCounter -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
    }

    private bool IsGrounded()
    {
        return playerEnvironment.IsGrounded();
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
        dashingCooldownCounter = dashingCooldown;
        float originalGravity = rigidbody.gravityScale;

        rigidbody.gravityScale = 0f;

        float horizontalDashPower = transform.localScale.x * Mathf.Abs(horizontal) * dashingPower;
        float verticalDashPower = transform.localScale.y * vertical * (dashingPower / 1.5f);

        if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
        {
            horizontalDashPower = transform.localScale.x * dashingPower;
        }

        rigidbody.velocity = new Vector2(horizontalDashPower, verticalDashPower);

        trail.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        trail.emitting = false;

        rigidbody.gravityScale = originalGravity;

        isDashing = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
         horizontal = context.ReadValue<Vector2>().x;
         vertical = context.ReadValue<Vector2>().y;
    }
}
