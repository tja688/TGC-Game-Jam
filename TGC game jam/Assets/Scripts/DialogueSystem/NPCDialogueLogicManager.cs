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
    public string dialogueTagToTest = "Greeting"; // 默认测试一个名为 "Greeting" 的对话标签

    // 输入Action实例 (基于你创建的 .inputactions 资源生成的类)
    // 假设你的Input Actions资源生成了名为 "PlayerInputActions" 的类
    private InputActions inputActions;

    void Awake()
    {
        // 1. 初始化 Input Actions
        inputActions = new InputActions(); // 使用你生成的C#类名

        // 2. 直接获取对象上的TJAnimationDialogueManager (如果脚本也挂在NPC上，或者用于特定场景)
        // 如果这个NPCDialogueLogicManager是设计为总是与同一个GameObject上的TJAnimationDialogueManager交互：
        // if (targetNpcDialogueManager == null)
        // {
        //     targetNpcDialogueManager = GetComponent<TJAnimationDialogueManager>();
        // }
        // 但为了测试灵活性，我们优先使用公共字段 targetNpcDialogueManager 来指定目标。
        // 如果这个脚本是挂载在玩家身上，那么 targetNpcDialogueManager 必须在Inspector中手动指定，
        // 或者通过更复杂的逻辑（如射线检测、触发器）来动态确定当前可交互的NPC。
        // 为了“一切从简”，我们这里依赖于在Inspector中手动指定 targetNpcDialogueManager。

        if (targetNpcDialogueManager == null)
        {
            Debug.LogWarning("NPCDialogueLogicManager: targetNpcDialogueManager 未在Inspector中设置。请分配一个目标NPC的对话管理器。", this);
        }
    }

    void OnEnable()
    {
        // 启用 Action Map
        inputActions.PlayerControl.Enable(); // 假设你的Action Map名为 "PlayerControl"

        // 订阅交互事件
        // 假设你的Action名为 "Interact"
        inputActions.PlayerControl.Interact.performed += OnInteractInputPerformed;
    }

    void OnDisable()
    {
        // 取消订阅并禁用 Action Map，防止内存泄漏和不必要的处理
        if (inputActions != null)
        {
            inputActions.PlayerControl.Interact.performed -= OnInteractInputPerformed;
            inputActions.PlayerControl.Disable();
        }
    }

    /// <summary>
    /// 当玩家按下交互按键时调用此方法。
    /// </summary>
    private void OnInteractInputPerformed(InputAction.CallbackContext context)
    {
        // 3. 当玩家按交互键时，发起对话
        Debug.Log("交互按键被按下！");

        if (targetNpcDialogueManager == null)
        {
            Debug.LogError("NPCDialogueLogicManager: 无法发起对话，因为 targetNpcDialogueManager 未设置！", this);
            return;
        }

        // 检查目标NPC是否可以开始对话
        if (targetNpcDialogueManager.CanStartDialogue())
        {
            Debug.Log($"尝试向NPC '{targetNpcDialogueManager.gameObject.name}' 发起对话，标签: '{dialogueTagToTest}'");
            bool success = targetNpcDialogueManager.RequestShowDialogue(dialogueTagToTest);
            if (success)
            {
                Debug.Log($"对话 '{dialogueTagToTest}' 成功开始。");
            }
            else
            {
                Debug.LogWarning($"对话 '{dialogueTagToTest}' 未能开始 (可能是标签无效或NPC内部逻辑问题)。");
            }
        }
        else
        {
            Debug.Log($"NPC '{targetNpcDialogueManager.gameObject.name}' 当前正忙，无法开始新的对话。");
            // 在这里，我们遵循了“不影响现有文本展示”的原则，只是打印一条日志。
            // 实际游戏中，你可能想给玩家一个“NPC正忙”的反馈。
        }
    }

    // (可选) 如果你想在运行时动态改变测试的NPC或对话标签，可以添加公共方法
    public void SetTargetNPC(TJAnimationDialogueManager npcManager)
    {
        targetNpcDialogueManager = npcManager;
        if (targetNpcDialogueManager == null)
        {
             Debug.LogWarning("NPCDialogueLogicManager: SetTargetNPC 时传入了空的 npcManager。", this);
        }
    }

    public void SetDialogueTagToTest(string tag)
    {
        dialogueTagToTest = tag;
    }
}