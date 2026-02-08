using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarScript : MonoBehaviour
{
    public Slider healthBarSlider;
    public TextMeshProUGUI healthBarValueText;
    [SerializeField] private Health playerHealth;

    private int maxHealth;
    // public int currHealth;

    void Start()
    {
        // currHealth = maxHealth; //set current health to max
        maxHealth = (int) playerHealth.currentHealth;
    }

    void Update()
    {
        //set health bar text
        // healthBarValueText.text = currHealth.ToString() + "/" + maxHealth.ToString();
        healthBarValueText.text = playerHealth.currentHealth.ToString() + "/" + maxHealth.ToString();
        
        //set slider values
        healthBarSlider.value = (int)playerHealth.currentHealth;
        healthBarSlider.maxValue = maxHealth;
    }
}
