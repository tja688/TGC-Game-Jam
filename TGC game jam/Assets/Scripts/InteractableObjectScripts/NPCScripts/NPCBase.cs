using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class NPCBase : InteractableObjectBase , IChattable
{
    [SerializeField] protected NPCDialogue dialogueData;
    
    protected override void Start()
    {
        base.Start();

        if (dialogueData) return;
        Debug.LogError($"NPC {gameObject.name} 没有配置对话数据 (NPCDialogue ScriptableObject)。", this);
    }
    
    public override void Interact(GameObject instigator)
    {
        // 如果此NPC已经在进行对话，或者整个对话系统正忙于其他对话，则阻止新的交互
        if (DialogueManager.Instance.isDialogueProcessActive)
        {
            Debug.Log($"对话系统正忙, 请等待当前对话结束。"); 
            return;
        }
        
        InitiateDialogue();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if(InstantiatedPromptInstance)
            InstantiatedPromptInstance.SetActive(!DialogueManager.Instance.isDialogueProcessActive);
    }

    /// <summary>
    /// 抽象方法，由子类实现具体的对话发起逻辑。
    /// </summary>
    /// <param name="instigator">交互的发起者</param>
    public abstract void InitiateDialogue();

    /// <summary>
    /// 辅助方法，用于NPC发送对话内容。
    /// </summary>
    /// <param name="dialogueID">要发送的对话ID。</param>
    /// <param name="customPosition">可选的自定义对话框位置。</param>
    public void SendDialogueLine( Vector2 customPosition, string dialogueID)
    {
        var textToSend = dialogueData.GetDialogueTextByID(dialogueID);
        if (string.IsNullOrEmpty(textToSend))
        {
            Debug.LogError("dialogueID: "+ dialogueID +"is empty!");
        }
        else
        {
            EventCenter.TriggerEvent<Vector2,string>(GameEvents.ShowDialogue,customPosition,textToSend);
        }
    }
}
