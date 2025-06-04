using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

using Cysharp.Threading.Tasks;


public class GameFlow : MonoBehaviour
{
    [Header("Audio")]
    public SoundEffect streetMainMusic;
    public SoundEffect beginPanelMusic;


    [Header("UI Elements")]
    [Tooltip("开始菜单的游戏对象，游戏开始时默认激活")]
    public GameObject startMenuPanel;
    [Tooltip("开始游戏按钮")]
    public Button startGameButton;
    [Tooltip("玩家界面按钮")]
    public GameObject playerPanelButton;

    private float originalCameraSpeed; 
    
    private void Start()
    {
        if (ScreenFadeController.Instance)
            ScreenFadeController.Instance.BeginFadeToClear(2f);
        
        // 1. 播放背景音乐
        if (!beginPanelMusic) // 修正：变量名应该是 streetMainMusic
        {
            Debug.LogError("GameFlow: streetMainMusic (背景音乐 SoundEffect) 未在 Inspector 中指定。");
        }
        else
        {
            AudioManager.Instance.Play(beginPanelMusic);
        }

        if (startMenuPanel)
        {
            startMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameFlow: startMenuPanel (开始菜单 GameObject) 未在 Inspector 中指定。");
        }

        if (PlayerInputController.Instance)
        {
            PlayerInputController.Instance.ActivateUIControls();
        }
        else
        {
            Debug.LogError("GameFlow: PlayerInputController.Instance 为空。无法设置初始输入模式。");
        }

        // 4. 为开始按钮添加监听器
        if (startGameButton)
        {
            startGameButton.onClick.AddListener(StartGameSequence);
        }
        else
        {
            Debug.LogError("GameFlow: startGameButton (开始游戏按钮) 未在 Inspector 中指定。");
        }

        // (可选) 获取相机初始速度，如果恢复速度不是固定值的话
        if (CameraSystem.Instance)
        {
            originalCameraSpeed = CameraSystem.Instance.MoveSpeed;
        }
        
        if(playerPanelButton)
            playerPanelButton.SetActive(false);
    }

    // 开始场景演出
    private void StartGameSequence()
    {
        // 0. (可选) 禁用按钮避免重复点击
        if (startGameButton)
        {
            startGameButton.interactable = false;
        }

        // 1. 失活开始菜单
        if (startMenuPanel)
        {
            startMenuPanel.SetActive(false);
        }

        // 2. 检查 CameraSystem 和 PlayerMove.CurrentPlayer
        if (!CameraSystem.Instance)
        {
            Debug.LogError("GameFlow: CameraSystem.Instance 为空。无法执行相机移动序列。");
            if (startGameButton) startGameButton.interactable = true; // 发生错误，恢复按钮交互
            return;
        }

        if (!PlayerMove.CurrentPlayer || PlayerMove.CurrentPlayer.transform == null)
        {
            Debug.LogError("GameFlow: PlayerMove.CurrentPlayer 或其 transform 为空。无法设置相机目标。请确保玩家对象已正确初始化并赋值给 PlayerMove.CurrentPlayer。");
            return;
        }

        // 3. 设置相机移动速度为1
        CameraSystem.Instance.MoveSpeed = 1f;

        // 4. 设置相机移动视点到玩家对象
        var playerTransform = PlayerMove.CurrentPlayer.transform;
        CameraSystem.SetSpecialCameraTarget(playerTransform); // 使用 CameraSystem 提供的静态方法

        // 5. 监听相机到达目标事件
        CameraSystem.OnCameraArrivedAtSpecialTarget += HandleCameraArrivedAtPlayer;
    }

    private async void HandleCameraArrivedAtPlayer() 
    {
        // 1. 恢复相机速度
        if (CameraSystem.Instance) {
            CameraSystem.Instance.MoveSpeed = 5f; 
        }

        // 2. 取消事件监听
        CameraSystem.OnCameraArrivedAtSpecialTarget -= HandleCameraArrivedAtPlayer;

        // 3. 激活玩家控制
        if (PlayerInputController.Instance) {
            PlayerInputController.Instance.ActivatePlayerControls();
        }
    
        // 4. 停止背景音乐
        AudioManager.Instance.Stop(beginPanelMusic);

        // 5. 设置游戏状态
        GameVariables.Day = 1;
        if(playerPanelButton) playerPanelButton.SetActive(true);

        // 6. 触发事件并等待对话
        EventCenter.TriggerEvent(GameEvents.GameStartsPlayerWakesUp);
        
        await WaitForDialogueAsync(); // 明确等待
    }

    private async UniTask WaitForDialogueAsync() {
        
        // 等待对话完成
        await WaitForEvent(
            h => DialogueManager.DialogueFinished += h,
            h => DialogueManager.DialogueFinished -= h
            );
    
        // 移动相机
        CameraSystem.SetSpecialCameraTarget(GameVariables.StartAndFindCameraPosition);

        // 停留1秒后开启寻找
        await UniTask.WaitForSeconds(1f);
        
        PlayerDialogue.Instance.FindLora(GameVariables.StartAndFindCameraPosition);
        
        // 等待对话完成
        await WaitForEvent(
            h => DialogueManager.DialogueFinished += h,
            h => DialogueManager.DialogueFinished -= h
        );
        
        CameraSystem.SetSpecialCameraTarget(PlayerMove.CurrentPlayer.transform);
    }
    
    private static async UniTask WaitForEvent(Action<Action> addListener, Action<Action> removeListener) {
        var utcs = new UniTaskCompletionSource();
    
        Action handler = null;
        handler = () => {
            removeListener(handler); 
            utcs.TrySetResult();
        };

        addListener(handler);
        await utcs.Task;
    }
    

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 当此 GameFlow 对象销毁时，确保取消事件订阅，以防内存泄漏
    private void OnDestroy()
    {
        // 确保在对象销毁时，如果事件仍然被订阅，则取消订阅
        // 这种情况可能发生在 HandleCameraArrivedAtPlayer 从未被调用的情况下（例如，场景切换或对象被销毁）
        CameraSystem.OnCameraArrivedAtSpecialTarget -= HandleCameraArrivedAtPlayer;

        // 如果按钮存在，也移除监听器
        if (startGameButton)
        {
            startGameButton.onClick.RemoveListener(StartGameSequence);
        }
    }
}