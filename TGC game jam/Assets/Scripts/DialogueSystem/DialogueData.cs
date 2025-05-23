// TJAnimationDialogueManager.cs (或者一个单独的 DialogueData.cs 文件)
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
// 确保引入 Febucci Text Animator 的相关命名空间
using Febucci.UI.Core;
using Febucci.UI.Core.Parsing;

// 可配置的单个对话页面/片段
[System.Serializable]
public class DialoguePage
{
    [TextArea(3, 10)] // 让 Inspector 中的文本框更大一些
    public string textContent; // 包含 Text Animator 富文本标签的对话内容
    // 未来可在这里扩展：例如，该页特定的音效、摄像机镜头等
}

// 可配置的完整对话序列 (由一个或多个页面组成)
[System.Serializable]
public class DialogueSequence
{
    [Tooltip("用于外部 DialogueLogicManager 调用的唯一标识符")]
    public string dialogueTag; // 例如 "NPC001_Greeting", "NPC001_QuestAccepted"
    public List<DialoguePage> pages; // 一个对话序列可以包含多个页面，按顺序播放

    public DialogueSequence(string tag) { dialogueTag = tag; pages = new List<DialoguePage>(); }
}

// 可配置的事件映射 (将文本内联事件标签关联到实际的游戏逻辑)
[System.Serializable]
public class TJEventMapping
{
    [Tooltip("在对话文本中使用的事件标签，如 <event=TAG_NAME>")]
    public string eventTagInText; // 例如 "OPEN_DUNGEON_DOOR", "PLAYER_RECEIVED_ITEM"

    [Tooltip("触发此事件时是否暂停文本的打字播放？")]
    public bool pauseTextOnTrigger;

    [Tooltip("当事件标签被触发时，将调用的 UnityEvent。可以传递参数。")]
    public UnityEvent<string[]> onTriggered; // string[] 用于接收来自 <event=TAG:param1:param2> 的参数
}