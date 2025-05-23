// TJAnimationDialogueManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // 用于 Find
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;
using UnityEngine.Events;

[DisallowMultipleComponent] // 通常一个NPC上只需要一个此管理器
public class TJAnimationDialogueManager : MonoBehaviour
{
    [Header("必要组件引用")]
    [SerializeField] private TypewriterCore typewriter; // 拖拽场景中的 Text Animator TypewriterCore 组件到这里
    [SerializeField] private GameObject continuePromptUI; // 可选：用于提示玩家继续对话的UI元素（例如一个"按E继续"的图标）

    [Header("对话配置")]
    [Tooltip("该NPC持有的所有对话序列")]
    [SerializeField] private List<DialogueSequence> dialogueSequences = new List<DialogueSequence>();

    [Header("事件映射配置")]
    [Tooltip("将文本内事件标签映射到具体方法调用")]
    [SerializeField] private List<TJEventMapping> eventMappings = new List<TJEventMapping>();

    // 内部状态变量
    private bool _isDisplayingText = false;
    private bool _isTextPausedForEvent = false; // 文本是否因为触发了需要暂停的事件而暂停
    private DialogueSequence _currentDialogueSequence;
    private int _currentPageIndex;
    private string _initiatingDialogueTag; // 记录当前是由哪个tag启动的对话

    // --- Unity 生命周期方法 ---
    void Awake()
    {
        if (typewriter == null)
        {
            Debug.LogError($"TJAnimationDialogueManager on {gameObject.name}: TypewriterCore 未分配!", this);
            enabled = false; // 禁用脚本如果核心组件缺失
            return;
        }

        if (continuePromptUI != null)
        {
            continuePromptUI.SetActive(false);
        }

        // 订阅 Text Animator 的事件
        typewriter.onMessage.AddListener(HandleTypewriterInlineEvent); // 处理 <event=TAG>
        typewriter.onTextShowed.AddListener(OnTypewriterPageShowed);   // 处理当前页面文本完全显示后的逻辑
    }

    void OnDestroy()
    {
        // 取消订阅，防止内存泄漏
        if (typewriter != null)
        {
            typewriter.onMessage.RemoveListener(HandleTypewriterInlineEvent);
            typewriter.onTextShowed.RemoveListener(OnTypewriterPageShowed);
        }
    }

    void Update()
    {
        // 处理玩家继续对话的输入 (示例：按E键)
        // 这个输入逻辑可以根据你的游戏输入管理器进行调整
        if (_isDisplayingText && !_isTextPausedForEvent && continuePromptUI != null && continuePromptUI.activeSelf)
        {
            // 在这里检查你的游戏设定的“互动/继续”按键
            if (Input.GetKeyDown(KeyCode.E)) // 示例按键
            {
                AdvanceToNextPage();
            }
        }
    }

    // --- 公共 API (供外部 DialogueLogicManager 调用) ---

    /// <summary>
    /// 检查当前NPC是否可以开始一段新的对话。
    /// </summary>
    public bool CanStartDialogue()
    {
        return !_isDisplayingText;
    }

    /// <summary>
    /// 根据提供的对话标签开始显示对话序列。
    /// </summary>
    /// <param name="dialogueTag">要显示的对话序列的唯一标签。</param>
    /// <returns>如果成功开始对话则返回true，否则返回false (例如NPC正忙或标签无效)。</returns>
    public bool RequestShowDialogue(string dialogueTag)
    {
        if (!CanStartDialogue())
        {
            Debug.LogWarning($"TJAnimationDialogueManager on {gameObject.name}: 尝试在播放对话时开始新的对话 '{dialogueTag}'。请求被忽略。", this);
            return false;
        }

        DialogueSequence sequenceToPlay = FindDialogueSequenceByTag(dialogueTag);
        if (sequenceToPlay == null || sequenceToPlay.pages.Count == 0)
        {
            Debug.LogError($"TJAnimationDialogueManager on {gameObject.name}: 未找到标签为 '{dialogueTag}' 的对话序列，或者该序列没有页面。", this);
            return false;
        }

        _isDisplayingText = true;
        _isTextPausedForEvent = false; // 重置暂停状态
        _currentDialogueSequence = sequenceToPlay;
        _currentPageIndex = 0;
        _initiatingDialogueTag = dialogueTag; // 记录

        Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 开始播放对话 '{dialogueTag}'.");
        DisplayCurrentPage();
        return true;
    }

    /// <summary>
    /// 如果文本因事件而暂停，则调用此方法来继续播放。
    /// </summary>
    public void ResumePausedDialogue()
    {
        if (!_isDisplayingText || !_isTextPausedForEvent)
        {
            // Debug.LogWarning($"TJAnimationDialogueManager on {gameObject.name}: ResumePausedDialogue 被调用，但文本并未因事件暂停。", this);
            return;
        }

        _isTextPausedForEvent = false;
        Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 从事件暂停中恢复对话 '{_initiatingDialogueTag}'，页面 {_currentPageIndex + 1}。");

        // 检查当前页面是否已经完全打出来了 (因为Typewriter可能在设置暂停标志后仍然完成了当前页的打印)
        // 如果TypewriterCore有IsDone()之类的状态，可以用它判断。
        // 简单起见，我们假设当Resume被调用时，应该表现得像刚打完字一样，准备显示“继续”提示或进入下一页。
        OnTypewriterPageShowedLogic(); // 手动触发页面显示完成的逻辑
    }


