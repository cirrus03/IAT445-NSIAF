using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private AudioMixer volumeMixer;


    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public void setMusicVolume()
    {
        float volume = musicSlider.value;
        volumeMixer.SetFloat("music_volume", Mathf.Log10(volume)*20);
    }

    public void setSFXVolume()
    {
        float volume = sfxSlider.value;
        volumeMixer.SetFloat("sfx_volume", Mathf.Log10(volume)*20);
    }
    
}
