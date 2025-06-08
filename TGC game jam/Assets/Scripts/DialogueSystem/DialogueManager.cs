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

    // [SerializeField] public bool isDialogueProcessActive; // 将通过CurrentState管理
    public TypewriterCore Typewriter => typewriter;
    public static DialogueManager Instance { get; private set; }
    public static event Action DialogueFinished; // 整个序列完成时触发

    // 新增：对话系统状态
    public enum DialogueSystemState { Idle, PlayingText, WaitingForContinuation }
    public DialogueSystemState CurrentState { get; private set; } = DialogueSystemState.Idle;
    public bool IsDialogueActive => CurrentState != DialogueSystemState.Idle; // 替代旧的 isDialogueProcessActive 公开访问

    private Queue<DialogueLineInfo> currentDialogueQueue;
    private bool autoPlayNextLine; // 当前序列是否自动播放
    private Action onSequenceCompleteCallback; // 当前序列完成时的特定回调
    private Transform currentDialogueAnchor; // 当前对话锚点，用于重新计算位置
    private NPCDialogue currentDialogueData; // 当前对话使用的数据源
    private Camera camera1;

    private struct DialogueLineInfo
    {
        public string Text;
        public Vector2 OriginalScreenPosition; // 记录原始计算的屏幕位置
        public Transform WorldAnchor; // 世界坐标锚点，用于动态更新位置
        public string DialogueID; // 对话ID，用于获取文本
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

    private void OnEnable()
    {
        // EventCenter.RemoveListener<Vector2, string>(GameEvents.ShowDialogue, HandleShowDialogueEvent); // 旧事件不再直接使用
    }

    private void OnDisable()
    {
        // EventCenter.RemoveListener<Vector2, string>(GameEvents.ShowDialogue, HandleShowDialogueEvent);
        typewriter.onTextDisappeared.RemoveListener(OnCurrentLineFinished);
    }

    /// <summary>
    /// 启动一个对话序列。
    /// </summary>
    /// <param name="dialogueIDs">要播放的对话ID列表。</param>
    /// <param name="dialogueData">包含这些ID的NPCDialogue数据。</param>
    /// <param name="anchorTransform">对话框锚点的Transform，用于定位。</param>
    /// <param name="autoPlay">true则自动播放下一句，false则每句后等待交互。</param>
    /// <param name="onComplete">整个序列完成后的回调。</param>
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
        
        PlayerMove.CanPlayerMove = false; // 序列开始，禁止玩家移动
        CurrentState = DialogueSystemState.PlayingText; // 初始状态为播放
        this.autoPlayNextLine = autoPlay;
        this.onSequenceCompleteCallback = onComplete;
        this.currentDialogueAnchor = anchorTransform; // 保存锚点
        this.currentDialogueData = dialogueData; // 保存对话数据源

        currentDialogueQueue.Clear();

        foreach (string id in dialogueIDs)
        {
            string text = dialogueData.GetDialogueTextByID(id);
            if (!string.IsNullOrEmpty(text))
            {
                // 位置将在ShowNextDialogueInQueue中根据锚点动态计算
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
            string textToShow = currentDialogueData.GetDialogueTextByID(lineInfo.DialogueID); // 重新获取，以防万一

            Vector2 screenPosition;
            if (lineInfo.WorldAnchor && camera1) // 确保有锚点和相机
            {
                screenPosition = camera1.WorldToScreenPoint(lineInfo.WorldAnchor.position);
            }
            else if (currentDialogueAnchor && camera1) // Fallback to sequence anchor
            {
                 screenPosition = camera1.WorldToScreenPoint(currentDialogueAnchor.position);
            }
            else
            {
                Debug.LogWarning("Dialogue anchor or Main Camera is missing. Using center screen.");
                screenPosition = new Vector2(Screen.width / 2, Screen.height / 4); // 默认位置
            }

            MoveToPosition(screenPosition);
            typewriter.ShowText(textToShow);
            CurrentState = DialogueSystemState.PlayingText; // 确保状态为播放中
        }
        else
        {
            EndSequenceInternal(); // 队列为空，结束序列
        }
    }

    private void OnCurrentLineFinished()
    {
        // 当前行文本已完全显示
        if (currentDialogueQueue.Count > 0) // 如果队列中还有对话
        {
            // 移除 if (autoPlayNextLine) 判断，直接调用 ShowNextDialogueInQueue()
            // 这会强制所有对话序列都自动播放下一句，无论原始 autoPlay 参数是什么。
            ShowNextDialogueInQueue();
        }
        else // 队列为空，这是序列的最后一句
        {
            EndSequenceInternal();
        }
    }

    // IEnumerator WaitAndShowNext(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     ShowNextDialogueInQueue();
    // }

    /// <summary>
    /// 当设置为非自动播放时，由外部调用（如NPC的Interact方法）以继续到下一行。
    /// </summary>
    public void ProceedToNextLine()
    {
        if (CurrentState == DialogueSystemState.WaitingForContinuation)
        {
            PlayerMove.CanPlayerMove = false; // 继续对话，暂时禁止移动
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

        // 1. 先将需要调用的回调保存到临时变量中。
        //    这是一个好习惯，以防回调函数内部又尝试操作DialogueManager。
        var tempCallback = onSequenceCompleteCallback;

        // 2. 立即、彻底地清理所有内部状态，将管理器重置为完全空闲。
        //    这是最关键的修改：将状态清理放在事件触发之前！
        CurrentState = DialogueSystemState.Idle;
        onSequenceCompleteCallback = null; 
        currentDialogueData = null;
        currentDialogueAnchor = null;
        // 为保险起见，也清空队列，防止任何残留。
        currentDialogueQueue.Clear(); 

        // 3. 最后，在所有内部状态都已安全重置后，再调用回调和触发全局事件。
        //    此时，任何监听者（比如你的 async/await）收到的信号都表示“可以安全开始新的对话了”。
        tempCallback?.Invoke();
        DialogueFinished?.Invoke(); 
    }
    
}