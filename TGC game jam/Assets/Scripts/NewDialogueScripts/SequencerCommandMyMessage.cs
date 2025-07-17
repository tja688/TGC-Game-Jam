using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.SequencerCommands; // 引入 Dialogue System 的命名空间

/// <summary>
/// 这是一个自定义的 Sequencer Command，用于桥接 Dialogue System 和 MessageTipManager。
/// 它允许你在对话的 Sequence 中调用 MessageTipManager 来显示消息。
/// 
/// 使用方法:
/// 在 Dialogue System 的 Sequence 字段中输入:
/// Message("你想显示的消息内容"[, immediate, waitForCompletion])
/// 
/// 参数说明:
/// - 参数1 (字符串): 必须。要显示的具体消息。
/// - 参数2 (布尔值, 可选): 可选。填 'true' 则立即显示消息（会打断当前消息），否则加入队列。默认为 'false'。
/// - 参数3 (布尔值, 可选): 可选。填 'true' 则对话会暂停，直到这个消息显示完成。默认为 'false'。
/// 
/// 示例:
/// 1. 显示一条普通消息: 
///    Message("你好，世界！")
/// 2. 立即显示一条紧急消息:
///    Message("警告！前方危险！", true)
/// 3. 显示一条消息，并等待其播放完毕:
///    Message("请仔细阅读这段说明。", false, true)
/// </summary>
public class SequencerCommandMyMessage : SequencerCommand
{
    private bool shouldWaitForCompletion = false;

    public void Start()
    {
        // --- 参数解析 ---
        // 参数1: 消息内容 (string)
        string message = GetParameter(0);

        // 参数2: 是否立即显示 (bool)
        bool immediate = GetParameterAsBool(1, false); // 默认为 false

        // 参数3: 是否等待完成 (bool)
        shouldWaitForCompletion = GetParameterAsBool(2, false); // 默认为 false

        // 如果消息内容为空，则直接停止该命令
        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("Dialogue System: Sequencer command 'Message' has an empty message.");
            Stop();
            return;
        }

        // --- 调用 MessageTipManager ---
        if (immediate)
        {
            // 立即显示模式不支持等待，因为它会打断一切
            if (shouldWaitForCompletion)
            {
                Debug.LogWarning("Dialogue System: 'Message' command with 'immediate=true' cannot also use 'waitForCompletion=true'. The sequence will not wait.");
            }
            MessageTipManager.ShowMessageImmediate(message);
            Stop(); // 立即模式下，命令本身也是立即完成
        }
        else
        {
            // 队列模式
            if (shouldWaitForCompletion)
            {
                // 如果需要等待，我们先注册完成事件，再显示消息
                // 当消息播放完毕后，OnMessageCompleted 方法会被调用，届时再 Stop()
                MessageTipManager.Instance.OnMessageComplete += OnMessageCompleted;
                MessageTipManager.ShowMessage(message);
            }
            else
            {
                // 如果不需要等待，直接调用并停止命令
                MessageTipManager.ShowMessage(message);
                Stop();
            }
        }
    }

    // 当消息播放完毕时，MessageTipManager 会触发 OnMessageComplete 事件，从而调用此方法
    private void OnMessageCompleted()
    {
        // 一旦收到完成回调，就立刻注销事件，防止重复调用
        if (MessageTipManager.Instance != null)
        {
            MessageTipManager.Instance.OnMessageComplete -= OnMessageCompleted;
        }

        // 停止该 Sequencer Command，让对话继续
        Stop();
    }

    // 当 Sequencer Command 被停止或所在的 GameObject 被销毁时调用
    public void OnDestroy()
    {
        // 这是一个安全措施，确保在任何情况下（如玩家跳过对话）都注销事件，防止内存泄漏
        if (shouldWaitForCompletion && MessageTipManager.Instance != null)
        {
            MessageTipManager.Instance.OnMessageComplete -= OnMessageCompleted;
        }
    }
}