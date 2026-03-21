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

    private void Start()
    {
        if (continueButton != null)
            continueButton.interactable = SaveSystem.HasSave();

        BackToMainMenu();
    }

    public void NewGame()
    {
        SaveSystem.DeleteSave();

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.ResetAllProgress();
        }

        StartCoroutine(LoadSceneWithFade(firstGameScene));
    }

    public void ContinueGame()
    {
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
        ShowOnlyPanel(settingsPanel);
    }

    public void OpenCredits()
    {
        ShowOnlyPanel(creditsPanel);
    }

    public void OpenControls()
    {
        ShowOnlyPanel(controlsPanel);
    }

    public void BackToMainMenu()
    {
        ShowOnlyPanel(mainMenuPanel);
    }

    public void QuitGame()
    {
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