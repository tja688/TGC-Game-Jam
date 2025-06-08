using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using UnityEngine;
using UnityEngine.Serialization;
using Cysharp.Threading.Tasks;


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
    [SerializeField] private NPCDialogue playerToNPClogueData;

    
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

    #region PlayerSelfTalking

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
    
    public void LiftNotice()
    {
        var ids = new List<string> { "liftnotice" };
        DialogueManager.Instance.StartDialogueSequence(ids, playerInternalMonologueData, playerTalkTransform, false);
    }

    public void Day1OutSideTalk()
    {
        var dialogueIDs = new List<string> { "outsidedialogue1", "outsidedialogue2", "outsidedialogue3" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerInternalMonologueData, playerTalkTransform, true, () => {
        });
    }
    
    public void Day2OutSideTalk()
    {
        var dialogueIDs = new List<string> { "outsidedialogue4", "outsidedialogue2", "outsidedialogue4" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerInternalMonologueData, playerTalkTransform, true, () => {
        });
    }
    
    public void Day3OutSideTalk()
    {
        var dialogueIDs = new List<string> { "outsidedialogue5", "outsidedialogue2", "outsidedialogue3" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerInternalMonologueData, playerTalkTransform, true, () => {
        });
    }
    
    public void Day4OutSideTalk()
    {
        var dialogueIDs = new List<string> { "outsidedialogue6", "outsidedialogue2", "outsidedialogue3" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerInternalMonologueData, playerTalkTransform, true, () => {
        });
    }
    
    public void Day5OutSideTalk()
    {
        var dialogueIDs = new List<string> { "outsidedialogue7", "outsidedialogue2", "outsidedialogue3" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerInternalMonologueData, playerTalkTransform, true, () => {
        });
    }
    
    
    public void Day1ImagingStoreEmail()
    {
        var ids = new List<string> { "imagingstoreday1" };
        DialogueManager.Instance.StartDialogueSequence(ids, playerInternalMonologueData, playerTalkTransform, false);
    }
    


    #endregion


    #region PlayerToNPCTalking

    public async UniTask Day1Bolu()
    {
        var dialogueIDs = new List<string> { "day1bolu1", "day1bolu2", "day1bolu3" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, playerTalkTransform, true, () => {
        });

        await BoluGiveLetter();
    }
    
    public async UniTask Day1Grandma()
    {
        var dialogueIDs = new List<string> { "day1grandma1"};
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, playerTalkTransform, false);

        await Day1Grandma2();
    }
    
    
    public async UniTask Day1Grandma2()
    {
        // 等待对话完成
        await GameFlow.WaitForEvent(
            h => DialogueManager.DialogueFinished += h,
            h => DialogueManager.DialogueFinished -= h
        );
        
        var dialogueIDs = new List<string> { "day1grandma2"};
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.GrandmaInSecondFloor, false);

        await Day1Grandma3();
    }
    
    public async UniTask Day1Grandma3()
    {
        // 等待对话完成
        await GameFlow.WaitForEvent(
            h => DialogueManager.DialogueFinished += h,
            h => DialogueManager.DialogueFinished -= h
        );
        
        var dialogueIDs = new List<string> { "day1grandma3", "day1grandma4"};
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, playerTalkTransform, true, () => {
        });

    }
    
    
    public async UniTask Day1Restaurant()
    {
        var dialogueIDs = new List<string> { "day1restaurant1"};
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.Restaurant, false);

        await Day1Restaurant2();
    }
    
    public async UniTask Day1Restaurant2()
    {
        // 等待对话完成
        await GameFlow.WaitForEvent(
            h => DialogueManager.DialogueFinished += h,
            h => DialogueManager.DialogueFinished -= h
        );
        
        var dialogueIDs = new List<string> { "day1restaurant2"};
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, playerTalkTransform, false);

        await Day1Restaurant3();
    }
    
    public async UniTask Day1Restaurant3()
    {
        // 等待对话完成
        await GameFlow.WaitForEvent(
            h => DialogueManager.DialogueFinished += h,
            h => DialogueManager.DialogueFinished -= h
        );
        
        var dialogueIDs = new List<string> { "day1restaurant3"};
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.Restaurant, false);
    }



    #endregion

    
    #region OtherEvents

    private async UniTask BoluGiveLetter()
    {
        // 等待对话完成
        await GameFlow.WaitForEvent(
            h => DialogueManager.DialogueFinished += h,
            h => DialogueManager.DialogueFinished -= h
        );

        BackpackManager.Instance.RetrieveItem("Letter1-2");
        MessageTipManager.ShowMessage("Letter has been delivered");
        GameVariables.Day1LetterSend++;

    }
    
    
    #endregion
    private void OnDestroy()
    {
        if (_instance != this) return;
        EventCenter.RemoveListener(GameEvents.GameStartsPlayerWakesUp, OnGameStartsPlayerWakesUp);
        _instance = null;
    }
}