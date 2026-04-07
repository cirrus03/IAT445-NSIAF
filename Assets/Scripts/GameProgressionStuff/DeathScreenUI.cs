using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject deathScreenPanel;

    public static bool IsDeathScreenOpen { get; private set; }

    private void Start()
    {
        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(false);

        IsDeathScreenOpen = false;
    }

    public void ShowDeathScreen()
    {
        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(true);

        Time.timeScale = 0f;
        IsDeathScreenOpen = true;
    }

    public void HideDeathScreen()
    {
        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(false);

        Time.timeScale = 1f;
        IsDeathScreenOpen = false;
    }

    public void RetryFromCheckpoint()
    {
        Time.timeScale = 1f;
        IsDeathScreenOpen = false;

        CheckpointManager checkpoint = FindFirstObjectByType<CheckpointManager>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && checkpoint != null)
        {
            checkpoint.RespawnPlayerAtCheckpoint(player);
        }

        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(false);
    }

    public void SaveAndQuit()
    {
        SaveSystem.SaveGame();

        Time.timeScale = 1f;
        IsDeathScreenOpen = false;
        SceneManager.LoadScene("02_MainMenu");
    }
}