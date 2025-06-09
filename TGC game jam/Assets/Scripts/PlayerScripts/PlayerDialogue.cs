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
    
    public void Charging()
    {
        var ids = new List<string> { "charging" };
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
    
    public void StoreClosed()
    {
        var ids = new List<string> { "storeclosed" };
        DialogueManager.Instance.StartDialogueSequence(ids, playerInternalMonologueData, playerTalkTransform, false);
    }

    
    public void FindPaper()
    {
        var ids = new List<string> { "findpaper" };
        DialogueManager.Instance.StartDialogueSequence(ids, playerInternalMonologueData, playerTalkTransform, false);
    }
    
    public void FixSender()
    {
        var ids = new List<string> { "fixsender" };
        DialogueManager.Instance.StartDialogueSequence(ids, playerInternalMonologueData, playerTalkTransform, false);
    }
    
    public void LetterSender()
    {
        var ids = new List<string> { "lettersender" };
        DialogueManager.Instance.StartDialogueSequence(ids, playerInternalMonologueData, playerTalkTransform, false);
    }
    
    public void EndTalk()
    {
        var ids = new List<string> { "end1", "end2",  "end3",  "end4",  "end5",  "end6",  "end7",  "end8",  "end9",  "end10",  "end11",  "end12" };
        DialogueManager.Instance.StartDialogueSequence(ids, playerInternalMonologueData, playerTalkTransform, true, () => {
        });   
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


    public void Day2Grandma()
    {
        var dialogueIDs = new List<string> { "day2tograndma1", "day2tograndma2", "day2tograndma3", "day2tograndma4" , "day2tograndma5" , "day2tograndma6" , "day2tograndma7" , "day2tograndma8" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.GrandmaInSecondFloor, true, () => {
        });

    }
    
    public void Day2Restaurant()
    {
        var dialogueIDs = new List<string> { "day2torestaurant1", "day2torestaurant2", "day2torestaurant3", "day2torestaurant4" , "day2torestaurant5" , "day2torestaurant6" , "day2torestaurant7" , "day2torestaurant8" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.Restaurant, true, () => {
        });

    }
    
    public void Day3ToBolu()
    {
        var dialogueIDs = new List<string> { "day3tobolu1",  "day3tobolu2", "day3tobolu3",  "day3tobolu4",   "day3tobolu5",  "day3tobolu6",   "day3tobolu7",   "day3tobolu8", "day3tobolu9", "day3tobolu10","day3tobolu11" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.BoluSetInStore, true, () => {
        });

    }
    
    
    public async UniTask Day3ToGrandma1()
    {
        var dialogueIDs = new List<string> { "day3tograndma1", "day3tograndma2", "day3tograndma3" , "day3tograndma4" , "day3tograndma5" , "day3tograndma6" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.GrandmaInSecondFloor , false);

        await Day3ToGrandma2();
    }
    
    public async UniTask Day3ToGrandma2()
    {
        // 等待对话完成
        await GameFlow.WaitForEvent(
            h => DialogueManager.DialogueFinished += h,
            h => DialogueManager.DialogueFinished -= h
        );
        
        MessageTipManager.ShowMessage("Gave Grandma the paper flowers folded yesterday.");
        
        var dialogueIDs = new List<string> { "day3tograndma7", "day3tograndma8" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.GrandmaInSecondFloor, false);
        
    }
    
    public async UniTask Day3ToRestaurant1()
    {
        var dialogueIDs = new List<string> { "day3torestaurant1",  "day3torestaurant2", "day3torestaurant3",  "day3torestaurant4",   "day3torestaurant5" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.Restaurant, false);

        await Day3ToRestaurant2();
    }
    
    public async UniTask Day3ToRestaurant2()
    {
        // 等待对话完成
        await GameFlow.WaitForEvent(
            h => DialogueManager.DialogueFinished += h,
            h => DialogueManager.DialogueFinished -= h
        );
        
        CameraSystem.SetSpecialCameraTarget(GameVariables.BatteryPos);
        
        await UniTask.WaitForSeconds(2f);
        
        CameraSystem.SetSpecialCameraTarget(PlayerMove.CurrentPlayer.transform);

        await UniTask.WaitForSeconds(1f);
        
        var dialogueIDs = new List<string> {  "day3torestaurant6" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.Restaurant, false);
        
    }
    
    public void Day4Bolu()
    {
        var dialogueIDs = new List<string> { "day4tobolu1",  "day4tobolu2", "day4tobolu3",   "day4tobolu4", "day4tobolu5", "day4tobolu6",  "day4tobolu7", "day4tobolu8",   "day4tobolu9", "day4tobolu10", "day4tobolu11"  };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, GameVariables.BoluSetInStore, true, () => {
        });

    }
    
    public void Day4Girl()
    {
        var dialogueIDs = new List<string> { "day4togirl1", "day4togirl2" ,  "day4togirl3",  "day4togirl4","day4togirl5", "day4togirl6" ,  "day4togirl7",  "day4togirl8",  "day4togirl9" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, playerTalkTransform, true, () => {
        });

    }

    public void Day5()
    {
        var dialogueIDs = new List<string> { "day51", "day52", "day53", "day54", "day55" };
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerToNPClogueData, playerTalkTransform, true, () => {
        });

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