using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] private HealthBar healthBar;

    [Header("체력 설정")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [Header("사망 상태")]
    [SerializeField] private bool isDead;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = MaxHealth;
    }
    public void TakeDamage(float damage)
    {
        if (IsDead) return;

        currentHealth = Mathf.Max(0, CurrentHealth - damage);

        if (CurrentHealth <= 0)
            isDead = true; //사망 상태 표시 -> 넉백 적용 x(PlayerHit, EnemyHit 스크립트에서 처리함), Die() 실행(어디서 처리할지 미정)

        if (healthBar)
        {
            healthBar.UpdateHealthBar(CurrentHealth, MaxHealth);
        }
    }
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        if (healthBar)
        {
            healthBar.UpdateHealthBar(CurrentHealth, MaxHealth);
        }
    }
}
