using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private SceneFader sceneFader;
    [SerializeField] private string firstGameScene = "NewSidescroll";

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    private SoundFXManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundFXManager>();
    }

    private void Start()
    {
        if (continueButton != null)
            continueButton.interactable = SaveSystem.HasSave();

        audioManager.PlaySFX(audioManager.menuUnderline);
        BackToMainMenu();
    }

    public void NewGame()
    {   
        audioManager.PlaySFX(audioManager.menuUnderline);

        SaveSystem.DeleteSave();

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.ResetAllProgress();
        }

        StartCoroutine(LoadSceneWithFade(firstGameScene));
    }

    public void ContinueGame()
    {
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
        audioManager.PlaySFX(audioManager.menuCheck1);
        ShowOnlyPanel(settingsPanel);
    }

    public void OpenCredits()
    {
        audioManager.PlaySFX(audioManager.menuCheck1);
        ShowOnlyPanel(creditsPanel);
    }

    public void OpenControls()
    {
        audioManager.PlaySFX(audioManager.menuCheck1);
        ShowOnlyPanel(controlsPanel);
    }

    public void BackToMainMenu()
    {
        audioManager.PlaySFX(audioManager.menuCheck2);
        ShowOnlyPanel(mainMenuPanel);
    }

    public void QuitGame()
    {   audioManager.PlaySFX(audioManager.menuCheck1);
        Application.Quit();
    }

    private void ShowOnlyPanel(GameObject panelToShow)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(panelToShow == mainMenuPanel);
        if (settingsPanel != null) settingsPanel.SetActive(panelToShow == settingsPanel);
        if (creditsPanel != null) creditsPanel.SetActive(panelToShow == creditsPanel);
        if (controlsPanel != null) controlsPanel.SetActive(panelToShow == controlsPanel);
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
            bool inSubmenu =
                (settingsPanel != null && settingsPanel.activeSelf) ||
                (creditsPanel != null && creditsPanel.activeSelf) ||
                (controlsPanel != null && controlsPanel.activeSelf);

            if (inSubmenu)
                BackToMainMenu();
        }
    }
}