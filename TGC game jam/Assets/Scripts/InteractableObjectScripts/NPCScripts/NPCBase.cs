using UnityEngine;

public abstract class NPCBase : InteractableObjectBase
{
    [SerializeField] protected NPCDialogue dialogueData;

    protected override void Start()
    {
        base.Start();
        if (!dialogueData)
        {
            Debug.LogError($"NPC {gameObject.name} 没有配置对话数据 (NPCDialogue ScriptableObject)。", this);
        }
    }

    public override void Interact(GameObject instigator)
    {
        // 如果DialogueManager正忙于播放文本，则不响应新的交互
        if (DialogueManagerOld.Instance.CurrentState == DialogueManagerOld.DialogueSystemState.PlayingText)
        {
            Debug.Log("对话系统正在播放文本，请稍候。");
            return;
        }

        // 如果DialogueManager正在等待继续，则发送继续信号
        if (DialogueManagerOld.Instance.CurrentState == DialogueManagerOld.DialogueSystemState.WaitingForContinuation)
        {
            DialogueManagerOld.Instance.ProceedToNextLine();
        }
        // 如果DialogueManager空闲，则发起新的对话
        else if (DialogueManagerOld.Instance.CurrentState == DialogueManagerOld.DialogueSystemState.Idle)
        {
            InitiateDialogue();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate(); // 处理玩家距离等逻辑

        if (!InstantiatedPromptInstance) return; // 管理交互提示的显隐
        bool shouldShowPrompt = DialogueManagerOld.Instance.CurrentState == DialogueManagerOld.DialogueSystemState.Idle ||
                                DialogueManagerOld.Instance.CurrentState == DialogueManagerOld.DialogueSystemState.WaitingForContinuation;
            
        // 还需要考虑玩家是否在交互范围内等其他条件
        // isPlayerInRangeAndLooking 等 InteractableObjectBase 中的条件
        InstantiatedPromptInstance.SetActive(shouldShowPrompt); // isPlayerInRange 是 InteractableObjectBase 的一个假设字段
    }

    /// <summary>
    /// 抽象方法，由子类实现具体的对话发起逻辑。
    /// 通常会调用 DialogueManager.Instance.StartDialogueSequence(...)
    /// </summary>
    public abstract void InitiateDialogue();
    
}