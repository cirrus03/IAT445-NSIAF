using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth; 
    public float currentHealth {get; private set; }

    private void Awake()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, startingHealth);
        Debug.Log("took this much damage:");
        Debug.Log(damageAmount);
        Debug.Log(currentHealth);

        if (currentHealth > 0)
        {   
            Debug.Log("still alive");
        }
        else
        {
            //player dead
            Debug.Log(currentHealth);
            Debug.Log("you died");
        }
    }
}
