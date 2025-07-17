using System.Linq;
using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// 外部对话继续控制器（已重构）。
/// 通过访问 TypewriterCommunicator 的全局列表来精确控制打字机。
/// </summary>
public class ExternalDialogueContinue : MonoBehaviour
{
    public void ContinueDialogue()
    {
        if (!DialogueManager.isConversationActive) return;

        bool foundPlayingTypewriter = false;

        // 1. 直接遍历已注册的通讯器列表
        // 这个列表由 TypewriterCommunicator 自身维护，保证了实时性和准确性。
        foreach (var communicator in TypewriterCommunicator.AllCommunicators.Where(communicator => communicator != null && communicator.Typewriter != null && communicator.Typewriter.isPlaying))
        {
            Debug.Log($"通过通讯接口找到正在播放的打字机: {communicator.name}，立即完成它。");
            communicator.Typewriter.Stop();
            foundPlayingTypewriter = true;
            break; // 找到并处理完毕，无需继续遍历
        }

        // 3. 如果没有任何打字机在播放，则继续到下一句
        if (!foundPlayingTypewriter)
        {
            Debug.Log("所有已注册的打字机均未在播放，继续到下一句对话。");
            
            // 依然需要获取UI实例来调用全局的继续方法
            var ui = DialogueManager.standardDialogueUI;
            if (ui != null)
            {
                ui.OnContinueConversation();
            }
        }
    }
}