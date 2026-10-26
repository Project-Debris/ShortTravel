using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    private float maxSpeed = 7f;
    private float acceleration = 35f;
    private float deceleration = 25f;
    private float moveInput;

    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask ceilingLayer;
    private float checkDistance = 0.05f;

    private bool isGrounded;
    private bool isTouchingCeiling;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        float move = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed) move -= 1f;
            if (Keyboard.current.rightArrowKey.isPressed) move += 1f;
        }
        moveInput = move;

        CheckCollisions();

        if (Keyboard.current.zKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Keyboard.current.zKey.wasReleasedThisFrame && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        if (isTouchingCeiling && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }

    void FixedUpdate()
    {
        float targetSpeed = moveInput * maxSpeed;
        float accelRate = (Mathf.Abs(moveInput) > 0.01f) ? acceleration : deceleration;
        float newSpeedX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            accelRate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(newSpeedX, rb.linearVelocity.y);
    }

    private void CheckCollisions()
    {
        Bounds bounds = boxCollider.bounds;

        RaycastHit2D groundHit = Physics2D.BoxCast(
            bounds.center,
            new Vector2(bounds.size.x * 0.9f, bounds.size.y),
            0f,
            Vector2.down,
            checkDistance,
            groundLayer
        );
        isGrounded = groundHit.collider != null;

        RaycastHit2D ceilingHit = Physics2D.BoxCast(
            bounds.center,
            new Vector2(bounds.size.x * 0.9f, bounds.size.y),
            0f,
            Vector2.up,
            checkDistance,
            ceilingLayer
        );
        isTouchingCeiling = ceilingHit.collider != null;
    }
}
