using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Collections.Generic;

/// <summary>
/// 打字机通讯接口。
/// 将此脚本挂载到每一个拥有 Typewriter Effect 组件的游戏对象上。
/// 它会自动获取该组件，并向一个全局静态列表注册自己，以便其他脚本可以轻松访问。
/// </summary>
public class TypewriterCommunicator : MonoBehaviour
{
    // 全局静态列表，存储场景中所有活动的通讯器实例
    public static readonly List<TypewriterCommunicator> AllCommunicators = new List<TypewriterCommunicator>();

    // 该通讯器关联的打字机组件（只读属性）
    public AbstractTypewriterEffect Typewriter { get; private set; }

    private void Awake()
    {
        // 在唤醒时，获取同一个游戏对象上的打字机组件
        Typewriter = GetComponent<AbstractTypewriterEffect>();

        if (Typewriter == null)
        {
            Debug.LogError($"错误：在对象 '{name}' 上的 TypewriterCommunicator 脚本未能找到 AbstractTypewriterEffect 组件！", this);
        }
    }

    private void OnEnable()
    {
        // 当对象启用时，将自己添加到全局列表
        if (!AllCommunicators.Contains(this))
        {
            AllCommunicators.Add(this);
        }
    }

    private void OnDisable()
    {
        // 当对象禁用或销毁时，从全局列表中移除自己，以防内存泄漏
        if (AllCommunicators.Contains(this))
        {
            AllCommunicators.Remove(this);
        }
    }
}