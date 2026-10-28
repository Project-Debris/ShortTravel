using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("컴포넌트")]
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    [Header("이동 관련 설정")]
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float acceleration = 35f;
    [SerializeField] private float deceleration = 25f;
    private float moveInput;

    [Header("점프 관련 설정")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("충돌 감지 설정")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask ceilingLayer;
    [SerializeField] private float checkDistance = 0.05f;

    [Header("현재 상태")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isTouchingCeiling;

    private PlayerInputActions inputActions;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();

        // 점프 키를 누르는 순간 OnJumpStarted 함수 실행
        inputActions.Player.Jump.started += OnJumpStarted;
        // 점프 키를 떼는 순간 OnJumpCanceled 함수 실행
        inputActions.Player.Jump.canceled += OnJumpCanceled;
    }

    void OnDisable()
    {
        // 이벤트 구독 해제
        inputActions.Player.Jump.started -= OnJumpStarted;
        inputActions.Player.Jump.canceled -= OnJumpCanceled;

        inputActions.Player.Disable();
    }

    void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<float>();

        CheckCollisions();

        // 천장에 부딫힐 때
        if (isTouchingCeiling && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }
    }

    private void HandleMovement()
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

        // 바닥 감지
        RaycastHit2D groundHit = Physics2D.BoxCast(
            bounds.center,
            new Vector2(bounds.size.x * 0.9f, bounds.size.y),
            0f,
            Vector2.down,
            checkDistance,
            groundLayer
        );
        isGrounded = groundHit.collider != null;

        // 천장 감지
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