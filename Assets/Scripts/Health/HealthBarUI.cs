using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image fillImage;

    private void Update()
    {
        if (playerHealth == null || fillImage == null)
            return;

        float maxHealth = playerHealth.MaxHealth;
        float currentHealth = playerHealth.currentHealth;

        if (maxHealth <= 0f)
        {
            fillImage.fillAmount = 0f;
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }
}