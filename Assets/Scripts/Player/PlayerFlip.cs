using UnityEngine;

public class PlayerFlip : MonoBehaviour
{
    private bool isFacingRight = true;

    public void Flip(float horizontal)
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    internal bool GetIsFacingRight()
    {
        return isFacingRight;
    }

    internal void setIsFacingRight(bool value)
    {
        isFacingRight = value;
    }
}
