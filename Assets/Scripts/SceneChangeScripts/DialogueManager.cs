using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private SceneFader sceneFader;
    [SerializeField] private GameObject sceneSprite;
    [SerializeField] private GameObject kazumiSprite;
    [SerializeField] private GameObject continueArrow;
    [SerializeField] private GameObject crowSprite;
    [SerializeField] private Animator crowAnimator;
    [SerializeField] private GameObject crowInWorldObject;
    [SerializeField] private GameObject crowParent;
    [SerializeField] private Transform crowSecondSpawn;
    [SerializeField] private GameObject whitecrowSprite;
    [SerializeField] private GameObject blackcrowSprite;


    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }
    public bool dialogueFinished { get; private set; }
    private static DialogueManager instance;

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
        if (continueArrow != null) continueArrow.SetActive(false);
        if (sceneSprite != null) sceneSprite.SetActive(false);
        if (kazumiSprite != null) kazumiSprite.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (crowSprite != null) crowSprite.SetActive(false);
        if (whitecrowSprite != null) whitecrowSprite.SetActive(false);
        if (blackcrowSprite != null) blackcrowSprite.SetActive(false);

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
        if (InputManager.GetInstance().GetSubmitPressed() || Input.GetMouseButtonDown(0))
        {
            ContinueStory();
            InputManager.GetInstance().RegisterSubmitPressed();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON, string knotName = "")
    {
        InputManager.GetInstance().RegisterSubmitPressed();

        currentStory = new Story(inkJSON.text);

        if (!string.IsNullOrEmpty(knotName))
        {
            currentStory.ChoosePathString(knotName);
        }

        dialogueIsPlaying = true;
        dialogueFinished = false;

        dialoguePanel.SetActive(true);
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialogueFinished = true;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        if (sceneSprite != null) sceneSprite.SetActive(false);
        if (whitecrowSprite != null) whitecrowSprite.SetActive(false);
        if (blackcrowSprite != null) blackcrowSprite.SetActive(false);
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
            if (tag == "fadein")
            {
                if (sceneFader != null) sceneFader.FadeIn(2f);
                if (sceneSprite != null) sceneSprite.SetActive(true);
            }

            if (tag == "kazumi" || tag == "kazumipuppyeyes" || tag == "kazumicheerful")
            {
                if (kazumiSprite != null) kazumiSprite.SetActive(true);
                if (sceneSprite != null) sceneSprite.SetActive(false);
            }

            if (tag == "nozomi")
            {
                if (sceneSprite != null) sceneSprite.SetActive(true);
                if (kazumiSprite != null) kazumiSprite.SetActive(false);
                if (crowSprite != null) crowSprite.SetActive(false);
                if (whitecrowSprite != null) whitecrowSprite.SetActive(false);
                if (blackcrowSprite != null) blackcrowSprite.SetActive(false);
            }

            if (tag == "crow")
            {
                if (sceneSprite != null) sceneSprite.SetActive(false);
                if (kazumiSprite != null) kazumiSprite.SetActive(false);
                if (crowSprite != null) crowSprite.SetActive(true);
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
                if (sceneSprite != null) sceneSprite.SetActive(false);
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
                if (sceneSprite != null) sceneSprite.SetActive(false);
                if (kazumiSprite != null) kazumiSprite.SetActive(false);
                if (crowSprite != null) crowSprite.SetActive(true);
                if (crowInWorldObject != null) crowInWorldObject.SetActive(true);
                if (crowAnimator != null)
                {
                    crowAnimator.SetTrigger("CrowAppears");
                }
            }

            if (tag == "whiteduocrow")
            {
                if (sceneSprite != null) sceneSprite.SetActive(false);
                if (blackcrowSprite != null) blackcrowSprite.SetActive(false);
                if (whitecrowSprite != null) whitecrowSprite.SetActive(true);
            }

            if (tag == "blackduocrow")
            {
                if (sceneSprite != null) sceneSprite.SetActive(false);
                if (blackcrowSprite != null) blackcrowSprite.SetActive(true);
                if (whitecrowSprite != null) whitecrowSprite.SetActive(false);
            }
            // quest stuff pls
            if (tag == "start_first_quest")
            {
                QuestManager.Instance.StartKillQuest("kill_first_enemy", "Defeat 1 Enemy", 1);
                GameProgress.Instance.currentQuestStage = 1;
            }

            if (tag == "start_second_quest")
            {
                QuestManager.Instance.StartCollectQuest("collect_top_right_item", "Collect the Upper-Right Item", 1);
            }

            if (tag == "start_third_quest")
            {
                QuestManager.Instance.StartCollectQuest("collect_final_item", "Collect the Final Item", 1);
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
                GameObject portal = GameObject.FindWithTag("LevelExitPortal");
                if (portal != null) portal.SetActive(true);
            }
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
    }
}
