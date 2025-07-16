using UnityEngine;
// 别忘了引用Dialogue System的命名空间！
using PixelCrushers.DialogueSystem;

public class ExternalDialogueContinue : MonoBehaviour
{
    /// <summary>
    /// 从外部调用此方法以继续对话。可以关联到按钮的OnClick事件。
    /// </summary>
    public void ContinueDialogue()
    {
        // 步骤1: 检查对话是否正在进行
        if (PixelCrushers.DialogueSystem.DialogueManager.isConversationActive)
        {
            // 步骤2: 获取当前的标准对话UI实例
            StandardDialogueUI ui = PixelCrushers.DialogueSystem.DialogueManager.standardDialogueUI;

            // 步骤3: 检查UI实例是否存在（这是一个好的防御性编程习惯）
            if (ui != null)
            {
                Debug.Log("外部脚本触发对话继续 (Final, Correct API)...");

                // 步骤4: 调用UI总控制器上的“继续对话”方法
                ui.OnContinueConversation();
            }
            else
            {
                Debug.LogWarning("StandardDialogueUI instance not found. Cannot continue conversation via external script.");
            }
        }
        else
        {
            // 如果对话没有在进行，不执行任何操作
        }
    }
}