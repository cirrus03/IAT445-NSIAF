using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public enum Speaker
{
    None,
    Fox,
    Crow,
    Kazumi,
    Nozomi
}

public class DialogueManager : MonoBehaviour
{

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private SceneFader sceneFader;
    [SerializeField] private GameObject sceneSprite;
    [SerializeField] private GameObject nozomiHappy;
    [SerializeField] private GameObject nozomiNeutral;
    [SerializeField] private GameObject nozomiAngry;
    [SerializeField] private GameObject kazumiSprite;
    [SerializeField] private GameObject kazumiCheer;
    [SerializeField] private GameObject kazumiPuppyeyes;
    [SerializeField] private GameObject kazumiNeutral;
    [SerializeField] private GameObject foxSprite;
    [SerializeField] private GameObject altFoxSprite;
    [SerializeField] private GameObject continueArrow;
    [SerializeField] private GameObject crowSprite;
    [SerializeField] private Animator crowAnimator;
    [SerializeField] private GameObject crowInWorldObject;
    [SerializeField] private GameObject crowParent;
    [SerializeField] private Transform crowSecondSpawn;
    [SerializeField] private GameObject whitecrowSprite;
    [SerializeField] private GameObject blackcrowSprite;
    [SerializeField] private GameObject levelExitPortal;
    [SerializeField] private TutorialEnemyRespawner tutorialEnemyRespawner;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }
    public bool dialogueFinished { get; private set; }
    private bool isDowntimeLine = false;
    private string simpleText;
    private static DialogueManager instance;
    private bool frozeWorldForDialogue = false;

    [Header("Dialogue Freeze")]
    [SerializeField] private bool freezeGameplayDuringDialogue = false;

    [Header("Popup Text")]
    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField] private float timedPopupDuration = 1.6f;
    [SerializeField] private float timedPopupFadeOutTime = 0.6f;

    private Coroutine timedPopupRoutine;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("girl theres more than one");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        frozeWorldForDialogue = false;

        foreach (GameObject choice in choices)
        {
            if (choice != null)
                choice.SetActive(false);
        }
        HideAllSprites();
        if (continueArrow != null) continueArrow.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // choices
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (PauseMenu.isPaused) return;
        if (!dialogueIsPlaying) return;
        if (currentStory.currentChoices.Count > 0) return;
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isDowntimeLine)
            {
                ExitDialogueMode();
                isDowntimeLine = false;
            }
            else
            {
                ContinueStory();
            }

        }
    }

    public void EnterDialogueMode(TextAsset inkJSON, string knotName = "")
    {
        Debug.Log("inkJSON null? " + (inkJSON == null));
        Debug.Log("dialoguePanel null? " + (dialoguePanel == null));
        Debug.Log("InputManager null? " + (InputManager.GetInstance() == null));

        currentStory = new Story(inkJSON.text);

        if (!string.IsNullOrEmpty(knotName))
        {
            currentStory.ChoosePathString(knotName);
        }

        dialogueIsPlaying = true;
        dialogueFinished = false;
        FreezeWorldForDialogue();
        dialoguePanel.SetActive(true);
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialogueFinished = true;
        isDowntimeLine = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        UnfreezeWorldAfterDialogue();
        HideAllSprites();
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            string text = currentStory.Continue();

            HandleTags(currentStory.currentTags);

            // skip empty lines
            if (string.IsNullOrWhiteSpace(text))
            {
                ContinueStory();
                return;
            }

            if (dialogueText != null)
                dialogueText.text = text;
            DisplayChoices();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void HandleTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (tag == "nextlevel")
            {
                StartCoroutine(NextLevelTransition("MINDSCAPE: The Fox's Attic"));
            }
            if (tag == "fadein")
            {
                if (sceneFader != null) sceneFader.FadeIn(2f);
                if (sceneSprite != null) sceneSprite.SetActive(true);
            }

            if (tag == "kazumi")
            {
                HideAllSprites();
                if (kazumiSprite != null) kazumiSprite.SetActive(true);
            }

            if (tag == "kazumipuppyeyes")
            {
                HideAllSprites();
                if (kazumiPuppyeyes != null) kazumiPuppyeyes.SetActive(true);
            }

            if (tag == "kazumicheerful")
            {
                HideAllSprites();
                if (kazumiCheer != null) kazumiCheer.SetActive(true);
            }

            if (tag == "kazumineutral")
            {
                HideAllSprites();
                if (kazumiNeutral != null) kazumiNeutral.SetActive(true);
            }

            if (tag == "nozomi")
            {
                HideAllSprites();
                if (sceneSprite != null) sceneSprite.SetActive(true);
            }

            if (tag == "nozomihappy")
            {
                HideAllSprites();
                if (nozomiHappy != null) nozomiHappy.SetActive(true);
            }

            if (tag == "nozomineutral")
            {
                HideAllSprites();
                if (nozomiNeutral != null) nozomiNeutral.SetActive(true);
            }

            if (tag == "nozomiangry")
            {
                HideAllSprites();
                if (nozomiAngry != null) nozomiAngry.SetActive(true);
            }

            if (tag == "crow")
            {
                HideAllSprites();
                if (crowSprite != null) crowSprite.SetActive(true);
            }

            if (tag == "fox")
            {
                HideAllSprites();
                if (foxSprite != null) foxSprite.SetActive(true);
            }

            if (tag == "altfox")
            {
                HideAllSprites();
                if (altFoxSprite != null) altFoxSprite.SetActive(true);
            }

            if (tag == "crowjump")
            {
                if (crowAnimator != null)
                {
                    crowAnimator.SetTrigger("CrowBooksIt");
                }
                if (continueArrow != null) continueArrow.SetActive(true);
            }

            if (tag == "crowdisappear")
            {
                HideAllSprites();
                if (crowAnimator != null)
                {
                    crowAnimator.SetBool("isDisappearing", true);
                }
                ExitDialogueMode();
            }
            if (tag == "crowappears")
            {
                if (crowParent != null && crowSecondSpawn != null)
                {
                    crowParent.transform.position = crowSecondSpawn.position;
                }

                // enable crow anyway
                if (crowParent != null && (crowSecondSpawn == null || crowAnimator == null))
                {
                    crowParent.SetActive(true);
                }

                HideAllSprites();
                if (crowSprite != null) crowSprite.SetActive(true);
                if (crowInWorldObject != null) crowInWorldObject.SetActive(true);

                if (crowAnimator != null)
                {
                    crowAnimator.SetTrigger("CrowAppears");
                }
            }

            if (tag == "whiteduocrow")
            {
                HideAllSprites();
                if (whitecrowSprite != null) whitecrowSprite.SetActive(true);
            }

            if (tag == "blackduocrow")
            {
                HideAllSprites();
                if (blackcrowSprite != null) blackcrowSprite.SetActive(true);
            }
            // quest stuff pls
            if (tag == "start_first_quest")
            {
                QuestManager.Instance.StartKillQuest("kill_first_enemy", "Defeat 1 Enemy", 1);
                GameProgress.Instance.currentQuestStage = 1;
                if (tutorialEnemyRespawner != null)
                {
                    tutorialEnemyRespawner.EnsureQuestEnemyExists();
                }
                else
                {
                    Debug.LogWarning("DialogueManager: tutorialEnemyRespawner is not assigned.");
                }
            }

            if (tag == "start_second_quest")
            {
                QuestManager.Instance.StartCollectQuest("collect_top_right_item", "Collect the Upper-Right Item", 1);
            }

            if (tag == "start_third_quest")
            {
                QuestManager.Instance.StartCollectQuest("collect_final_item", "Collect the Final Item", 1);
            }

            // level 1 quest tags

            if (tag == "fox_give_lamp")
            {
                DarknessController.Instance?.GiveLamp();

                GameProgress.Instance.level2LampAcquired = true;
                GameProgress.Instance.level2QuestStage = 1;
                GameProgress.Instance.SetObjective("Find the breaker");
            }

            if (tag == "fox_power_restored")
            {
                GameProgress.Instance.level2QuestStage = 3;
                GameProgress.Instance.level2BugQuestStarted = true;

                BugQuestGroup bugQuest = FindObjectOfType<BugQuestGroup>();
                if (bugQuest != null)
                {
                    bugQuest.BeginQuest();

                    GameProgress.Instance.SetObjective(
                        "Clear the attic bugs",
                        bugQuest.CurrentKilled + " / " + bugQuest.RequiredKills
                    );
                }
            }

            if (tag == "fox_bugs_cleared")
            {
                GameProgress.Instance.level2BugQuestComplete = true;
                GameProgress.Instance.level2QuestStage = 4;
                GameProgress.Instance.SetObjective("Enter the portal");
            }

            if (tag == "fox_open_portal")
            {
                GameProgress.Instance.level2QuestStage = 5;

                if (levelExitPortal != null)
                    levelExitPortal.SetActive(true);
            }

            // ability unlocks
            if (tag == "unlockdash")
            {
                GameProgress.Instance.UnlockDash();

                if (FindObjectOfType<AbilityUnlocker>()?.player != null)
                    FindObjectOfType<AbilityUnlocker>().player.SetDashUnlocked(true);

                QuestManager.Instance.ClearQuest();
                GameProgress.Instance.currentQuestStage = 2;
            }

            if (tag == "unlockwalljump")
            {
                GameProgress.Instance.UnlockWallJump();

                if (FindObjectOfType<AbilityUnlocker>()?.player != null)
                    FindObjectOfType<AbilityUnlocker>().player.SetWallJumpUnlocked(true);

                QuestManager.Instance.ClearQuest();
                GameProgress.Instance.currentQuestStage = 3;
            }

            if (tag == "unlockdoublejump")
            {
                GameProgress.Instance.UnlockDoubleJump();

                if (FindObjectOfType<AbilityUnlocker>()?.player != null)
                    FindObjectOfType<AbilityUnlocker>().player.SetDoubleJumpUnlocked(true);

                QuestManager.Instance.ClearQuest();
                GameProgress.Instance.currentQuestStage = 4;
            }

            if (tag == "openportal")
            {
                if (levelExitPortal != null)
                    levelExitPortal.SetActive(true);
            }

            if (tag.StartsWith("setMood:"))
            {
                string mood = tag.Split(':')[1].ToLower();

                switch (mood)
                {
                    case "happy":
                        GameProgress.Instance.SetPlayerMood(GameProgress.MoodState.Happy);
                        break;
                    case "sad":
                        GameProgress.Instance.SetPlayerMood(GameProgress.MoodState.Sad);
                        break;
                    case "angry":
                        GameProgress.Instance.SetPlayerMood(GameProgress.MoodState.Angry);
                        break;
                    default:
                        GameProgress.Instance.SetPlayerMood(GameProgress.MoodState.Neutral);
                        break;
                }
                Debug.Log("Mood tag received: " + tag);
            }
        }
    }

    public void PlayDowntimeDialogue(string line, Speaker speaker = Speaker.None)
    {
        dialogueIsPlaying = true;
        dialogueFinished = false;

        FreezeWorldForDialogue();

        isDowntimeLine = true;
        simpleText = line;

        dialoguePanel.SetActive(true);
        dialogueText.text = line;

        HideAllSprites();

        GameObject spriteToShow = GetSpeaker(speaker);
        if (spriteToShow != null)
            spriteToShow.SetActive(true);
    }

    public void PlayTimedDowntimeDialogue(string line, Speaker speaker = Speaker.None)
    {
        if (timedPopupRoutine != null)
        {
            StopCoroutine(timedPopupRoutine);
        }

        timedPopupRoutine = StartCoroutine(PlayTimedDowntimeDialogueRoutine(line, speaker));
    }

    private IEnumerator PlayTimedDowntimeDialogueRoutine(string line, Speaker speaker)
    {
        dialogueIsPlaying = false;
        dialogueFinished = false;
        isDowntimeLine = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (dialogueText != null)
            dialogueText.text = line;

        HideAllSprites();

        GameObject spriteToShow = GetSpeaker(speaker);
        if (spriteToShow != null)
            spriteToShow.SetActive(true);

        if (dialogueCanvasGroup != null)
            dialogueCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(timedPopupDuration);

        float t = 0f;

        if (dialogueCanvasGroup != null)
        {
            while (t < timedPopupFadeOutTime)
            {
                t += Time.deltaTime;
                dialogueCanvasGroup.alpha = 1f - (t / timedPopupFadeOutTime);
                yield return null;
            }

            dialogueCanvasGroup.alpha = 0f;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        HideAllSprites();

        if (dialogueCanvasGroup != null)
            dialogueCanvasGroup.alpha = 1f;

        timedPopupRoutine = null;
    }

    private void HideAllSprites()
    {
        if (kazumiSprite != null) kazumiSprite.SetActive(false);
        if (kazumiCheer != null) kazumiCheer.SetActive(false);
        if (kazumiNeutral != null) kazumiNeutral.SetActive(false);
        if (kazumiPuppyeyes != null) kazumiPuppyeyes.SetActive(false);
        if (sceneSprite != null) sceneSprite.SetActive(false);
        if (nozomiHappy != null) nozomiHappy.SetActive(false);
        if (nozomiNeutral != null) nozomiNeutral.SetActive(false);
        if (nozomiAngry != null) nozomiAngry.SetActive(false);
        if (crowSprite != null) crowSprite.SetActive(false);
        if (foxSprite != null) foxSprite.SetActive(false);
        if (altFoxSprite != null) altFoxSprite.SetActive(false);
        if (whitecrowSprite != null) whitecrowSprite.SetActive(false);
        if (blackcrowSprite != null) blackcrowSprite.SetActive(false);
    }

    public GameObject GetSpeaker(Speaker speaker)
    {
        switch (speaker)
        {
            case Speaker.Fox:
                return foxSprite;
            case Speaker.Crow:
                return whitecrowSprite;
            case Speaker.Kazumi:
                return kazumiSprite;
            case Speaker.Nozomi:
                return sceneSprite;
            default:
                return null;
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count == 0)
        {
            for (int i = 0; i < choices.Length; i++)
            {
                choices[i].SetActive(false);
            }
            return;
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        if (choices.Length == 0) yield break;

        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0]);
    }

    public void MakeChoice(int ChoiceIndex)
    {
        currentStory.ChooseChoiceIndex(ChoiceIndex);
        ContinueStory();
    }

    private IEnumerator NextLevelTransition(string transitionText)
    {
        dialoguePanel.SetActive(false);
        UnfreezeWorldAfterDialogue();
        dialogueIsPlaying = false;
        dialogueFinished = true;
        isDowntimeLine = false;

        if (sceneFader != null)
            sceneFader.FadeToBlack(0.3f);

        yield return new WaitForSecondsRealtime(0.3f);
        if (sceneFader != null)
        {
            yield return sceneFader.StartCoroutine(
                sceneFader.FadeTextRoutine(transitionText, 1f, 2f, 1f)
            );
        }

        Time.timeScale = 1f;
        SceneControl.instance.NextLevel();
    }

    private void FreezeWorldForDialogue()
    {
        if (!freezeGameplayDuringDialogue)
            return;

        if (!PauseMenu.isPaused)
        {
            Time.timeScale = 0f;
            frozeWorldForDialogue = true;
        }
    }

    private void UnfreezeWorldAfterDialogue()
    {
        if (frozeWorldForDialogue)
        {
            Time.timeScale = 1f;
            frozeWorldForDialogue = false;
        }
    }
}
