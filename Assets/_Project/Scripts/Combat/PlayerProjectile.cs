using UnityEngine;
public class PlayerProjectile : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    public void Initialize(Vector2 direction, float customSpeed, int customDamage)
    {
        moveDirection = direction.normalized;
        damage = customDamage;

        if (customSpeed > 0f) speed = customSpeed;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyHit>(out var enemyHit)
            && collision.TryGetComponent<Health>(out var health)
            && !health.IsDead)
        {
            enemyHit.TakeHit(damage, transform.position);
            Destroy(gameObject);
        }

        else if (((1 << collision.gameObject.layer) & LayerMask.GetMask("Ground", "Ceiling", "Block")) != 0)
        {
            Destroy(gameObject);
        }
    }
}