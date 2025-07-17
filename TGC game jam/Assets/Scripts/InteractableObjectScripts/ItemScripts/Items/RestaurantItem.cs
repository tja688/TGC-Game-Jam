using UnityEngine;
using PixelCrushers.DialogueSystem; // 确保引入 Dialogue System 的命名空间


[RequireComponent(typeof(DialogueActor))] // 强制要求此GameObject上必须有一个DialogueActor组件
public class RestaurantItem : ItemBase
{
    [Header("Dialogue Settings")]
    [Tooltip("要启动的对话的标题。")]
    [ConversationPopup]
    public string conversationTitle;

    // --- 私有状态变量 ---
    private DialogueActor localDialogueActor; // 用来存储对本地Dialogue Actor组件的引用
    private bool isInteractionAllowed = false;

    #region Unity生命周期与事件订阅

    // Awake在所有Start函数之前被调用，适合用于获取组件引用
    private void Awake()
    {
        // 获取挂载在同一个GameObject上的DialogueActor组件
        localDialogueActor = GetComponent<DialogueActor>();
        if (localDialogueActor == null)
        {
            // RequireComponent属性会防止这种情况发生，但这是一个好的安全检查
            Debug.LogError("[RestaurantItem] 严重错误: 此GameObject上缺少 DialogueActor 组件!", this.gameObject);
        }
    }

    private void OnEnable()
    {
        GameVariables.OnDayChanged += HandleDayChanged;
        HandleDayChanged();
    }

    private void OnDisable()
    {
        GameVariables.OnDayChanged -= HandleDayChanged;
    }

    #endregion

    /// <summary>
    /// 事件处理函数：根据天数，修改本地Dialogue Actor的设置
    /// </summary>
    private void HandleDayChanged()
    {
        if (localDialogueActor == null) return;

        int currentDay = GameVariables.Day;

        switch (currentDay)
        {
            case 1:
                // 第一天：将本地Actor的名字设置为 "EricaMom"，并允许交互
                localDialogueActor.actor = "EricaMom";
                isInteractionAllowed = true;
                break;

            case 2:
            case 3:
                // 第二、三天：将本地Actor的名字设置为 "Erica"，并允许交互
                localDialogueActor.actor = "Erica";
                isInteractionAllowed = true;
                break;

            case 4:
            case 5:
                // 第四、五天：不允许交互
                isInteractionAllowed = false;
                break;

            default:
                // 其他天数，默认不允许交互
                isInteractionAllowed = false;
                break;
        }
    }

    /// <summary>
    /// 覆盖基类的交互方法
    /// </summary>
    public override void Interact(GameObject instigator)
    {
        if (!isInteractionAllowed)
        {
            Debug.Log($"[RestaurantItem] Interaction is disabled on Day {GameVariables.Day}.");
            return;
        }

        if (grabSound != null)
        {
            AudioManager.Instance.Play(grabSound);
        }

        
        // 开始对话。因为我们已经修改了本地Dialogue Actor的设置，
        // 所以对话的"Conversant"（对话者）就是这个GameObject本身 (this.transform)。
        // Dialogue System会自动读取localDialogueActor.actor的值("Erica"或"EricaMom")来显示正确的名字和头像。
        DialogueManager.StartConversation(conversationTitle);
    }
}