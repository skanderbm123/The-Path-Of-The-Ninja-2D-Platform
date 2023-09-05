using UnityEngine;

public static class PlayerUtilities
{

    /*private void GravityChecks(PlayerData Data, Rigidbody2D rigidbody)
    {
        if (!Data.isDashing)
        {
            if (Data.isWallSliding && rigidbody.velocity.y < 0)
            {
                SetGravityScale(0);
            }
            else if (Data.isJumpCut)
            {
                //Higher gravity if jump button released
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Max(rigidbody.velocity.y, -Data.maxFallSpeed));
            }
            else if ((Data.isJumping || Data.isWallJumping || Data.isJumpFalling) && Mathf.Abs(rigidbody.velocity.y) < Data.jumpHangTimeThreshold)
            {
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (Data.isKnockbackActive)
            {
                //lower gravity if is getting hit
                SetGravityScale(0);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
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
    }*/

    public static void SetGravityScale(Rigidbody2D rigidbody, float scale)
    {
        rigidbody.gravityScale = scale;
    }

}
