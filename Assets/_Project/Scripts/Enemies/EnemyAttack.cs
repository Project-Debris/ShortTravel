// Game 화면에서 공격 방향과 범위 보이게 설정하기

using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("컴포넌트")]
    private SpriteRenderer spriteRenderer;
    private EnemyHit enemyHit;

    [Header("공격 사거리")]
    [SerializeField] private float attackRange = 5f;

    [Header("공격 타이밍")]
    [SerializeField] private float telegraphDuration = 0.8f;
    [SerializeField] private float recoveryDuration = 0.5f;
    [SerializeField] private float attackCooldown = 2.0f;

    [Header("투사체 및 위치 설정")]
    [SerializeField] private GameObject projectilePrefab;
    private Transform firePoint;
    private Transform playerTransform;

    private bool isAttacking = false;
    private bool isTelegraphing = false;
    private Vector2 lockedDirection;
    private Coroutine attackCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyHit = GetComponent<EnemyHit>();

        firePoint = transform.Find("FirePoint");

        GameObject playerObj = GameObject.FindWithTag("Player");
        playerTransform = playerObj.transform;
    }

    void Update()
    {
        if (playerTransform == null) return;

        // 플레이어와의 거리 측정
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            TriggerAttack();
        }
    }

    private void OnDisable()
    {
        CancelAttack();
    }

    private void TriggerAttack()
    {
        if (isAttacking || enemyHit.IsStunned) return;
        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        isTelegraphing = true;
        float timer = 0f;

        while (timer < telegraphDuration)
        {
            lockedDirection = (playerTransform.position - firePoint.position).normalized;
            spriteRenderer.color = Color.purple;
            timer += Time.deltaTime;
            yield return null;
        }

        lockedDirection = (playerTransform.position - firePoint.position).normalized;

        isTelegraphing = false;
        spriteRenderer.color = Color.red;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        if (projectile.TryGetComponent<Projectile>(out var proj))
        {
            proj.Initialize(lockedDirection, 10f, 10);
        }

        yield return new WaitForSeconds(recoveryDuration);

        float remainCooldown = attackCooldown - (telegraphDuration + recoveryDuration);
        if (remainCooldown > 0f)
        {
            yield return new WaitForSeconds(remainCooldown);
        }

        isAttacking = false;
        attackCoroutine = null;
    }

    public void CancelAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isTelegraphing = false;
        isAttacking = false;

        spriteRenderer.color = Color.red;
    }

    private void OnDrawGizmos()
    {
        // 플레이어 인식 범위
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Transform currentFirePoint = (firePoint != null) ? firePoint : transform.Find("FirePoint");
        if (currentFirePoint == null)
        {
            currentFirePoint = transform;
        }

        if (isTelegraphing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(currentFirePoint.position, lockedDirection * 8f);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(currentFirePoint.position, 0.1f);
        }
    }
}
