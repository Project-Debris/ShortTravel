using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class EnemyHit : MonoBehaviour
{
    [Header("피격 설정")]
    [SerializeField] private float knockbackForce = 3f;//넉백힘(임시값)
    [SerializeField] private float stunnedDuration = 0.15f; //경직시간

    public bool IsStunned { get; private set; } //IsStunned를 일반 적 공격, 이동 스크립트에서 읽어서 true이면 공격및이동 불가하도록(경직) 구현해주세요
    private Health health;
    private Rigidbody2D rb;
    private void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeHit(int damage, Vector2 attackerPosition)//플레이어의 공격 스크립트에서 호출
    {
        if (health.IsDead) return;
        health.TakeDamage(damage);
        if (health.IsDead) return; //이번 피해로 사망 시 넉백, 경직 적용 x
        ApplyKnockback(attackerPosition);
        StartStunned();
    }

    private void ApplyKnockback(Vector2 attackerPosition)
    {
        Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;//넉백 방향
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    //적은 무적시간이 없어서 경직시간이 끝나기 전에 또 경직을 받을 경우에 대비해서 이렇게 작성
    private Coroutine stunnedCoroutine;
    private void StartStunned()
    {
        if (stunnedCoroutine != null)
            StopCoroutine(stunnedCoroutine);

        stunnedCoroutine = StartCoroutine(StunnedRoutine());
    }
    private IEnumerator StunnedRoutine()
    {
        IsStunned = true;
        yield return new WaitForSeconds(stunnedDuration);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); //넉백 종료
        IsStunned = false;
        stunnedCoroutine = null;
    }


    private void Update()
    {
        // 테스트용: 2 키를 누르면 적 피격
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            TakeHit(10, (Vector2)transform.position + Vector2.left);
        }
    }
}
