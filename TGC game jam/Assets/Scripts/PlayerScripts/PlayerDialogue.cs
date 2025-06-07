using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDialogue : MonoBehaviour
{
    // 单例实例
    private static PlayerDialogue _instance;
    public static PlayerDialogue Instance
    {
        get
        {
            if (_instance) return _instance;
            _instance = FindObjectOfType<PlayerDialogue>();
            if (_instance) return _instance;
            var obj = new GameObject("PlayerDialogue");
            _instance = obj.AddComponent<PlayerDialogue>();
            DontDestroyOnLoad(obj); // 可选：跨场景不销毁
            return _instance;
        }
    }

    private TypewriterCore typewriter;
    public GameFlow gameFlow;
    
    [SerializeField] private Transform playerTalkTransform;
    [SerializeField] private NPCDialogue playerInternalMonologueData;
    
    private void Awake()
    {
        if (_instance && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject); // 可选：跨场景不销毁
    }


    private void Start()
    {
        
        typewriter = DialogueManager.Instance.Typewriter;
        
        EventCenter.AddEventListener(GameEvents.GameStartsPlayerWakesUp, OnGameStartsPlayerWakesUp);

    }

    private void OnGameStartsPlayerWakesUp()
    {
        if (GameVariables.Day != 1) return; 
        if (GameVariables.DebugNoOpener) return;

        var dialogueIDs = new List<string> { "initial1", "initial2", "initial3", "initial4", "initial5" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerInternalMonologueData, playerTalkTransform, true, () => {
        });
    }

    public void FindLora(Transform t)
    {
        var ids = new List<string> { "findlora" };
        DialogueManager.Instance.StartDialogueSequence(ids, playerInternalMonologueData, t, false);
    }
    
    public void SendLetter()
    {
        var ids = new List<string> { "sendletter" };
        DialogueManager.Instance.StartDialogueSequence(ids, playerInternalMonologueData, playerTalkTransform, false);
    }
    
    public void Roadsigns()
    {
        var ids = new List<string> { "roadsigns" };
        DialogueManager.Instance.StartDialogueSequence(ids, playerInternalMonologueData, playerTalkTransform, false);
    }
    
    private void OnDestroy()
    {
        if (_instance != this) return;
        EventCenter.RemoveListener(GameEvents.GameStartsPlayerWakesUp, OnGameStartsPlayerWakesUp);
        _instance = null;
    }
}