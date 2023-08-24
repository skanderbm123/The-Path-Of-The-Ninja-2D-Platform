using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;
    [SerializeField] private float ahead;
    [SerializeField] private float onTopOrBelow;



    private PlayerFlip playerFlip;
    private void Awake()
    {
        playerFlip = GetComponent<PlayerFlip>();
    }

    private void Update()
    {
        //float isFacingRight = Mathf.Sign(target.localScale.x);
        offset = new Vector3(ahead, onTopOrBelow, -10f);
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
