using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")]
    public GameObject visualCue;

    [Header("Ink JSON")]
    public TextAsset inkJSON;
    public string knotName;

    [Header("Quest Reference")]
    public QuestControl questControl;
}