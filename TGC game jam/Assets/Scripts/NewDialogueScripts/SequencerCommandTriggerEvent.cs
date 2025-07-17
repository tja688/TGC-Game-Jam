using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.SequencerCommands;

/// <summary>
/// Dialogue System 序列器命令：
/// 用于触发您自定义的 EventCenter 中的事件。
/// 这是一个非阻塞式命令，触发后会立即继续对话。
/// 语法: TriggerEvent(eventName, [param1], [param2], [param3], [param4], [param5])
/// </summary>
public class SequencerCommandTriggerEvent : SequencerCommand
{
    public void Start()
    {
        // --- 1. 获取事件名称 (必须的第一个参数) ---
        var eventName = GetParameter(0);

        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("Dialogue System: Command 'TriggerEvent' requires an event name.", this);
            Stop();
            return;
        }
        
        Debug.Log("Dialogue System TriggerEvent: "+eventName, this);

        EventCenter.TriggerEvent(eventName);

        // --- 3. 立即停止命令，让对话继续 ---
        Stop();
    }
}