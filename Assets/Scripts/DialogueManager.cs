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

    [Header("Quest UI")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questText;
    private bool questIsActive;
    public bool QuestIsActive => questIsActive;
    private string waitingQuestDescription;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set; }
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
        questIsActive = false;
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }



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
        if (!dialogueIsPlaying)
        {
            return;
        }

        if (InputManager.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {

        // RESET QUEST STATE FORJHTE LOVE OF GOD
        questIsActive = false;
        waitingQuestDescription = null;
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }

        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private void EnterQuestMode(string questDescription)
    {
        if (questPanel == null || string.IsNullOrEmpty(questDescription))
            return;

        questIsActive = true;
        questPanel.SetActive(true);
        questText.text = questDescription;
    }


    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        if (!string.IsNullOrEmpty(waitingQuestDescription))
        {
            EnterQuestMode(waitingQuestDescription);
            waitingQuestDescription = null;
        }
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            // if there are
            HandleTags(currentStory.currentTags);
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
            if (tag.StartsWith("QUEST:"))
            {
                waitingQuestDescription = tag.Substring(6).Trim();
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
                choices[i].gameObject.SetActive(false);
            }
            return;
        }

        int index = 0;
        // ink configuring stuff and stuff
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int ChoiceIndex)
    {
        currentStory.ChooseChoiceIndex(ChoiceIndex);
    }
}
