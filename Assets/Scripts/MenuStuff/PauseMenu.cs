using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenuPanel;
    public GameObject pauseMainPanel;
    public GameObject pauseSettingsPanel;
    public GameObject pauseControlsPanel;

    public static bool isPaused;

    private SoundFXManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundFXManager>();
    }

    void Start()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(true);

        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);

        if (pauseControlsPanel != null)
            pauseControlsPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        if (DeathScreenUI.IsDeathScreenOpen)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                if (pauseSettingsPanel != null && pauseSettingsPanel.activeSelf)
                {
                    OpenMainPausePanel();
                }
                else if (pauseControlsPanel != null && pauseControlsPanel.activeSelf)
                {
                    OpenMainPausePanel();
                }
                else
                {
                    ResumeGame();
                }
            }
        }
    }

    public void PauseGame()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        OpenMainPausePanel();

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenMainPausePanel()
    {

        audioManager.PlaySFX(audioManager.menuCheck1);

        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(true);

        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);

        if (pauseControlsPanel != null)
            pauseControlsPanel.SetActive(false);
    }

    public void OpenSettingsPanel()
    {
        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(false);

        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(true);

        if (pauseControlsPanel != null)
            pauseControlsPanel.SetActive(false);
    }

    public void OpenControlsPanel()
    {
        if (pauseMainPanel != null)
            pauseMainPanel.SetActive(false);

        if (pauseSettingsPanel != null)
            pauseSettingsPanel.SetActive(false);

        if (pauseControlsPanel != null)
            pauseControlsPanel.SetActive(true);
    }

    // public void GoToMainMenu()
    // {
    //     Time.timeScale = 1f;
    //     isPaused = false;
    //     SceneManager.LoadScene("MainMenu");
    // }

    public void SaveAndReturnToTitle()
    {
        SaveSystem.SaveGame();
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        Application.Quit();
    }

    private bool IsDialogueOpen()
    {
        return DialogueManager.GetInstance() != null &&
        DialogueManager.GetInstance().dialogueIsPlaying;
    }
}