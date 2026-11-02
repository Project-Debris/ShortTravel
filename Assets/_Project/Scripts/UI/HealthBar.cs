using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        fillImage.fillAmount = currentHealth / maxHealth;
    }
}
