using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneFader sceneFader;
    public void PlayGame()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        sceneFader.FadeToBlack(2f);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
