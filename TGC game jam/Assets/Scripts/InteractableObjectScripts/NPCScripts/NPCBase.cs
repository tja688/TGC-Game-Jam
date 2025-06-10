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
        if (DialogueManager.Instance.CurrentState == DialogueManager.DialogueSystemState.PlayingText)
        {
            Debug.Log("对话系统正在播放文本，请稍候。");
            return;
        }

        if (DialogueManager.Instance.CurrentState == DialogueManager.DialogueSystemState.WaitingForContinuation)
        {
            DialogueManager.Instance.ProceedToNextLine();
        }
        else if (DialogueManager.Instance.CurrentState == DialogueManager.DialogueSystemState.Idle)
        {
            InitiateDialogue();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate(); 

        if (!InstantiatedPromptInstance) return;
        bool shouldShowPrompt = DialogueManager.Instance.CurrentState == DialogueManager.DialogueSystemState.Idle ||
                                DialogueManager.Instance.CurrentState == DialogueManager.DialogueSystemState.WaitingForContinuation;
            

        InstantiatedPromptInstance.SetActive(shouldShowPrompt); 
    }


    public abstract void InitiateDialogue();
    
}