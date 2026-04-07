using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private SceneFader sceneFader;
    [SerializeField] private string firstGameScene = "SS_1";

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject newGameConfirmPanel;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    private SoundFXManager audioManager;

    private void Awake()
    {
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
        {
            audioManager = audioObject.GetComponent<SoundFXManager>();
        }
    }

    private void Start()
    {
        if (continueButton != null)
            continueButton.interactable = SaveSystem.HasSave();

        if (newGameConfirmPanel != null)
            newGameConfirmPanel.SetActive(false);

        if (audioManager != null && audioManager.menuUnderline != null)
            audioManager.PlaySFX(audioManager.menuUnderline);

        BackToMainMenu();
    }

    public void NewGame()
    {
        if (audioManager != null && audioManager.menuUnderline != null)
            audioManager.PlaySFX(audioManager.menuUnderline);

        if (SaveSystem.HasSave())
        {
            if (newGameConfirmPanel != null)
                newGameConfirmPanel.SetActive(true);

            return;
        }

        StartFreshGame();
    }

    public void ConfirmNewGameYes()
    {
        if (audioManager != null && audioManager.menuCheck1 != null)
            audioManager.PlaySFX(audioManager.menuCheck1);

        if (newGameConfirmPanel != null)
            newGameConfirmPanel.SetActive(false);

        SaveSystem.DeleteSave();

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.ResetAllProgress();
        }

        StartCoroutine(LoadSceneWithFade(firstGameScene));
    }

    public void ConfirmNewGameNo()
    {
        if (audioManager != null && audioManager.menuCheck2 != null)
            audioManager.PlaySFX(audioManager.menuCheck2);

        if (newGameConfirmPanel != null)
            newGameConfirmPanel.SetActive(false);
    }

    public void ContinueGame()
    {
        if (audioManager != null && audioManager.menuUnderline != null)
            audioManager.PlaySFX(audioManager.menuUnderline);

        SaveData data = SaveSystem.LoadGame();

        if (data == null)
        {
            Debug.LogWarning("No save found");
            return;
        }

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.ApplySaveData(data);
        }

        StartCoroutine(LoadSceneWithFade(data.sceneName));
    }

    public void OpenSettings()
    {
        if (audioManager != null && audioManager.menuCheck1 != null)
            audioManager.PlaySFX(audioManager.menuCheck1);

        ShowOnlyPanel(settingsPanel);
    }

    public void OpenCredits()
    {
        if (audioManager != null && audioManager.menuCheck1 != null)
            audioManager.PlaySFX(audioManager.menuCheck1);

        ShowOnlyPanel(creditsPanel);
    }

    public void OpenControls()
    {
        if (audioManager != null && audioManager.menuCheck1 != null)
            audioManager.PlaySFX(audioManager.menuCheck1);

        ShowOnlyPanel(controlsPanel);
    }

    public void BackToMainMenu()
    {
        if (audioManager != null && audioManager.menuCheck2 != null)
            audioManager.PlaySFX(audioManager.menuCheck2);

        if (newGameConfirmPanel != null)
            newGameConfirmPanel.SetActive(false);

        ShowOnlyPanel(mainMenuPanel);
    }

    public void QuitGame()
    {
        if (audioManager != null && audioManager.menuCheck1 != null)
            audioManager.PlaySFX(audioManager.menuCheck1);

        Application.Quit();
    }

    private void ShowOnlyPanel(GameObject panelToShow)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(panelToShow == mainMenuPanel);
        if (settingsPanel != null) settingsPanel.SetActive(panelToShow == settingsPanel);
        if (creditsPanel != null) creditsPanel.SetActive(panelToShow == creditsPanel);
        if (controlsPanel != null) controlsPanel.SetActive(panelToShow == controlsPanel);
    }

    private void StartFreshGame()
    {
        SaveSystem.DeleteSave();

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.ResetAllProgress();
        }

        StartCoroutine(LoadSceneWithFade(firstGameScene));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        if (sceneFader != null)
        {
            sceneFader.FadeToBlack(1f);
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (newGameConfirmPanel != null && newGameConfirmPanel.activeSelf)
            {
                ConfirmNewGameNo();
                return;
            }

            bool inSubmenu =
                (settingsPanel != null && settingsPanel.activeSelf) ||
                (creditsPanel != null && creditsPanel.activeSelf) ||
                (controlsPanel != null && controlsPanel.activeSelf);

            if (inSubmenu)
                BackToMainMenu();
        }
    }
}