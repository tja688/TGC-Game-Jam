using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // <--- 确保添加了这一行

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
    public GameObject PlayerPanelButton;

    private float originalCameraSpeed; // 用于存储相机原始速度 (如果需要恢复到非固定值)

    private void Start()
    {
        // 1. 播放背景音乐
        if (!beginPanelMusic) // 修正：变量名应该是 streetMainMusic
        {
            Debug.LogError("GameFlow: streetMainMusic (背景音乐 SoundEffect) 未在 Inspector 中指定。");
        }
        else
        {
            AudioManager.Instance.Play(beginPanelMusic);
        }

        // 2. 激活开始菜单
        if (startMenuPanel)
        {
            startMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameFlow: startMenuPanel (开始菜单 GameObject) 未在 Inspector 中指定。");
        }

        // 3. 设置初始输入模式为 UI 控制
        if (PlayerInputController.Instance != null)
        {
            PlayerInputController.Instance.ActivateUIControls();
        }
        else
        {
            Debug.LogError("GameFlow: PlayerInputController.Instance 为空。无法设置初始输入模式。");
        }

        // 4. 为开始按钮添加监听器
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGameSequence);
        }
        else
        {
            Debug.LogError("GameFlow: startGameButton (开始游戏按钮) 未在 Inspector 中指定。");
        }

        // (可选) 获取相机初始速度，如果恢复速度不是固定值的话
        if (CameraSystem.Instance != null)
        {
            originalCameraSpeed = CameraSystem.Instance.MoveSpeed;
        }
        
        if(PlayerPanelButton)
            PlayerPanelButton.SetActive(false);
    }

    void StartGameSequence()
    {
        Debug.Log("GameFlow: StartGameSequence initiated.");

        // 0. (可选) 禁用按钮避免重复点击
        if (startGameButton != null)
        {
            startGameButton.interactable = false;
        }

        // 1. 失活开始菜单
        if (startMenuPanel != null)
        {
            startMenuPanel.SetActive(false);
        }

        // 2. 检查 CameraSystem 和 PlayerMove.CurrentPlayer
        if (CameraSystem.Instance == null)
        {
            Debug.LogError("GameFlow: CameraSystem.Instance 为空。无法执行相机移动序列。");
            if (startGameButton != null) startGameButton.interactable = true; // 发生错误，恢复按钮交互
            return;
        }

        if (PlayerMove.CurrentPlayer == null || PlayerMove.CurrentPlayer.transform == null)
        {
            Debug.LogError("GameFlow: PlayerMove.CurrentPlayer 或其 transform 为空。无法设置相机目标。请确保玩家对象已正确初始化并赋值给 PlayerMove.CurrentPlayer。");
            // 视情况决定是否恢复UI或按钮
            // 例如：startMenuPanel?.SetActive(true);
            // if (startGameButton != null) startGameButton.interactable = true;
            // PlayerInputController.Instance?.ActivateUIControls(); // 恢复UI控制
            return;
        }

        // 3. 设置相机移动速度为1
        CameraSystem.Instance.MoveSpeed = 1f;
        Debug.Log($"GameFlow: Camera speed set to {CameraSystem.Instance.MoveSpeed}");

        // 4. 设置相机移动视点到玩家对象
        Transform playerTransform = PlayerMove.CurrentPlayer.transform;
        CameraSystem.SetSpecialCameraTarget(playerTransform); // 使用 CameraSystem 提供的静态方法
        Debug.Log($"GameFlow: Camera special target set to player: {playerTransform.name}");

        // 5. 监听相机到达目标事件
        CameraSystem.OnCameraArrivedAtSpecialTarget += HandleCameraArrivedAtPlayer;
        Debug.Log("GameFlow: Subscribed to OnCameraArrivedAtSpecialTarget event.");
    }

    void HandleCameraArrivedAtPlayer()
    {
        Debug.Log("GameFlow: HandleCameraArrivedAtPlayer called (camera arrived at player).");

        // 1. 检查 CameraSystem 实例
        if (CameraSystem.Instance == null)
        {
            Debug.LogError("GameFlow: CameraSystem.Instance 为空，在 HandleCameraArrivedAtPlayer 中。无法恢复相机速度。");
            return; // 提前退出，避免后续空指针
        }
        
        // 2. 设置相机移动速度回5 (或原始速度)
        CameraSystem.Instance.MoveSpeed = 5f; // 或者使用 originalCameraSpeed
        Debug.Log($"GameFlow: Camera speed restored to {CameraSystem.Instance.MoveSpeed}");

        // 3. 取消监听事件，避免重复执行
        CameraSystem.OnCameraArrivedAtSpecialTarget -= HandleCameraArrivedAtPlayer;
        Debug.Log("GameFlow: Unsubscribed from OnCameraArrivedAtSpecialTarget event.");

        // 4. 设置游戏控制为玩家控制组
        if (PlayerInputController.Instance != null)
        {
            PlayerInputController.Instance.ActivatePlayerControls();
            Debug.Log("GameFlow: Player controls activated.");
        }
        else
        {
            Debug.LogError("GameFlow: PlayerInputController.Instance 为空。无法激活玩家控制。");
        }
        
        AudioManager.Instance.Stop(beginPanelMusic);
        
        if(PlayerPanelButton)
            PlayerPanelButton.SetActive(true);

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