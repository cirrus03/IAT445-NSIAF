using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleDialogueUI : MonoBehaviour
{
    public static SimpleDialogueUI Instance { get; private set; }

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject continuePrompt;

    private Queue<string> lines = new Queue<string>();
    private bool dialogueActive = false;
    private System.Action onDialogueFinished;

    public bool DialogueActive => dialogueActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continuePrompt != null)
            continuePrompt.SetActive(false);

    }

    private void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
    }

    public void StartDialogue(IEnumerable<string> dialogueLines, System.Action onFinished = null)
    {
        lines.Clear();

        foreach (string line in dialogueLines)
        {
            lines.Enqueue(line);
        }

        onDialogueFinished = onFinished;
        dialogueActive = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        string nextLine = lines.Dequeue();

        if (dialogueText != null)
            dialogueText.text = nextLine;

        if (continuePrompt != null)
            continuePrompt.SetActive(lines.Count > 0);
    }

    private void EndDialogue()
    {
        dialogueActive = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continuePrompt != null)
            continuePrompt.SetActive(false);

        onDialogueFinished?.Invoke();
        onDialogueFinished = null;
    }
}