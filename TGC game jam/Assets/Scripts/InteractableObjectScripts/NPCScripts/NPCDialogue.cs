using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 代表单个对话条目，包含一个标识符和对话文本。
/// </summary>
[System.Serializable]
public class DialogueEntry
{
    [Tooltip("对话的唯一标识符 (例如：Greeting, Farewell, QuestStart)")]
    public string dialogueID; // 对话语句的标识

    [Tooltip("NPC 要说的具体文本内容")]
    [TextArea(3, 10)] // 使文本框在 Inspector 中可以多行显示
    public string dialogueText; // 对话语句的内容
}

/// <summary>
/// ScriptableObject 用于存储特定 NPC 的对话库。
/// 可以在 Unity 编辑器中创建和配置此类型的资源。
/// </summary>
[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC/Dialogue Configuration", order = 1)]
public class NPCDialogue : ScriptableObject
{
    [Tooltip("此 NPC 的显示名称 (可选)")]
    public string npcName; // NPC的名字（可选）

    [Tooltip("此 NPC 的所有对话条目列表")]
    public List<DialogueEntry> dialogueEntries = new List<DialogueEntry>(); // 存储所有对话语句的列表

    /// <summary>
    /// 根据提供的 ID 查找并返回对话文本。
    /// </summary>
    /// <param name="id">要查找的对话标识符。</param>
    /// <returns>如果找到，则返回对话文本；否则返回 null。</returns>
    public string GetDialogueTextByID(string id)
    {
        DialogueEntry entry = dialogueEntries.Find(e => e.dialogueID == id);
        if (entry != null)
        {
            return entry.dialogueText;
        }
        Debug.LogWarning($"在 NPC '{npcName}' (或未命名 NPC) 的对话库中未找到 ID 为 '{id}' 的对话。");
        return null;
    }
}