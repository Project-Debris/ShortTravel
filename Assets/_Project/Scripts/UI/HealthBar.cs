using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        fillImage.fillAmount = currentHealth / maxHealth;
    }
}
