using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnvironment : MonoBehaviour
{

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallFrontCheck;
    [SerializeField] private Transform wallBackCheck;
    [SerializeField] private LayerMask wallLayer;

    internal bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    internal bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallFrontCheck.position, 0.2f, groundLayer) || Physics2D.OverlapCircle(wallBackCheck.position, 0.2f, groundLayer);
    }

    internal bool IsWalledFromFront()
    {
        return Physics2D.OverlapCircle(wallFrontCheck.position, 0.2f, groundLayer);
    }

    internal bool IsWalledFromBack()
    {
        return Physics2D.OverlapCircle(wallBackCheck.position, 0.2f, groundLayer);
    }
}
