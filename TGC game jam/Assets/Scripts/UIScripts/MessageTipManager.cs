using System;
using UnityEngine;
using System.Collections;
using Febucci.UI.Core;

public class MessageTipManager : MonoBehaviour
{
    private static MessageTipManager _instance;
    public static SoundEffect MessageTips;
    public SoundEffect messageSounds;
    
    public bool debugText = true;
    
    public TypewriterCore textAnimatorPlayer;
    
    private System.Collections.Generic.Queue<string> messageQueue = new System.Collections.Generic.Queue<string>();
    private bool isShowingMessage = false;
    private Coroutine currentMessageCoroutine;
    
    public event Action OnMessageComplete;
    
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
    
    public static void ShowMessage(string message)
    {
        Instance.EnqueueMessage(message);
    }
    
    public static void ShowMessageImmediate(string message)
    {
        Instance.ShowMessageImmediately(message);
    }
    
    public static void ClearMessageQueue()
    {
        Instance.messageQueue.Clear();
    }
    
    public static bool IsShowingMessage()
    {
        return Instance.isShowingMessage;
    }
    
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
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }
        
        messageQueue.Clear();
        
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