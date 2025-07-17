using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.SequencerCommands; // 必须引用 Dialogue System 的命名空间

/// <summary>
/// 为 Dialogue System 的序列器添加一个自定义指令 "RestartLevel"。
/// 当序列中出现 RestartLevel() 时，会调用这个脚本。
/// </summary>
public class SequencerCommandRestartLevel : SequencerCommand
{
    public void Start()
    {
        // 检查 LevelManager 是否存在
        if (LevelManager.Instance != null)
        {
            // 调用 LevelManager 中的重启方法
            LevelManager.Instance.RestartCurrentLevel();
        }
        else
        {
            // 如果找不到 LevelManager，在控制台给出清晰的错误提示
            UnityEngine.Debug.LogError("Dialogue System Sequencer: RestartLevel() 指令无法执行，因为场景中找不到 LevelManager 的实例。");
        }

        // 指令执行完毕，必须调用 Stop() 来告诉序列器可以继续执行后续指令（如果有的话）
        Stop();
    }
}