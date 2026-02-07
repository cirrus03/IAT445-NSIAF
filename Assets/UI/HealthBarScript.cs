using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarScript : MonoBehaviour
{
    public Slider healthBarSlider;
    public TextMeshProUGUI healthBarValueText;

    public int maxHealth;
    public int currHealth;

    void Start()
    {
        currHealth = maxHealth; //set current health to max
    }

    void Update()
    {
        //set health bar text
        healthBarValueText.text = currHealth.ToString() + "/" + maxHealth.ToString();
        
        //set slider values
        healthBarSlider.value = currHealth;
        healthBarSlider.maxValue = maxHealth;
    }
}
