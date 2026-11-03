using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        Debug.Log("함수 실행");
        Debug.Log(damage);
        if (IsDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log(currentHealth);

        if (currentHealth <= 0)
            IsDead = true; //사망 상태 표시 -> 넉백 적용 x(PlayerHit, EnemyHit 스크립트에서 처리함), Die() 실행(어디서 처리할지 미정)

        if (healthBar)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
    }
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        if (healthBar)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
    }
}
