using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("공격 수치")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 0.35f;
    [SerializeField] private float attackDuration = 0.15f;

    [Header("공격 범위")]
    // 플레이어 중심 기준 공격 위치
    [SerializeField] private Vector2 attackOffset = new Vector2(0.8f, 0f);
    // 공격 범위 크기
    [SerializeField] private Vector2 attackSize = new Vector2(1.2f, 1.0f);
    [SerializeField] private LayerMask enemyLayer;

    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    private PlayerInputActions inputActions;

    void Awake()
    {
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
        if (Time.time < nextAttackTime || isAttacking) return;

        nextAttackTime = Time.time + attackCooldown;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        float attackDir = GetFacingDirection();
        float timer = 0f;

        HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

        while (timer < attackDuration)
        {
            Vector2 attackCenter = (Vector2)transform.position + new Vector2(attackOffset.x * attackDir, attackOffset.y);

            // 범위 내 히트된 enemy들을 관리 - 다단히트 방지
            Collider2D[] hits = Physics2D.OverlapBoxAll(attackCenter, attackSize, 0f, enemyLayer);
            foreach (Collider2D hit in hits)
            {
                if (!hitTargets.Contains(hit))
                {
                    hitTargets.Add(hit);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
    }

    private float GetFacingDirection()
    {
        if (GetComponent<SpriteRenderer>().flipX) return -1f;
        return 1f;
    }

    // 공격 범위를 확인하기 위한 기즈모
    private void OnDrawGizmosSelected()
    {
        float dir = Application.isPlaying ? GetFacingDirection() : 1f;
        Vector2 center = (Vector2)transform.position + new Vector2(attackOffset.x * dir, attackOffset.y);

        Gizmos.color = isAttacking ? Color.red : Color.yellow;
        Gizmos.DrawWireCube(center, attackSize);
    }
}
