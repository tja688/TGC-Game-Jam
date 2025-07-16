
using System;
using UnityEngine;
using UnityEngine.UI; // 引用UI命名空间以使用Button类
using PixelCrushers.DialogueSystem; // 引用Dialogue System的命名空间

[RequireComponent(typeof(StandardUISubtitlePanel))]
public class AutoAssignContinueButton : MonoBehaviour
{
    private IDialogueUI subtitlePanel;
    void Start()
    {
        var Panel= GetComponent<StandardUISubtitlePanel>();
        // 1. 获取附加在同一个游戏对象上的 StandardUISubtitlePanel 组件
        subtitlePanel = Panel.dialogueUI;
    }

    private void Update()
    {
        ContinueDialogue();
    }


    private void ContinueDialogue()
    {
        if (!Input.GetMouseButton(0)) return;
        
        if (PixelCrushers.DialogueSystem.DialogueManager.IsConversationActive)
        {
            PixelCrushers.DialogueSystem.DialogueManager.conversationView.OnConversationContinueAll();
        }
        else
        {
            Debug.Log("对话未在进行，不执行继续操作。");
        }
    }
}