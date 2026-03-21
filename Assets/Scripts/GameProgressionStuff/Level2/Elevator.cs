using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private bool playerInRange = false;

    [Header("Elevator Settings")]
    [SerializeField] private Transform destination;
    [SerializeField] private bool requiresPower = true;
    [SerializeField] private bool poweredOn = false;

    [Header("Optional Quest Gate")]
    [SerializeField] private bool requireLevel2QuestStage = false;
    [SerializeField] private int requiredLevel2QuestStage = 3;
    [SerializeField] private string lockedQuestMessage = "Looks like Fox still has something to say.";

    [Header("Camera")]
    [SerializeField] private Behaviour cmCameraBehaviour;

    [Header("Messages")]
    [SerializeField] private string noPowerMessage = "Looks like it's not working right now.";
    [SerializeField] private string useMessage = "Hehe uppies.";

    private void Update()
    {
        if (PauseMenu.isPaused)
            return;

        if (interactPrompt != null)
            interactPrompt.SetActive(playerInRange);

        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }
    }

    public void SetPoweredOn(bool value)
    {
        poweredOn = value;
    }

    public void Interact()
    {
        if (requiresPower && !poweredOn)
        {
            Debug.Log(noPowerMessage);
            return;
        }

        if (requireLevel2QuestStage)
        {
            if (GameProgress.Instance == null)
            {
                Debug.LogWarning("Missing GameProgress in scene.");
                return;
            }

            if (GameProgress.Instance.level2QuestStage < requiredLevel2QuestStage)
            {
                Debug.Log(lockedQuestMessage);
                return;
            }
        }

        Debug.Log(useMessage);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && destination != null)
        {
            StartCoroutine(TeleportPlayer(player));
        }
        else
        {
            Debug.LogWarning("Elevator missing player or destination.");
        }
    }

    private IEnumerator TeleportPlayer(GameObject player)
    {
        if (cmCameraBehaviour != null)
            cmCameraBehaviour.enabled = false;

        player.transform.position = destination.position;

        if (Camera.main != null)
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = destination.position.x;
            camPos.y = destination.position.y;
            Camera.main.transform.position = camPos;
        }

        yield return new WaitForEndOfFrame();
        yield return null;

        if (cmCameraBehaviour != null)
            cmCameraBehaviour.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }
}