using System;
using UnityEngine;
using System.Collections;
using Febucci.UI.Core;

/// <summary>
/// 单例消息提示管理器。
/// 外部通过调用 ShowMessage(string message) 来显示提示。
/// </summary>
public class MessageTipManager : MonoBehaviour
{
    private static MessageTipManager _instance;
    public static SoundEffect MessageTips;
    public SoundEffect messageSounds;
    
    public bool debugText = true;
    
    [Tooltip("请将你的 Text Animator 组件拖拽到这里。例如 Febucci 的 TextAnimatorPlayer。")]
    public TypewriterCore textAnimatorPlayer;
    
    // 消息队列
    private System.Collections.Generic.Queue<string> messageQueue = new System.Collections.Generic.Queue<string>();
    private bool isShowingMessage = false;
    private Coroutine currentMessageCoroutine;
    
    // 消息完成回调事件
    public event Action OnMessageComplete;

    /// <summary>
    /// 获取 MessageTipManager 的单例实例。
    /// </summary>
    public static MessageTipManager Instance
    {
        get
        {
            if (_instance) return _instance;
            _instance = FindObjectOfType<MessageTipManager>();
            if (_instance) return _instance;
            var singletonObject = new GameObject("MessageTipManager");
            _instance = singletonObject.AddComponent<MessageTipManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance && _instance != this)
        {
            Debug.LogWarning("场景中已存在 MessageTipManager 实例，销毁当前重复的实例。");
            Destroy(gameObject);
            return;
        }
        _instance = this;
        
        if (!textAnimatorPlayer)
        {
            Debug.LogError("MessageTipManager: 未在 Inspector 中指定 Text Animator 组件，并且未能自动获取。请手动赋值。");
        }

        MessageTips = messageSounds;
        
        // 注册消息完成回调
        if(textAnimatorPlayer != null)
        {
            textAnimatorPlayer.onTextDisappeared.AddListener(HandleMessageDisappeared);
        }
    }

    private void OnDestroy()
    {
        if (textAnimatorPlayer != null)
        {
            textAnimatorPlayer.onTextDisappeared.RemoveListener(HandleMessageDisappeared);
        }
    }

    private void Update()
    {
        if(!debugText) return;
        
        if (Input.GetKeyDown(KeyCode.F9))
        {
            ShowMessage($"message test!");
        }
    }

    /// <summary>
    /// 显示消息（如果当前有消息正在显示，则加入队列）
    /// </summary>
    public static void ShowMessage(string message)
    {
        Instance.EnqueueMessage(message);
    }

    /// <summary>
    /// 立即显示消息（会打断当前显示的消息）
    /// </summary>
    public static void ShowMessageImmediate(string message)
    {
        Instance.ShowMessageImmediately(message);
    }

    /// <summary>
    /// 清空消息队列
    /// </summary>
    public static void ClearMessageQueue()
    {
        Instance.messageQueue.Clear();
    }

    /// <summary>
    /// 检查当前是否有消息正在显示
    /// </summary>
    public static bool IsShowingMessage()
    {
        return Instance.isShowingMessage;
    }

    /// <summary>
    /// 手动触发消息消失
    /// </summary>
    public static void HideCurrentMessage()
    {
        if (Instance.textAnimatorPlayer != null && Instance.isShowingMessage)
        {
            Instance.textAnimatorPlayer.StartDisappearingText();
        }
    }

    private void EnqueueMessage(string message)
    {
        messageQueue.Enqueue(message);
        
        if (!isShowingMessage && messageQueue.Count > 0)
        {
            ProcessNextMessage();
        }
    }

    private void ShowMessageImmediately(string message)
    {
        // 停止当前显示的消息
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }
        
        // 清空队列
        messageQueue.Clear();
        
        // 直接显示新消息
        StartCoroutine(ShowMessageCoroutine(message));
    }

    private void ProcessNextMessage()
    {
        if (messageQueue.Count > 0 && !isShowingMessage)
        {
            string nextMessage = messageQueue.Dequeue();
            currentMessageCoroutine = StartCoroutine(ShowMessageCoroutine(nextMessage));
        }
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        isShowingMessage = true;
        
        if (textAnimatorPlayer != null)
        {
            textAnimatorPlayer.ShowText(message);
            AudioManager.Instance.Play(MessageTips);
        }
        
        yield return null;
    }

    private void HandleMessageDisappeared()
    {
        isShowingMessage = false;
        OnMessageComplete?.Invoke();
        ProcessNextMessage();
    }
}