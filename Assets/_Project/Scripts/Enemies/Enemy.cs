using UnityEngine;

// 모든 적이 공유하는 몸체. 수치는 EnemyData 에셋에서 관리
[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [Header("적 데이터")]
    [SerializeField] protected EnemyData data;
    protected int health;
    protected Transform player;

    // 공격 구현 전. 아래 주석 유지.
    // protected AttackData attack;
    // protected float nextAttackTime;
    // public AttackData Attack => attack;

    // Z 축 교정 
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    protected virtual void Start()
    {
        health = data.maxHealth;
        // attack = data.attack;
        var p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    // 2.6.1 행동규칙 / 2.6.2 공격규칙 확정 후 업데이트
    //protected virtual void Update()
    //{
    //    if (attack == null || player == null || Time.time < nextAttackTime) return;

    //    Vector2 toPlayer = player.position - transform.position;
    //    if (toPlayer.magnitude <= attack.range)
    //    {
    //        attack.Execute(transform, toPlayer.normalized);
    //        nextAttackTime = Time.time + attack.cooldown;
    //    }
    //}

    public virtual void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    [ContextMenu("Test Damage")]
    private void TestDamage() => TakeDamage(1);
}