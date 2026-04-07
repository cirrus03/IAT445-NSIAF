using System.Collections;
using UnityEngine;

public class SaveLoadInitializer : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;

        SaveData data = SaveSystem.PendingLoadData;
        if (data == null)
            yield break;

        if (GameProgress.Instance != null)
            GameProgress.Instance.ApplySaveData(data);

        if (QuestManager.Instance != null)
            QuestManager.Instance.ApplySaveData(data);

        if (CheckpointManager.Instance != null)
            CheckpointManager.Instance.RestoreCheckpointById(data.checkpointId);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && CheckpointManager.Instance != null && CheckpointManager.Instance.HasCheckpoint())
        {
            player.transform.position = CheckpointManager.Instance.GetRespawnPosition();

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }

        SaveSystem.ClearPendingLoadData();
    }
}