using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnvironment : MonoBehaviour
{

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallFrontCheck;
    [SerializeField] private LayerMask wallFrontLayer;
    [SerializeField] private Transform wallBackCheck;
    [SerializeField] private LayerMask wallBackLayer;

    internal bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    internal bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallFrontCheck.position, 0.2f, wallFrontLayer) || Physics2D.OverlapCircle(wallBackCheck.position, 0.2f, wallBackLayer);
    }

    internal bool IsWalledFromFront()
    {
        return Physics2D.OverlapCircle(wallFrontCheck.position, 0.2f, wallFrontLayer);
    }

    internal bool IsWalledFromBack()
    {
        return  Physics2D.OverlapCircle(wallBackCheck.position, 0.2f, wallBackLayer);
    }
}
