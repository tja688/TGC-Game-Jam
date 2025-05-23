// TJAnimationDialogueManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // 用于 Find
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class TJAnimationDialogueManager : MonoBehaviour
{
    [Header("必要组件引用")]
    [SerializeField] private TypewriterCore typewriter;
    [SerializeField] private GameObject continuePromptUI; // 可选，用于提示继续

    [Header("对话配置")]
    [Tooltip("该NPC持有的所有对话序列")]
    [SerializeField] private List<DialogueSequence> dialogueSequences = new List<DialogueSequence>();

    [Header("事件映射配置")]
    [Tooltip("将文本内事件标签映射到具体方法调用")]
    [SerializeField] private List<TJEventMapping> eventMappings = new List<TJEventMapping>();

    // 内部状态变量
    private bool isDisplayingText = false;           // 是否有任何对话序列正在激活（包括打字、暂停、等待继续）
    private bool isTextPausedForEvent = false;       // 当前文本是否因内联事件而暂停
    private bool isWaitingForPlayerToContinuePage = false; // 当前页面是否已完全打出，并等待玩家输入以继续

    private DialogueSequence currentDialogueSequence; // 当前正在播放的对话序列
    private int currentPageIndex;                   // 当前对话序列的页面索引
    private string initiatingDialogueTag;           // 当前激活的对话序列的标签

    // --- “出”事件：对话序列完成 ---
    /// <summary>
    /// 当一个完整的对话序列（所有页面）播放完毕后触发。
    /// 参数1: 对话发起者 (NPC GameObject)
    /// 参数2: 完成的对话序列的标签
    /// </summary>
    public static event System.Action<GameObject, string> OnDialogueSequenceCompleted;

    // --- Unity 生命周期方法 ---
    private void Awake()
    {
        if (!typewriter)
        {
            Debug.LogError($"TJAnimationDialogueManager on {gameObject.name}: TypewriterCore 未分配!", this);
            enabled = false;
            return;
        }

        if (continuePromptUI)
        {
            continuePromptUI.SetActive(false);
        }

        typewriter.onMessage.AddListener(HandleTypewriterInlineEvent);    // 处理 <event=TAG>
        typewriter.onTextShowed.AddListener(OnTypewriterPageFullyDisplayed); // 处理当前页面文本完全显示后的逻辑
    }

    private void OnDestroy()
    {
        if (!typewriter) return;
        typewriter.onMessage.RemoveListener(HandleTypewriterInlineEvent);
        typewriter.onTextShowed.RemoveListener(OnTypewriterPageFullyDisplayed);
    }

    // --- 公共 “进” API ---

    /// <summary>
    /// 外部系统调用此方法来请求与NPC进行特定主题的对话。
    /// 内部会自动处理是开启新对话还是继续当前对话。
    /// </summary>
    /// <param name="dialogueTag">要发起或继续的对话的主题标签。</param>
    /// <returns>如果请求被处理（开始新对话或成功翻页）则返回true，否则返回false (例如NPC忙于其他不同对话，或标签无效)。</returns>
    public bool RequestDialogueInteraction(string dialogueTag)
    {
        if (string.IsNullOrEmpty(dialogueTag))
        {
            Debug.LogWarning($"TJAnimationDialogueManager on {gameObject.name}: RequestDialogueInteraction 收到空的 dialogueTag。", this);
            return false;
        }

        // 情况 1: 当前没有任何对话在进行 (_isDisplayingText 为 false)
        if (!isDisplayingText)
        {
            return StartNewDialogueSequence(dialogueTag);
        }
        // 情况 2: 当前有对话正在进行 (_isDisplayingText 为 true)
        else
        {
            // 检查是否是针对当前正在进行的同一对话主题的“继续”请求
            if (initiatingDialogueTag == dialogueTag)
            {
                if (isTextPausedForEvent) // 如果因事件暂停
                {
                    // Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 对话 '{_initiatingDialogueTag}' 因事件暂停中。请先调用 ResumePausedDialogue() 或等待事件自动完成。");
                    return false; // 不能通过此API恢复事件暂停，或在事件暂停时翻页
                }

                if (isWaitingForPlayerToContinuePage) // 如果当前页面已显示完毕，等待玩家继续
                {
                    // Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 继续对话 '{_initiatingDialogueTag}'，前往下一页。");
                    AdvanceToNextPage();
                    return true; // 成功继续/翻页
                }
                else // 当前页面正在打字，或处于其他非“等待继续”的状态
                {
                    // Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 对话 '{_initiatingDialogueTag}' 页面正在打字或处理中，请稍候。");
                    // 未来可考虑实现“快进当前页”的逻辑，但目前按“不打断”原则，忽略此“继续”请求，让其自然完成打字
                    return false; // 表示请求已收到，但当前无法执行翻页 (可能正在打字)
                }
            }
            else // 请求的是一个不同的对话主题，但当前NPC正忙于另一个主题
            {
                Debug.LogWarning($"TJAnimationDialogueManager on {gameObject.name}: NPC 正忙于对话 '{initiatingDialogueTag}'。无法开始新的对话主题 '{dialogueTag}'。", this);
                return false; // 忽略不同主题的请求，保证当前对话连贯性
            }
        }
    }

    /// <summary>
    /// 如果文本因<event>标签中配置的事件而暂停，则调用此方法来继续播放。
    /// </summary>
    public void ResumePausedDialogue()
    {
        if (!isDisplayingText || !isTextPausedForEvent)
        {
            // Debug.LogWarning($"TJAnimationDialogueManager on {gameObject.name}: ResumePausedDialogue 被调用，但文本并未因事件暂停。", this);
            return;
        }

        isTextPausedForEvent = false;
        // Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 从事件暂停中恢复对话 '{_initiatingDialogueTag}'，页面 {_currentPageIndex + 1}。");

        // 事件暂停解除后，我们认为当前页面逻辑上已“显示完毕”，进入等待玩家继续的状态
        // (因为Typewriter可能在事件触发、我们设置暂停标志后，仍然完成了当前页面的剩余打印)
        ProcessPageFullyDisplayedLogic();
    }

    // --- 内部对话流程控制方法 ---

    private bool StartNewDialogueSequence(string dialogueTag)
    {
        DialogueSequence sequenceToPlay = FindDialogueSequenceByTag(dialogueTag);
        if (sequenceToPlay == null || sequenceToPlay.pages.Count == 0)
        {
            Debug.LogError($"TJAnimationDialogueManager on {gameObject.name}: 未找到标签为 '{dialogueTag}' 的对话序列，或者该序列没有页面。", this);
            return false;
        }

        isDisplayingText = true;
        isTextPausedForEvent = false;
        isWaitingForPlayerToContinuePage = false; // 新对话的第一页开始时不等待
        currentDialogueSequence = sequenceToPlay;
        currentPageIndex = 0;
        initiatingDialogueTag = dialogueTag;

        // Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 开始新对话 '{_initiatingDialogueTag}'.");
        DisplayCurrentPage();
        return true;
    }

    private void DisplayCurrentPage()
    {
        // 检查是否所有页面都已显示完毕
        if (currentDialogueSequence == null || currentPageIndex >= currentDialogueSequence.pages.Count)
        {
            EndDialogueSequence();
            return;
        }

        isWaitingForPlayerToContinuePage = false; // 新页面开始显示时，重置“等待继续”状态
        if (continuePromptUI)
        {
            continuePromptUI.SetActive(false); // 隐藏“继续”提示，直到页面显示完毕
        }

        string textToShow = currentDialogueSequence.pages[currentPageIndex].textContent;
        typewriter.ShowText(textToShow); // Text Animator 开始播放当前页的文本
    }

    private void AdvanceToNextPage()
    {
        // 由 RequestDialogueInteraction (当判断为继续当前对话时) 调用
        currentPageIndex++;
        // DisplayCurrentPage 内部会检查 _currentPageIndex 是否越界，并相应地调用 EndDialogueSequence
        DisplayCurrentPage();
    }

    private void EndDialogueSequence()
    {
        string completedTag = initiatingDialogueTag;
        GameObject npcObject = this.gameObject;

        if (continuePromptUI)
        {
            continuePromptUI.SetActive(false);
        }

        // 重置所有状态
        isDisplayingText = false;
        isTextPausedForEvent = false;
        isWaitingForPlayerToContinuePage = false;
        currentDialogueSequence = null;
        currentPageIndex = 0;
        initiatingDialogueTag = null;

        Debug.Log($"TJAnimationDialogueManager on {npcObject.name}: 对话 '{completedTag}' 结束。");
        OnDialogueSequenceCompleted?.Invoke(npcObject, completedTag); // 触发“出”事件
    }

    // --- Text Animator 事件回调处理 ---

    // 当 TypewriterCore 完成显示当前页面所有文本后调用
    private void OnTypewriterPageFullyDisplayed()
    {
        // Debug.Log("Finished typewriter page");
        
        if (!isDisplayingText) return; // 如果对话在打字过程中被意外结束，则不处理

        ProcessPageFullyDisplayedLogic();
    }

    // 当页面显示完毕 或 从事件暂停中恢复时，调用此逻辑
    private void ProcessPageFullyDisplayedLogic()
    {
        if (isTextPausedForEvent)
        {
            // 如果当前是因事件而暂停的状态，则不进入“等待玩家继续”
            // Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 页面技术上显示完毕，但当前因事件 '{_initiatingDialogueTag}' 暂停。");
            return;
        }

        // Debug.Log("_isWaitingForPlayerToContinuePage");
        
        // 当前页面已完整显示，并且没有因事件暂停 -> 进入“等待玩家继续”状态
        isWaitingForPlayerToContinuePage = true;

        if (continuePromptUI)
        {
            continuePromptUI.SetActive(true); // 显示“继续”提示
        }
        // Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 对话 '{_initiatingDialogueTag}' 的页面 {_currentPageIndex + 1} 显示完毕，等待玩家通过 RequestDialogueInteraction 继续。");
    }

    // 当 TypewriterCore 遇到文本中的 <event=TAG_NAME:param1:param2> 标签时调用
    private void HandleTypewriterInlineEvent(EventMarker eventData)
    {
        if (!isDisplayingText) return;

        // Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 收到内联事件: '{eventData.name}' 带参数: [{string.Join(", ", eventData.parameters)}]");

        TJEventMapping mapping = FindEventMappingByTag(eventData.name);
        if (mapping != null)
        {
            mapping.onTriggered?.Invoke(eventData.parameters); // 传递解析出的参数

            if (mapping.pauseTextOnTrigger)
            {
                isTextPausedForEvent = true;
                isWaitingForPlayerToContinuePage = false; // 事件暂停时，不响应“继续”请求

                if (continuePromptUI) continuePromptUI.SetActive(false); // 隐藏可能存在的“继续”提示
                // Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 事件 '{eventData.name}' 触发，文本播放已暂停。等待 ResumePausedDialogue() 调用。");
            }
        }
        else
        {
            Debug.LogWarning($"TJAnimationDialogueManager on {gameObject.name}: 在对话 '{initiatingDialogueTag}' 中收到未配置的事件标签: '{eventData.name}'", this);
        }
    }

    // --- 辅助方法 ---
    private DialogueSequence FindDialogueSequenceByTag(string tag)
    {
        return dialogueSequences.Find(seq => seq.dialogueTag == tag);
    }

    private TJEventMapping FindEventMappingByTag(string tag)
    {
        return eventMappings.Find(mapping => mapping.eventTagInText == tag);
    }

    public bool IsCompletelyIdle()
    {
        return !isDisplayingText;
    }
}