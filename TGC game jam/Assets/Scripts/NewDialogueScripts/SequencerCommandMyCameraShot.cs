using UnityEngine;
using PixelCrushers.DialogueSystem;
using Cysharp.Threading.Tasks;
using PixelCrushers.DialogueSystem.SequencerCommands; // 引入 UniTask

/// <summary>
/// 一个“发射后不管”的序列器命令。
/// 它会调用 CameraSystem 的 PanAndReturnAsync 方法，然后立即结束，允许对话继续。
/// 语法: MyCameraShot(targetName, waitDuration)
/// </summary>
public class SequencerCommandMyCameraShot : SequencerCommand
{
    public void Start()
    {
        // 1. 参数解析
        string targetName = GetParameter(0);
        float waitDuration = GetParameterAsFloat(1, 1f); // 默认等待1秒

        // 2. 查找目标
        var targetObject = GameObject.Find(targetName);
        if (targetObject == null)
        {
            Debug.LogWarning($"Dialogue System: Command 'MyCameraShot' can't find GameObject named '{targetName}'.", this);
            Stop(); // 找不到目标，直接停止
            return;
        }

        // 3. 检查 CameraSystem 实例
        if (CameraSystem.Instance == null)
        {
            Debug.LogError("Dialogue System: Command 'MyCameraShot' requires a CameraSystem instance.", this);
            Stop();
            return;
        }
        
        // 4. 调用异步方法，并用 .Forget() 将其“发射后不管” (核心)
        CameraSystem.Instance.PanAndReturnAsync(targetObject.transform, waitDuration).Forget();
        
        // 5. 立即停止此序列命令，让 Dialogue System 继续执行下一条指令
        Stop();
    }
}