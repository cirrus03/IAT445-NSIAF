using UnityEngine;
using UnityEngine.UI;

public class MoodUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image moodIcon;

    [Header("Mood Sprites")]
    [SerializeField] private Sprite neutralSprite;
    [SerializeField] private Sprite angrySprite;
    [SerializeField] private Sprite sadSprite;
    [SerializeField] private Sprite happySprite;

    private GameProgress.MoodState lastMood;

    void Start()
    {
        UpdateMoodIcon();
    }

    void Update()
    {
        if (GameProgress.Instance == null) return;

        if (GameProgress.Instance.playerMood != lastMood)
        {
            UpdateMoodIcon();
        }
    }

    private void UpdateMoodIcon()
    {
        if (GameProgress.Instance == null) return;

        var mood = GameProgress.Instance.playerMood;
        lastMood = mood;

        switch (mood)
        {
            case GameProgress.MoodState.Angry:
                moodIcon.sprite = angrySprite;
                break;

            case GameProgress.MoodState.Sad:
                moodIcon.sprite = sadSprite;
                break;

            case GameProgress.MoodState.Happy:
                moodIcon.sprite = happySprite;
                break;

            default:
                moodIcon.sprite = neutralSprite;
                break;
        }
    }
}