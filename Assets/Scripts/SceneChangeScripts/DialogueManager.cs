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
        continueArrow.SetActive(false);
        dialogueIsPlaying = false;
        sceneSprite.SetActive(false);
        kazumiSprite.SetActive(false);
        dialoguePanel.SetActive(false);
        crowSprite.SetActive(false);
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
        sceneSprite.SetActive(false);
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();

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
            if (tag == "fadein")
            {
                sceneFader.FadeIn(2f);
                sceneSprite.SetActive(true);
            }

            if (tag == "kazumi" || tag == "kazumipuppyeyes" || tag == "kazumicheerful")
            {
                kazumiSprite.SetActive(true);
                sceneSprite.SetActive(false);
            }

            if (tag == "nozomi")
            {
                sceneSprite.SetActive(true);
                kazumiSprite.SetActive(false);
                crowSprite.SetActive(false);
            }

            if (tag == "crow")
            {
                sceneSprite.SetActive(false);
                kazumiSprite.SetActive(false);
                crowSprite.SetActive(true);
            }

            if (tag == "crowjump")
            {
                crowAnimator.SetTrigger("CrowBooksIt");
                continueArrow.SetActive(true);
            }

            if (tag == "crowdisappear")
            {
                sceneSprite.SetActive(false);
                crowAnimator.SetBool("isDisappearing", true);
                ExitDialogueMode();
            }

            if (tag == "crowappears")
            {
                crowParent.transform.position = crowSecondSpawn.position;
                sceneSprite.SetActive(false);
                kazumiSprite.SetActive(false);
                crowSprite.SetActive(true);
                crowInWorldObject.SetActive(true);
                crowAnimator.SetTrigger("CrowAppears");
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
