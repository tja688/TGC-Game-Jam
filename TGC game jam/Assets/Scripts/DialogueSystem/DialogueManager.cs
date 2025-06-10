using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic; // 需要 List 和 Queue
using Febucci.UI.Core;
using System; // 需要 Action

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TypewriterCore typewriter;
    [SerializeField] private GameObject dialoguePanel;

    public TypewriterCore Typewriter => typewriter;
    public static DialogueManager Instance { get; private set; }
    public static event Action DialogueFinished;

    public enum DialogueSystemState { Idle, PlayingText, WaitingForContinuation }
    public DialogueSystemState CurrentState { get; private set; } = DialogueSystemState.Idle;
    public bool IsDialogueActive => CurrentState != DialogueSystemState.Idle;

    private Queue<DialogueLineInfo> currentDialogueQueue;
    private bool autoPlayNextLine; 
    private Action onSequenceCompleteCallback; 
    private Transform currentDialogueAnchor; 
    private NPCDialogue currentDialogueData;
    private Camera camera1;

    private struct DialogueLineInfo
    {
        public string Text;
        public Vector2 OriginalScreenPosition; 
        public Transform WorldAnchor;
        public string DialogueID; 
    }

    private void Start()
    {
        camera1 = Camera.main;
    }

    private void Awake()
    {
        if (Instance && !Equals(Instance, this))
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (!dialogueText) Debug.LogError("Dialogue Text (TMP) reference is missing!", this);
        if (!typewriter) Debug.LogError("TypewriterCore component is missing!", this);
        if (!dialoguePanel) Debug.LogWarning("Dialogue Panel reference is missing!", this);

        currentDialogueQueue = new Queue<DialogueLineInfo>();
        typewriter.onTextDisappeared.AddListener(OnCurrentLineFinished); // 打字机播放完一句后调用
    }
    
    private void OnDisable()
    {
        typewriter.onTextDisappeared.RemoveListener(OnCurrentLineFinished);
    }
    
    public void StartDialogueSequence(List<string> dialogueIDs, NPCDialogue dialogueData, Transform anchorTransform, bool autoPlay, Action onComplete = null)
    {
        if (CurrentState != DialogueSystemState.Idle)
        {
            Debug.LogWarning("DialogueManager is already busy. Cannot start new sequence.");
            onComplete?.Invoke(); // 如果有完成回调，立即调用表示未执行
            return;
        }

        if (dialogueIDs == null || dialogueIDs.Count == 0)
        {
            Debug.LogWarning("Dialogue sequence is null or empty.");
            onComplete?.Invoke();
            return;
        }
        
        PlayerMove.CanPlayerMove = false; 
        CurrentState = DialogueSystemState.PlayingText; 
        this.autoPlayNextLine = autoPlay;
        this.onSequenceCompleteCallback = onComplete;
        this.currentDialogueAnchor = anchorTransform; 
        this.currentDialogueData = dialogueData; 

        currentDialogueQueue.Clear();

        foreach (string id in dialogueIDs)
        {
            string text = dialogueData.GetDialogueTextByID(id);
            if (!string.IsNullOrEmpty(text))
            {
                currentDialogueQueue.Enqueue(new DialogueLineInfo { DialogueID = id, WorldAnchor = anchorTransform });
            }
            else
            {
                Debug.LogWarning($"Dialogue ID '{id}' not found or empty in '{dialogueData.name}'. Skipping.");
            }
        }

        if (currentDialogueQueue.Count > 0)
        {
            if (dialoguePanel) dialoguePanel.SetActive(true);
            ShowNextDialogueInQueue();
        }
        else
        {
            Debug.LogWarning("No valid dialogues in the sequence.");
            EndSequenceInternal();
        }
    }

    private void ShowNextDialogueInQueue()
    {
        if (currentDialogueQueue.Count > 0)
        {
            DialogueLineInfo lineInfo = currentDialogueQueue.Dequeue();
            string textToShow = currentDialogueData.GetDialogueTextByID(lineInfo.DialogueID); 

            Vector2 screenPosition;
            if (lineInfo.WorldAnchor && camera1) 
            {
                screenPosition = camera1.WorldToScreenPoint(lineInfo.WorldAnchor.position);
            }
            else if (currentDialogueAnchor && camera1) 
            {
                 screenPosition = camera1.WorldToScreenPoint(currentDialogueAnchor.position);
            }
            else
            {
                Debug.LogWarning("Dialogue anchor or Main Camera is missing. Using center screen.");
                screenPosition = new Vector2(Screen.width / 2, Screen.height / 4); 
            }

            MoveToPosition(screenPosition);
            typewriter.ShowText(textToShow);
            CurrentState = DialogueSystemState.PlayingText; 
        }
        else
        {
            EndSequenceInternal();
        }
    }

    private void OnCurrentLineFinished()
    {
        if (currentDialogueQueue.Count > 0)
        {
            ShowNextDialogueInQueue();
        }
        else
        {
            EndSequenceInternal();
        }
    }
    
    public void ProceedToNextLine()
    {
        if (CurrentState == DialogueSystemState.WaitingForContinuation)
        {
            PlayerMove.CanPlayerMove = false; 
            ShowNextDialogueInQueue();
        }
        else
        {
            Debug.LogWarning("Cannot proceed: DialogueManager is not waiting for continuation or is idle.");
        }
    }

    private void MoveToPosition(Vector2 screenPosition)
    {
        if (dialogueText)
        {
            dialogueText.rectTransform.position = screenPosition;
        }
    }
    
    private void EndSequenceInternal()
    {
        if (dialoguePanel) dialoguePanel.SetActive(false);
        PlayerMove.CanPlayerMove = true; 


        var tempCallback = onSequenceCompleteCallback;
        
        CurrentState = DialogueSystemState.Idle;
        onSequenceCompleteCallback = null; 
        currentDialogueData = null;
        currentDialogueAnchor = null;
        currentDialogueQueue.Clear(); 

        tempCallback?.Invoke();
        DialogueFinished?.Invoke(); 
    }
    
}