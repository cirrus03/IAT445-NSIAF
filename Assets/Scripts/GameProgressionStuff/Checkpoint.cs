using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Respawn Point")]
    [SerializeField] private Transform respawnPoint;

    public void ActivateCheckpoint()
    {
        if (CheckpointManager.Instance == null)
            return;

        if (CheckpointManager.Instance.GetCurrentCheckpoint() == this)
            return;

        CheckpointManager.Instance.SetCheckpoint(this);
        Debug.Log("Activated checkpoint: " + name);
    }

    public Vector3 GetRespawnPosition()
    {
        if (respawnPoint != null)
            return respawnPoint.position;

        return transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        ActivateCheckpoint();
    }
}