using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("컴포넌트")]
    private BoxCollider2D boxCollider;
    private PlayerHit playerHit;
    private Repair repair;

    [Header("공격 수치")]
    [SerializeField] private int attackDamage = 10;

    [Header("공격 타이밍(초)")]
    [SerializeField] private float startupDelay = 0.08f;
    [SerializeField] private float activeDuration = 0.1f;
    [SerializeField] private float recoveryDelay = 0.17f;

    [Header("공격 범위")]
    // 플레이어 중심 기준 공격 위치
    [SerializeField] private Vector2 attackOffset = new Vector2(0.8f, 0f);
    // 공격 범위 크기(기본 값)
    [SerializeField] private Vector2 attackSize = new Vector2(1.2f, 1.2f);
    [SerializeField] private LayerMask enemyLayer;

    [Header("플레이어 공격 상태")]
    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;
    private bool isStartup = false;
    private bool isActive = false;
    private bool isRecovery = false;

    private float currentAttackDir = 1f;
    private Coroutine attackCoroutine;
    private PlayerInputActions inputActions;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        playerHit = GetComponent<PlayerHit>();
        repair = GetComponent<Repair>(); 

        // 공격 범위 y값 설정 - 부모 스케일 반영
        float playerHeight = boxCollider.size.y * Mathf.Abs(transform.lossyScale.y);
        attackSize.y = 1.2f * playerHeight;

        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Attack.started += OnAttackStarted;
    }

    void OnDisable()
    {
        CancelAttack();

        inputActions.Player.Attack.started -= OnAttackStarted;
        inputActions.Player.Disable();
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        if (isAttacking || playerHit.IsStunned || repair.IsRepairing) return;
        attackCoroutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        isStartup = true;

        // 공격 방향 고정
        currentAttackDir = GetFacingDirection();

        float timer = 0f;

        // startupDelay 시간 동안 피격 시 공격 취소
        while (timer < startupDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isStartup = false;
        isActive = true;

        // 다단히트 방지를 위한 HashSet
        HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

        timer = 0f;
        while (timer < activeDuration)
        {
            Vector2 attackCenter = (Vector2)transform.position + new Vector2(attackOffset.x * currentAttackDir, attackOffset.y);

            // 범위 내 히트된 enemy
            Collider2D[] hits = Physics2D.OverlapBoxAll(attackCenter, attackSize, 0f, enemyLayer);
            foreach (Collider2D hit in hits)
            {
                if (!hitTargets.Contains(hit))
                {
                    hitTargets.Add(hit);

                    if (hit.TryGetComponent<EnemyHit>(out var enemyHit))
                    {
                        enemyHit.TakeHit(attackDamage, transform.position);
                    }
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        isActive = false;
        isRecovery = true;

        yield return new WaitForSeconds(recoveryDelay);

        isRecovery = false;
        isAttacking = false;
        attackCoroutine = null;
    }

    public void CancelAttack(bool force = false)
    {
        // 회복 단계에서 캔슬 X
        if (!force && isRecovery) return;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isStartup = false;
        isActive = false;
        isAttacking = false;
    }

    private float GetFacingDirection()
    {
        if (GetComponent<SpriteRenderer>().flipX) return -1f;
        return 1f;
    }

    // 공격 범위를 확인하기 위한 기즈모
    private void OnDrawGizmos()
    {
        float dir = isAttacking ? currentAttackDir : (Application.isPlaying ? GetFacingDirection() : 1f);
        Vector2 center = (Vector2)transform.position + new Vector2(attackOffset.x * dir, attackOffset.y);

        if (isStartup)
        {
            Gizmos.color = Color.red;
        }
        else if (isActive)
        {
            Gizmos.color = Color.blue;
        }
        else if (isRecovery)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawWireCube(center, attackSize);
    }
}
