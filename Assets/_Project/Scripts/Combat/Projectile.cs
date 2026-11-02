using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("컴포넌트")]
    private Rigidbody2D rb;

    [Header("기본 설정")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage;
    // 메모리를 위한 자동 파괴 시간
    [SerializeField] private float lifeTime = 5f;

    private Vector2 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    public void Initialize(Vector2 direction, float customSpeed, float customDamage)
    {
        moveDirection = direction.normalized;
        damage = customDamage;

        if (customSpeed > 0f)
        {
            speed = customSpeed;
        }

        // 이동 방향에 맞춰 스프라이트 회전
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 일정 시간 뒤 자동 파괴
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 지형에 부딫히면 파괴
        if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Ground")) != 0)
        {
            Destroy(gameObject);
        }
    }
}
