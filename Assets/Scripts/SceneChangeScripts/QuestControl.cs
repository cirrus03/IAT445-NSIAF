using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class QuestControl : MonoBehaviour
{
    [Header("Quest UI")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questText;

    [Header("Quest Data")]
    [TextArea]
    [SerializeField] private string questDescription;
    private bool questShown = false;
    public bool QuestShown => questShown;

    void Start()
    {
        questPanel.SetActive(false);
    }

    void Update()
    {
        if (questShown)
            return;

        if (!DialogueManager.GetInstance().dialogueFinished)
            return;

        ShowQuest();
    }

    private void ShowQuest()
    {
        questShown = true;
        questPanel.SetActive(true);
        questText.text = questDescription;
    }
}
