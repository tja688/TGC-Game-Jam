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
        SleepCycleManager.Instance.TriggerSleepSequence();
        
        Stop();
    }
}