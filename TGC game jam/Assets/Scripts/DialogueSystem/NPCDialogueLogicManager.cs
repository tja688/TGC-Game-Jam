// NPCDialogueLogicManager.cs
using UnityEngine;
using UnityEngine.InputSystem; // 引入新的输入系统命名空间

public class NPCDialogueLogicManager : MonoBehaviour
{
    [Header("目标NPC对话管理器")]
    [Tooltip("将你想要测试对话的NPC（其上挂载有TJAnimationDialogueManager脚本）拖拽到这里")]
    public TJAnimationDialogueManager targetNpcDialogueManager;

    [Header("测试对话设置")]
    [Tooltip("当玩家交互时，要尝试播放的目标NPC的对话标签 (Dialogue Tag)")]
    public string dialogueTagToTest = "Greeting";

    private InputActions inputActions;
    
    public Transform cameraTarget;

    private void Awake()
    {
        inputActions = new InputActions(); 
        
        if (!targetNpcDialogueManager)
        {
            Debug.LogWarning("NPCDialogueLogicManager: targetNpcDialogueManager 未在Inspector中设置。请分配一个目标NPC的对话管理器。", this);
        }
    }

    private void OnEnable()
    {
        inputActions.PlayerControl.Enable(); // 假设你的Action Map名为 "PlayerControl"

        inputActions.PlayerControl.Interact.performed += OnInteractInputPerformed;
    }

    private void OnDisable()
    {
        if (inputActions == null) return;
        inputActions.PlayerControl.Interact.performed -= OnInteractInputPerformed;
        inputActions.PlayerControl.Disable();
    }

    /// <summary>
    /// 当玩家按下交互按键时调用此方法。
    /// </summary>
    private void OnInteractInputPerformed(InputAction.CallbackContext context)
    {
        // if (!targetNpcDialogueManager)
        // {
        //     Debug.LogError("NPCDialogueLogicManager: 无法发起对话，因为 targetNpcDialogueManager 未设置！", this);
        //     return;
        // }
        //
        //
        // var success = targetNpcDialogueManager.RequestDialogueInteraction(dialogueTagToTest);
        // if (success)
        // {
        //     Debug.Log($"对话 '{dialogueTagToTest}' 成功开始。");
        // }
        // else
        // {
        //     Debug.LogWarning($"对话 '{dialogueTagToTest}' 未能开始 (可能是标签无效或NPC内部逻辑问题)。");
        // }
        //
        // CameraSystem.SetCameraTarget(cameraTarget);

    }

    public void SetDialogueTagToTest(string tag1)
    {
        dialogueTagToTest = tag1;
    }
}