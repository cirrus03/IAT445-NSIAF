using UnityEngine;

public class Level3HazardUnlock : MonoBehaviour
{
    [Header("Unlock Settings")]
    [SerializeField] private int unlockAtQuestStage = 3;
    [SerializeField] private GameObject hazardToToggle;

    private bool unlocked = false;

    private void Start()
    {
        if (hazardToToggle == null || GameProgress.Instance == null)
            return;

        if (GameProgress.Instance.level3HazardSpawnUnlocked)
        {
            unlocked = true;
            hazardToToggle.SetActive(true);
            return;
        }

        hazardToToggle.SetActive(false);
        RefreshState();
    }

    private void Update()
    {
        if (!unlocked)
        {
            RefreshState();
        }
    }

    private void RefreshState()
    {
        if (GameProgress.Instance == null || hazardToToggle == null)
            return;

        if (GameProgress.Instance.level3HazardSpawnUnlocked ||
            GameProgress.Instance.level3QuestStage >= unlockAtQuestStage)
        {
            unlocked = true;
            hazardToToggle.SetActive(true);

            GameProgress.Instance.level3HazardSpawnUnlocked = true;
        }
    }
}