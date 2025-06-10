using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class DialogueEntry
{
    [Tooltip("对话的唯一标识符 (例如：Greeting, Farewell, QuestStart)")]
    public string dialogueID;

    [Tooltip("NPC 要说的具体文本内容")]
    [TextArea(3, 10)] 
    public string dialogueText; 
}

[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC/Dialogue Configuration", order = 1)]
public class NPCDialogue : ScriptableObject
{
    [Tooltip("此 NPC 的显示名称 (可选)")]
    public string npcName; 

    [Tooltip("此 NPC 的所有对话条目列表")]
    public List<DialogueEntry> dialogueEntries = new List<DialogueEntry>(); 

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