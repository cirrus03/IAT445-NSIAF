using UnityEngine;
using UnityEngine.UI; 
public class HealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider slider; 

    public void SetMaxHealth(float health)
    {
        if (slider == null) return;//please run please
        slider.maxValue = health; 
        slider.value= health;
    }

    public void SetHealth(float health)
    {
        if (slider == null) return;
        slider.value = health;
    }

}