    // --- 内部对话流程控制方法 ---

    private void DisplayCurrentPage()
    {
        if (_currentDialogueSequence == null || _currentPageIndex >= _currentDialogueSequence.pages.Count)
        {
            EndDialogueSequence();
            return;
        }

        if (continuePromptUI != null)
        {
            continuePromptUI.SetActive(false); // 在新页面开始显示时隐藏“继续”提示
        }

        string textToShow = _currentDialogueSequence.pages[_currentPageIndex].textContent;
        typewriter.ShowText(textToShow); // Text Animator 开始播放当前页的文本
    }

    private void AdvanceToNextPage()
    {
        _currentPageIndex++;
        DisplayCurrentPage(); // 显示下一页，如果已经是最后一页，DisplayCurrentPage内部会调用EndDialogueSequence
    }

    private void EndDialogueSequence()
    {
        Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 对话 '{_initiatingDialogueTag}' 结束。");
        if (continuePromptUI != null)
        {
            continuePromptUI.SetActive(false);
        }
        // typewriter.StartDisappearingText(); // 如果需要文本消失效果，可以在这里调用
        // 或者直接清除文本: typewriter.ShowText(""); // 但这会触发onTextShowed，可能需要额外处理

        _isDisplayingText = false;
        _isTextPausedForEvent = false;
        _currentDialogueSequence = null;
        _currentPageIndex = 0;
        _initiatingDialogueTag = null;

        // 可选：在这里通知外部 DialogueLogicManager 对话已结束
        // DialogueEvents.OnDialogueEnded?.Invoke(gameObject, _initiatingDialogueTag);
    }

    // --- Text Animator 事件回调处理 ---

    // 当 TypewriterCore 完成显示当前页面所有文本后调用
    private void OnTypewriterPageShowed()
    {
        if (!_isDisplayingText) return; // 如果对话中途被某种方式结束了，则不处理

        OnTypewriterPageShowedLogic();
    }

    private void OnTypewriterPageShowedLogic() // 提取公共逻辑，方便Resume调用
    {
        if (_isTextPausedForEvent)
        {
            // 文本已显示完毕，但我们正因事件而“暂停”，等待外部Resume。
            // 不显示“继续”提示。
            Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 页面 {_currentPageIndex + 1} 显示完毕，但因事件暂停。");
            return;
        }

        // 检查是否是对话序列的最后一页
        bool isLastPage = (_currentDialogueSequence != null && _currentPageIndex >= _currentDialogueSequence.pages.Count - 1);

        if (isLastPage)
        {
            // 如果是最后一页，并且有continuePromptUI，也显示它，让玩家按键来最终结束对话。
            // 或者，你可以设计成最后一页自动结束，不显示提示。取决于你的设计。
            if (continuePromptUI != null)
            {
                continuePromptUI.SetActive(true);
            }
            else
            {
                // 如果没有继续提示，并且是最后一页，那么可以认为对话在此处自然结束
                // 但我们的 AdvanceToNextPage 会在 Update 中由玩家输入触发，所以这里通常还是等待玩家输入
                // 如果需要最后一页自动结束，需要修改 AdvanceToNextPage 和这里的逻辑
            }
            Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 对话 '{_initiatingDialogueTag}' 的最后一页 ({_currentPageIndex + 1}) 显示完毕。");
        }
        else
        {
            // 如果不是最后一页，显示“继续”提示 (如果配置了)
            if (continuePromptUI != null)
            {
                continuePromptUI.SetActive(true);
            }
             Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 对话 '{_initiatingDialogueTag}' 的页面 {_currentPageIndex + 1} 显示完毕。");
        }
    }


    // 当 TypewriterCore 遇到文本中的 <event=TAG_NAME:param1:param2> 标签时调用
    private void HandleTypewriterInlineEvent(EventMarker eventData)
    {
        if (!_isDisplayingText) return;

        Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 收到内联事件: '{eventData.name}' 带参数: [{string.Join(", ", eventData.parameters)}]");

        TJEventMapping mapping = FindEventMappingByTag(eventData.name);
        if (mapping != null)
        {
            // 触发关联的 UnityEvent
            mapping.onTriggered?.Invoke(eventData.parameters); // 传递解析出的参数

            if (mapping.pauseTextOnTrigger)
            {
                _isTextPausedForEvent = true;
                // 注意：Text Animator 的 TypewriterCore 本身可能没有一个显式的 PauseTyping() 方法
                // 我们的“暂停”逻辑是：设置 _isTextPausedForEvent = true;
                // 然后 OnTypewriterPageShowed 回调会检查这个标志，如果不为true，则不显示“继续”提示，
                // 从而达到“暂停”等待外部 ResumePausedDialogue() 调用的效果。
                // Typewriter 会继续完成当前页的打印，但流程会在此“卡住”直到Resume。
                Debug.Log($"TJAnimationDialogueManager on {gameObject.name}: 事件 '{eventData.name}' 触发，文本播放已暂停。等待 ResumePausedDialogue() 调用。");

                // 如果有 "继续" UI，此时也应该隐藏它，因为是事件暂停，不是等待玩家继续页面
                if(continuePromptUI != null) continuePromptUI.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning($"TJAnimationDialogueManager on {gameObject.name}: 在对话 '{_initiatingDialogueTag}' 中收到未配置的事件标签: '{eventData.name}'", this);
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
}