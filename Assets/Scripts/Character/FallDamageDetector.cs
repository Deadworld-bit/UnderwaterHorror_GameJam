using UnityEngine;

public class FallDamageDetector : MonoBehaviour
{
    [Header("Fall Damage Settings")]
    [SerializeField] private float damageThresholdHeight = 5f;
    [SerializeField] private float damageMultiplier = 10f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private float maxHeight;
    private bool isFalling;
    private Health health;
    private Vector3 lastGroundedPosition;

    void Start()
    {
        health = GetComponent<Health>();
        lastGroundedPosition = transform.position;
    }

    void Update()
    {
        if (IsGrounded())
        {
            if (isFalling)
            {
                HandleFallDamage();
                isFalling = false;
            }
            lastGroundedPosition = transform.position;
        }
        else
        {
            TrackFallHeight();
        }
    }

    private void HandleFallDamage()
    {
        float fallHeight = maxHeight - transform.position.y;
        if (fallHeight > damageThresholdHeight)
        {
            float damage = (fallHeight - damageThresholdHeight) * damageMultiplier;
            health.TakeDamage(damage);
        }
    }

    private void TrackFallHeight()
    {
        if (!isFalling)
        {
            isFalling = true;
            maxHeight = lastGroundedPosition.y;
        }
        else if (transform.position.y > maxHeight)
        {
            maxHeight = transform.position.y;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}