using UnityEngine;
using PixelCrushers.DialogueSystem;
using Cysharp.Threading.Tasks;
using PixelCrushers.DialogueSystem.SequencerCommands; // 引入 UniTask


public class SequencerCommandAutoSleep : SequencerCommand
{
    // 这个 Start 方法会立即返回，但它启动的异步任务会继续运行
    public void Start()
    {
        // 启动一个异步的包装方法，并“忘记”它，让它在后台运行。
        // 这个异步方法将负责在任务完成后调用 Stop()。
        DoAutoMoveAsync().Forget(); 
    }

    private async UniTask DoAutoMoveAsync()
    {
        // --- 1. 解析参数 ---
        string targetName = GetParameter(0);
        float proximity = GetParameterAsFloat(1, 0.1f); // 如果不提供，则使用默认值0.1f

        // --- 2. 验证参数和环境 ---
        if (string.IsNullOrEmpty(targetName))
        {
            Debug.LogWarning("Dialogue System: Command 'AutoMove' requires a target object name.", this);
            Stop(); // 参数无效，停止命令
            return;
        }

        var targetObject = GameObject.Find(targetName);
        if (targetObject == null)
        {
            Debug.LogWarning($"Dialogue System: Command 'AutoMove' can't find GameObject named '{targetName}'.", this);
            Stop(); // 找不到目标，停止命令
            return;
        }

        if (PlayerMove.CurrentPlayer == null)
        {
            Debug.LogError("Dialogue System: Command 'AutoMove' can't find PlayerMove.CurrentPlayer.", this);
            Stop(); // 找不到玩家，停止命令
            return;
        }
        
        var playerMove = PlayerMove.CurrentPlayer.GetComponent<PlayerMove>();
        if (playerMove == null)
        {
            Debug.LogError($"Dialogue System: Command 'AutoMove' can't find PlayerMove component on '{PlayerMove.CurrentPlayer.name}'.", this);
            Stop(); // 玩家身上没有脚本，停止命令
            return;
        }

        // --- 3. 执行核心逻辑 ---
        if (DialogueDebug.LogInfo)
        {
            Debug.Log($"Dialogue System: Command 'AutoMove' started. Moving player to '{targetName}'. Dialogue will wait.", this);
        }

        // a. 调用 PlayerMove 脚本中的异步方法，并等待它彻底完成
        await playerMove.AutoMoveToSleep(targetObject.transform.position, proximity);

        // b. 异步方法完成后，停止此序列命令，让对话继续
        if (DialogueDebug.LogInfo)
        {
            Debug.Log("Dialogue System: Command 'AutoMove' has finished. Continuing dialogue.", this);
        }
        Stop();
    }
}
