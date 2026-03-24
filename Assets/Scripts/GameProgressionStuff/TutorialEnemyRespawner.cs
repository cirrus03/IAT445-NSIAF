using UnityEngine;

public class TutorialEnemyRespawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;

    public void EnsureQuestEnemyExists()
    {
        TutorialQuestEnemy existingEnemy = FindFirstObjectByType<TutorialQuestEnemy>();

        if (existingEnemy == null)
        {
            SpawnEnemy();
            Debug.Log("YOU KILLED ME? WHO DECIDED THAT.");
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("TutorialEnemyRespawner missing prefab or spawn point.");
            return;
        }

        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }
}