using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using UnityEngine;

public class PlayerDialogue : MonoBehaviour
{
    private TypewriterCore typewriter;

    public GameFlow gameFlow;
    
    [SerializeField] private Transform playerTalkTransform;
    
    [SerializeField] private NPCDialogue playerInternalMonologueData;

    
    private void Start()
    {
        
        typewriter = DialogueManager.Instance.Typewriter;
        
        EventCenter.AddEventListener(GameEvents.GameStartsPlayerWakesUp,OnGameStartsPlayerWakesUp);
    }

    private void OnGameStartsPlayerWakesUp()
    {
        // if (GameVariables.Day != 1) return; // 假设有 GameVariables

        var dialogueIDs = new List<string> { "initial1", "initial2", "initial3", "initial4", "initial5" , "initial6", "initial7"};
        
        DialogueManager.Instance.StartDialogueSequence(dialogueIDs, playerInternalMonologueData, playerTalkTransform, true, () => {
            Debug.Log("玩家初始唤醒对话序列完成!");
        });
    }

    private void NextDialogue3()
    {
        
    }


}
