using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHit : MonoBehaviour
{
    [Header("컴포넌트")]
    private Rigidbody2D rb;
    private Health health;
    private PlayerAttack playerAttack;

    [Header("피격 설정")]
    [SerializeField] private float invincibilityDuration = 0.5f;//무적시간
    [SerializeField] private float knockbackForce = 5f;//넉백힘(임시값)
    [SerializeField] private float stunnedDuration = 0.1f; //경직시간

    private bool isInvincible = false;
    public bool IsStunned { get; private set; } //IsStunned를 Player랑 PlayerAttack스크립트에서 읽어서 true이면 공격및이동 불가하도록(경직) 구현해주세요

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    public void TakeHit(int damage, Vector2 attackerPosition)//적의 공격 스크립트에서 호출
    {
        if (health.IsDead) return;
        if (isInvincible) return;
        health.TakeDamage(damage);

        if (playerAttack != null)
        {
            playerAttack.CancelAttack();
        }

        if (health.IsDead) return; //이번 피해로 사망 시 넉백, 무적, 경직 적용 x

        ApplyKnockback(attackerPosition);
        StartCoroutine(InvincibilityRoutine());
        StartCoroutine(StunnedRoutine());
    }

    private void ApplyKnockback(Vector2 attackerPosition)
    {
        Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }


    private IEnumerator StunnedRoutine()
    {
        IsStunned = true;
        yield return new WaitForSeconds(stunnedDuration);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); //넉백 종료
        IsStunned = false;
    }


    private void Update()
    {
        // 테스트용: 1 키를 누르면 플레이어 피격
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            TakeHit(10, (Vector2)transform.position + Vector2.right);
        }
    }
}
