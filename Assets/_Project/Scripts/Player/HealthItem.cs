using UnityEngine;

public class HealthItem : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    public float healAmount = 40;
    public float healDuration = 1.0f;

    public void Use()
    {
        playerHealth.Heal(healAmount);
    }


}
