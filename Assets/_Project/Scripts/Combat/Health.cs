using UnityEngine;

public class Health : MonoBehaviour
{
    private float currentHealth = 100;
    private float maxHealth = 100;
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
    }
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
}
