using UnityEngine;
using UnityEngine.UI; 

public class HealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider slider; 

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health; 
        slider.value= health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }

}
