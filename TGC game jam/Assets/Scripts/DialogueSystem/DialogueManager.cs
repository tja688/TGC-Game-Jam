using System;
using UnityEngine;
using TMPro;
using System.Collections;
using Febucci.UI.Core; // TypewriterCore 的命名空间
using Febucci.UI.Core.Parsing;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TypewriterCore typewriter;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] public bool isDialogueProcessActive;

    public TypewriterCore Typewriter => typewriter;

    public static DialogueManager Instance { get; private set; }
    
    private void Awake()
    {
        if (!dialogueText)
        {
            Debug.LogError("Dialogue Text (TMP) reference is missing!", this);
        }

        if (!typewriter)
        {
            Debug.LogError("TypewriterCore component is missing!", this);
        }

        if (!dialoguePanel)
        {
            Debug.LogWarning("Dialogue Panel reference is missing! UI visibility might not be managed.", this);
        }
        
        if (Instance && !Equals(Instance, this))
        {
            Debug.LogWarning("场景中存在多个DialogueManager实例，销毁当前这个。", this);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    

    private void OnEnable()
    {
        // 监听显示对话的事件
        EventCenter.AddEventListener<Vector2, string>(GameEvents.ShowDialogue, HandleShowDialogueEvent);
    }

    private void OnDisable()
    {
        // 移除事件监听
        EventCenter.RemoveListener<Vector2, string>(GameEvents.ShowDialogue, HandleShowDialogueEvent);
    }

    /// <summary>
    /// 处理来自事件中心的显示对话请求。
    /// </summary>
    private void HandleShowDialogueEvent(Vector2 pos, string dialogueTexts)
    {
        if (dialoguePanel)
        {
            dialoguePanel.SetActive(true); // 确保UI是可见的
        }
        
        MoveToPosition(pos);
        ShowTextInternal(dialogueTexts);
    }

    /// <summary>
    /// 内部方法，实际显示文本。
    /// </summary>
    private void ShowTextInternal(string text)
    {
        if (!dialogueText || !typewriter) return;
        typewriter.ShowText(text);
    }

    /// <summary>
    /// 移动对话文本框到指定的世界坐标位置。
    /// </summary>
    /// <param name="position">世界坐标</param>
    private void MoveToPosition(Vector2 position)
    {
        if (dialogueText)
        {
            dialogueText.rectTransform.position = position;
        }
    }

    /// <summary>
    /// 这会激活对话系统，允许它响应 ShowDialogue 事件。
    /// </summary>
    public void StartDialogueProcess()
    {
        isDialogueProcessActive =  true;
        PlayerMove.CanPlayerMove = false;
    }

    /// <summary>
    /// 这会关闭对话UI，并阻止对话系统响应 ShowDialogue 事件。
    /// </summary>
    public void EndDialogueProcess()
    {
        isDialogueProcessActive = false;
        PlayerMove.CanPlayerMove = true;
    }
    
}