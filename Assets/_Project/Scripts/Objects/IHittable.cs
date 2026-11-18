using UnityEngine;

public enum AttackSource { Melee, PlayerPin, EnemyPin }

// 적이 아닌 피격 대상(약한 벽, 유리, 버튼 등)의 공통 진입점.
// 공격 종류별 반응은 각 대상이 정한다.
public interface IHittable
{
    void OnHit(AttackSource source);
}

// 핀 스크립트(PlayerProjectile, Projectile)를 수정하지 않고, 닿은 콜라이더가 어느 핀인지로 공격 종류를 판별한다.
// 핀은 트리거라서 닿으면 맞은 오브젝트 쪽에도 OnTriggerEnter2D가 호출된다.
public static class PinContact
{
    public static bool TryGetSource(Collider2D other, out AttackSource source)
    {
        source = default;
        Rigidbody2D body = other.attachedRigidbody;
        if (body == null) return false;

        if (body.TryGetComponent<PlayerProjectile>(out _)) { source = AttackSource.PlayerPin; return true; }
        if (body.TryGetComponent<Projectile>(out _)) { source = AttackSource.EnemyPin; return true; }
        return false;
    }
}
