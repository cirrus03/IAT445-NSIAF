using UnityEngine;

public class BreakableWall : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float startingHealth = 3f;
    private float currentHealth;

    [Header("Optional")]
    [SerializeField] private GameObject breakVfxPrefab;

    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        Debug.Log($"{name} took damage. HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Break();
        }
    }

    private void Break()
    {
        if (breakVfxPrefab != null)
        {
            Instantiate(breakVfxPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}