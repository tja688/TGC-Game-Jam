using System;
using UnityEngine;
// ------------------------------------------------------------------------------------------
// 注意：如果你使用的是 Febucci Text Animator 插件，下面的 using 可能是正确的。
// 如果你使用其他 Text Animator 插件或者自定义的 Text Animator，
// 你可能需要修改或移除这个 using 语句，并调整下面的 textAnimatorPlayer 变量的类型。
// ------------------------------------------------------------------------------------------
using Febucci.UI;
using Febucci.UI.Core;
using UnityEngine.Serialization; // 仅当使用 Febucci Text Animator 时需要，否则请移除或替换

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
    
    /// <summary>
    /// 获取 MessageTipManager 的单例实例。
    /// </summary>
    public static MessageTipManager Instance
    {
        get
        {
            if (_instance) return _instance;
            // 尝试在场景中查找已存在的实例
            _instance = FindObjectOfType<MessageTipManager>();

            if (_instance) return _instance;
            // 如果场景中不存在，则创建一个新的 GameObject 并添加此脚本
            var singletonObject = new GameObject("MessageTipManager");
            _instance = singletonObject.AddComponent<MessageTipManager>();
            return _instance;
        }
    }
    
    [Tooltip("请将你的 Text Animator 组件拖拽到这里。例如 Febucci 的 TextAnimatorPlayer。")]
    public TypewriterCore textAnimatorPlayer; // <--- 修改这里的类型为你实际使用的组件类型

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
    }

    private void Update()
    {
        if(!debugText) return;
        
        if (Input.GetKeyDown(KeyCode.F9))
        {
            MessageTipManager.ShowMessage($"message test!");
        }
    }

    /// <summary>
    /// 公开的静态方法，用于显示消息。
    /// </summary>
    /// <param name="message">要显示的消息内容。</param>
    public static void ShowMessage(string message)
    {
        if (!Instance)
        {
            Debug.LogError("MessageTipManager 实例尚未初始化。无法显示消息。");
            return;
        }

        if (!Instance.textAnimatorPlayer)
        {
            Debug.LogError("MessageTipManager: Text Animator 组件未被赋值。无法显示消息。请在 MessageTipManager GameObject 的 Inspector 中赋值。");
            return;
        }
        
        Instance.textAnimatorPlayer.ShowText(message); 
        AudioManager.Instance.Play(MessageTips);

    }
}