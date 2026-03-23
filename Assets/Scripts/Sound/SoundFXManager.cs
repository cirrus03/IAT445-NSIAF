using System;
using System.IO.Compression;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SoundFXManager : MonoBehaviour
{
   [Header("Audio Source")]
   [SerializeField] AudioSource musicSource;
   [SerializeField] AudioSource sfxSource;


    [Header("Audio Files Music")]
    public AudioClip levelBGM_0;
    
    [Header("Audio Clip SFX")]
    public AudioClip playerDash;
    public AudioClip playerAttack;
    public AudioClip playerLand;
    public AudioClip pickUpHeal;
    public AudioClip getQuestItem;
    

    [Header("Menu SFX")]
    
    public AudioClip menu1;
    public AudioClip menu2;
    public AudioClip menu3;
    [Header("Audio Clip SFX")]


    private string currentSceneName;
    private AudioClip currentLevelBGM;

    private void Awake()
    {   
        //get level bgm based on current scene
        currentSceneName = getCurrentSceneName();
        chooseLevelBGM(currentSceneName);
    }

    void Start()
    {   
        //set bgm to music source player
        musicSource.clip = currentLevelBGM;
        musicSource.Play();

        Debug.Log("isPlaying: " + musicSource.isPlaying);

         Debug.Log("BGM: " + currentLevelBGM);
        Debug.Log("Music Source: " + musicSource);

        Debug.Log("enabled: " + musicSource.enabled);
        Debug.Log("volume: " + musicSource.volume);
        Debug.Log("mute: " + musicSource.mute);
    }



    private string getCurrentSceneName()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        Debug.Log(sceneName);
        return sceneName;
    }


    private void chooseLevelBGM(string sceneName)
    {
        //defaults set to basic
        currentLevelBGM = levelBGM_0;
        
        if(sceneName == "LabAreaClone") 
            currentLevelBGM = levelBGM_0;
            return;

        // if(sceneName == "LabAreaClone") 
        //     currentLevelBGM = levelBGM_0;

        //add more eventually here

    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
