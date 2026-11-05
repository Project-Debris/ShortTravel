using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("컴포넌트")]
    private Rigidbody2D rd;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private PlayerHit playerHit;

    // 방향 확인용 임시
    [SerializeField] private SpriteRenderer childSpriteRenderer;

    [Header("이동 설정")]
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float acceleration = 35f;
    [SerializeField] private float deceleration = 25f;
    private float moveInput;
    public float MoveInput => moveInput;

    [Header("점프 설정")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("충돌 감지 설정")]
    [SerializeField] private float collisionCheckDistance = 0.05f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask ceilingLayer;

    [Header("현재 상태")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isTouchingCeiling;

    private PlayerInputActions inputActions;

    void Awake()
    {
        rd = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHit = GetComponent<PlayerHit>();

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

        FlipSprite();

        CheckCollisions();

        // 천장에 부딫힐 때
        if (isTouchingCeiling && rd.linearVelocity.y > 0)
        {
            rd.linearVelocity = new Vector2(rd.linearVelocity.x, 0f);
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (playerHit.IsStunned) return;

        if (isGrounded)
        {
            rd.linearVelocity = new Vector2(rd.linearVelocity.x, jumpForce);
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (rd.linearVelocity.y > 0)
        {
            rd.linearVelocity = new Vector2(rd.linearVelocity.x, rd.linearVelocity.y * jumpCutMultiplier);
        }
    }

    // 이동 방향에 맞게 플레이어 방향 수정
    private void FlipSprite()
    {
        if (moveInput > 0f)
        {
            spriteRenderer.flipX = false;
            // 임시 코드
            childSpriteRenderer.flipY = false;
        }
        else if (moveInput < 0f)
        {
            spriteRenderer.flipX = true;
            // 임시 코드
            childSpriteRenderer.flipY = true;
        }
    }

    // 플레이어 이동 함수
    private void HandleMovement()
    {
        if (playerHit.IsStunned) return;

        float targetSpeed = moveInput * maxSpeed;
        float accelRate = (Mathf.Abs(moveInput) > 0.01f) ? acceleration : deceleration;
        float newSpeedX = Mathf.MoveTowards(
            rd.linearVelocity.x,
            targetSpeed,
            accelRate * Time.fixedDeltaTime
        );

        rd.linearVelocity = new Vector2(newSpeedX, rd.linearVelocity.y);
    }

    // 충돌 감지 함수
    private void CheckCollisions()
    {
        Bounds bounds = boxCollider.bounds;

        // 바닥 감지
        RaycastHit2D groundHit = Physics2D.BoxCast(
            bounds.center,
            new Vector2(bounds.size.x * 0.9f, bounds.size.y),
            0f,
            Vector2.down,
            collisionCheckDistance,
            groundLayer
        );
        isGrounded = groundHit.collider != null;

        // 천장 감지
        RaycastHit2D ceilingHit = Physics2D.BoxCast(
            bounds.center,
            new Vector2(bounds.size.x * 0.9f, bounds.size.y),
            0f,
            Vector2.up,
            collisionCheckDistance,
            ceilingLayer
        );
        isTouchingCeiling = ceilingHit.collider != null;
    }
}