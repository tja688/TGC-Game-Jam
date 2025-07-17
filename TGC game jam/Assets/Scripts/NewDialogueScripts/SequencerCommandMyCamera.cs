using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.SequencerCommands; // 引入 Dialogue System 的命名空间

/// <summary>
/// Dialogue System for Unity 的一个自定义序列器命令，
/// 用于控制您的 CameraSystem。
/// 语法: MyCamera(targetName, [speed])
/// - targetName: 场景中目标GameObject的名称。特殊值 "player" 或 "null" 用于恢复跟随玩家。
/// - speed (可选): 临时的相机移动速度。
/// </summary>
public class SequencerCommandMyCamera : SequencerCommand
{
    private float originalSpeed;
    private bool speedWasChanged = false;

    public void Start()
    {
        // --- 1. 参数解析 ---
        // 参数0: 目标物体的名称 (string)。
        string targetName = GetParameter(0);
        // 参数1 (可选): 临时的相机移动速度 (float)。
        float newSpeed = GetParameterAsFloat(1, -1f); 

        if (DialogueDebug.LogInfo) 
        {
            Debug.Log($"Dialogue System: Running Sequencer Command 'MyCamera({targetName}, {newSpeed})'");
        }

        // --- 2. 查找目标 Transform ---
        Transform targetTransform = null;
        if (string.IsNullOrEmpty(targetName) || targetName.Equals("player", System.StringComparison.OrdinalIgnoreCase) || targetName.Equals("null", System.StringComparison.OrdinalIgnoreCase))
        {
            targetTransform = null; // 传入 null 会让 CameraSystem 恢复跟随玩家
        }
        else
        {
            GameObject targetObject = GameObject.Find(targetName);
            if (targetObject != null)
            {
                targetTransform = targetObject.transform;
            }
            else
            {
                Debug.LogWarning($"Dialogue System: Command 'MyCamera' can't find GameObject named '{targetName}'.", this);
                Stop(); // 找不到目标，直接停止命令，防止对话卡住
                return;
            }
        }
        
        // --- 3. 检查 CameraSystem 实例是否存在 ---
        if (CameraSystem.Instance == null)
        {
             Debug.LogError("Dialogue System: Command 'MyCamera' requires a CameraSystem instance in the scene.", this);
             Stop(); // 没有 CameraSystem 实例，无法执行
             return;
        }

        // --- 4. 处理相机速度 (如果提供了新速度) ---
        if (newSpeed > 0)
        {
            originalSpeed = CameraSystem.Instance.MoveSpeed;
            CameraSystem.Instance.MoveSpeed = newSpeed;
            speedWasChanged = true;
        }

        // --- 5. 设置相机目标并监听抵达事件 ---
        // 如果目标是恢复跟随玩家，我们实际上是清空特殊目标。
        // 这种情况下，相机应该立即开始跟随，无需等待“抵达”，所以直接停止命令，让对话继续。
        if (targetTransform == null)
        {
            CameraSystem.SetSpecialCameraTarget(null);
            Stop(); 
        }
        else
        {
            // 如果是移动到特殊目标，则订阅“抵达”事件。
            // 当相机抵达后，OnCameraArrived 方法会被调用，然后停止此命令。
            CameraSystem.OnCameraArrivedAtSpecialTarget += OnCameraArrived;
            CameraSystem.SetSpecialCameraTarget(targetTransform);
        }
    }

    // 当 CameraSystem 触发抵达事件时，此方法被调用
    private void OnCameraArrived()
    {
        if (DialogueDebug.LogInfo)
        {
            Debug.Log("Dialogue System: 'MyCamera' command finished because camera arrived at target.");
        }
        Stop(); // 停止该序列命令，让对话流程继续
    }

    // 当此命令停止时（无论是正常结束还是被中断），此方法被调用
    public void OnDestroy()
    {
        // 为防止内存泄漏和逻辑错误，必须在命令结束时取消订阅事件
        if (CameraSystem.Instance != null)
        {
             CameraSystem.OnCameraArrivedAtSpecialTarget -= OnCameraArrived;
             
             // 如果之前修改了速度，在这里恢复原始速度
             if (speedWasChanged)
             {
                 CameraSystem.Instance.MoveSpeed = originalSpeed;
                 speedWasChanged = false;
             }
        }
    }
}