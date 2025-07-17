// 引入官方文档指定的命名空间

using System;
using PixelCrushers.DialogueSystem;
using UnityEngine;

/// <summary>
/// 根据官方文档，使用标准API，在按下空格键时触发指定对话。
/// </summary>
public class DialogueTestTrigger : MonoBehaviour
{
    [Tooltip("要启动的对话的标题，该标题在Dialogue Editor中定义")]
    [ConversationPopup]
    public string conversationTitle;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartDialogue();
    }

    void StartDialogue()
    {
        // 检查标题是否为空，避免不必要的错误
        if (!string.IsNullOrEmpty(conversationTitle))
        {
            // 使用官方文档中最常用的方法来启动对话
            DialogueManager.StartConversation(conversationTitle);
        }
        else
        {
            Debug.LogWarning("对话标题未指定，无法开始对话！", this);
        }
    
    }
}