using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("컴포넌트")]
    private BoxCollider2D boxCollider;
    private PlayerHit playerHit;

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
    private bool isActiveFrame = false;

    private float currentAttackDir = 1f;

    private PlayerInputActions inputActions;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        playerHit = GetComponent<PlayerHit>();

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
        inputActions.Player.Attack.started -= OnAttackStarted;
        inputActions.Player.Disable();
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        if (isAttacking) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        Debug.Log("루틴 시작");
        isAttacking = true;

        // 공격 방향 고정
        currentAttackDir = GetFacingDirection();

        float timer = 0f;

        // startupDelay 시간 동안 피격 시 공격 취소
        while (timer < startupDelay)
        {
            if (playerHit.IsStunned)
            {
                isAttacking = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("공격 시작");

        isActiveFrame = true;

        // 다단히트 방지를 위한 HashSet
        HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

        timer = 0f;
        while (timer < activeDuration)
        {
            Debug.Log("방향 고정");
            Vector2 attackCenter = (Vector2)transform.position + new Vector2(attackOffset.x * currentAttackDir, attackOffset.y);

            // 범위 내 히트된 enemy
            Collider2D[] hits = Physics2D.OverlapBoxAll(attackCenter, attackSize, 0f, enemyLayer);
            foreach (Collider2D hit in hits)
            {
                Debug.Log($"감지된 오브젝트: {hit.name}");

                if (!hitTargets.Contains(hit))
                {
                    hitTargets.Add(hit);

                    if (hit.TryGetComponent<EnemyHit>(out var enemyHit))
                    {
                        enemyHit.TakeHit(attackDamage, transform.position);
                        Debug.Log("damege");
                    }
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("공격 끝");

        isActiveFrame = false;

        yield return new WaitForSeconds(recoveryDelay);

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

        if (isActiveFrame)
        {
            Gizmos.color = Color.red;
        }
        else if (isAttacking)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawWireCube(center, attackSize);
    }
}
