using System.Collections;
using UnityEngine;

public class BossEncounterController : MonoBehaviour
{
    [Header("Boss Setup")]
    [SerializeField] private GameObject bossRoot;

    [Header("Dialogue Timing (temp until Ink is hooked up)")]
    [SerializeField] private float firstIntroDialogueDuration = 2.5f;
    [SerializeField] private float reEntryDialogueDuration = 1.8f;
    [SerializeField] private float bossDeathLineDuration = 2.2f;
    [SerializeField] private float retryPlayerLineDuration = 1.8f;

    [Header("Dialogue Lines (temp placeholders)")]
    [TextArea] [SerializeField] private string firstIntroLine = "Hm.. something is off about this place...";
    [TextArea] [SerializeField] private string reEntryLine = "Here we go again...";
    [TextArea] [SerializeField] private string bossDeathLine = "MY ATTACKS HAVE NO AFFECT ON YOU? WHO DECIDED THAT.";
    [TextArea] [SerializeField] private string retryPlayerLine = "Oh.. that didn't go well.";
    [SerializeField] private TextAsset inkJSON;

    [Header("References")]
    [SerializeField] private DeathScreenUI deathScreenUI;

    private bool encounterStarted = false;
    private bool bossFightActive = false;
    private bool deathSequencePlaying = false;
    private bool hasEnteredBefore = true;

    public static BossEncounterController ActiveEncounter { get; private set; }

    private void Awake()
    {
        ActiveEncounter = this;
    }

    private void OnDestroy()
    {
        if (ActiveEncounter == this)
            ActiveEncounter = null;
    }

    private void Start()
    {
        if (bossRoot != null)
            bossRoot.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (encounterStarted) return;

        encounterStarted = true;
        StartCoroutine(BeginEncounterRoutine());
    }

    private IEnumerator BeginEncounterRoutine()
    {
        if (!hasEnteredBefore)
        {
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "bossdialogue");
            hasEnteredBefore = true;
        }
        else
        {
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, "bossdialogue");
            yield return StartCoroutine(PlayTempDialogue(reEntryLine, reEntryDialogueDuration));
        }

        if (bossRoot != null)
            bossRoot.SetActive(true);

        bossFightActive = true;

        Debug.Log("Boss fight started");
    }

    public bool ShouldOverrideDeathScreen()
    {
        return bossFightActive;
    }

    public void HandleBossFightDeath()
    {
        if (!bossFightActive || deathSequencePlaying)
            return;

        StartCoroutine(BossDeathSequenceRoutine());
    }

    private IEnumerator BossDeathSequenceRoutine()
    {
        deathSequencePlaying = true;

        Time.timeScale = 1f;

        yield return StartCoroutine(PlayTempDialogue(bossDeathLine, bossDeathLineDuration));

        if (deathScreenUI != null)
            deathScreenUI.ShowDeathScreen();

        deathSequencePlaying = false;
    }

    public void OnRetryRespawned()
    {
        StartCoroutine(RetryRespawnDialogueRoutine());
    }

    private IEnumerator RetryRespawnDialogueRoutine()
    {
        Time.timeScale = 1f;
        yield return StartCoroutine(PlayTempDialogue(retryPlayerLine, retryPlayerLineDuration));
    }

    private IEnumerator PlayTempDialogue(string line, float duration)
    {
        Debug.Log("DIALOGUE: " + line);
        yield return new WaitForSeconds(duration);
    }

    public void ResetEncounterState()
    {
        bossFightActive = false;
        deathSequencePlaying = false;

        if (bossRoot != null)
            bossRoot.SetActive(false);

        encounterStarted = false;
    }
}