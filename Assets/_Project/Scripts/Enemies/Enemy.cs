using UnityEngine;

// 모든 적이 공유하는 몸체. 수치는 EnemyData 에셋에서 관리
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [Header("적 데이터")]
    [SerializeField] protected EnemyData data;
    protected int health;
    protected Transform player;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    protected virtual void Start()
    {
        health = data.maxHealth;
        var p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}