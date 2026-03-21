using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitPortal2 : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        SceneManager.LoadScene(nextSceneName);
    }
}