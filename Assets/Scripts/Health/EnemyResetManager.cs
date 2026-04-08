using UnityEngine;

public class EnemyResetManager : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerHealth.PlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        PlayerHealth.PlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        ResetPlacedEnemies();
        ResetBosses();
        DespawnSpawnedEnemies();
    }

    private void ResetPlacedEnemies()
    {
        EnemyResettable[] resettableEnemies = FindObjectsByType<EnemyResettable>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var enemy in resettableEnemies)
        {
            if (enemy != null)
                enemy.ResetEnemy();
        }
    }

    private void DespawnSpawnedEnemies()
    {
        SpawnedEnemyMarker[] spawnedEnemies = FindObjectsByType<SpawnedEnemyMarker>(FindObjectsSortMode.None);

        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy.gameObject);
        }
    }

    private void ResetBosses()
    {
        BossResettable[] bosses = FindObjectsByType<BossResettable>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var boss in bosses)
        {
            if (boss != null)
                boss.ResetBoss();
        }
    }
}