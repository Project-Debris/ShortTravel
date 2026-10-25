using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    private float maxSpeed = 7f;
    private float acceleration = 35f;
    private float deceleration = 25f;
    private float moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
}
