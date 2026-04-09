using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Image fillImage;

    private void Update()
    {
        if (bossHealth == null || fillImage == null)
            return;

        float maxHealth = bossHealth.MaxHealth;
        float currentHealth = bossHealth.currentHealth;

        if (maxHealth <= 0f)
        {
            fillImage.fillAmount = 0f;
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }
}