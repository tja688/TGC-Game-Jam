using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using UnityEngine;

public class PlayerDialogue : MonoBehaviour
{
    private TypewriterCore typewriter;

    public GameFlow gameFlow;
    
    [SerializeField] private Transform playerTalkTransform;
    
    private void Start()
    {
        
        typewriter = DialogueManager.Instance.Typewriter;
        
        EventCenter.AddEventListener(GameEvents.GameStartsPlayerWakesUp,OnGameStartsPlayerWakesUp);
    }

    private void OnGameStartsPlayerWakesUp()
    {
        if(GameVariables.Day != 1) return;
        
        EventCenter.TriggerEvent<Vector2,string>(GameEvents.ShowDialogue,
            UIUtility.WorldToScreenSpaceOverlayPosition(playerTalkTransform.position),"initial1");

        DialogueManager.DialogueFinished += NextDialogue;

    }

    private void NextDialogue()
    {
        DialogueManager.DialogueFinished -= NextDialogue;
        
        EventCenter.TriggerEvent<Vector2,string>(GameEvents.ShowDialogue,
            UIUtility.WorldToScreenSpaceOverlayPosition(playerTalkTransform.position),"initial2");
        
        
        DialogueManager.DialogueFinished += NextDialogue2;
        
    }
    
    private void NextDialogue2()
    {
        DialogueManager.DialogueFinished -= NextDialogue2;
        
        EventCenter.TriggerEvent<Vector2,string>(GameEvents.ShowDialogue,
            UIUtility.WorldToScreenSpaceOverlayPosition(playerTalkTransform.position),"initial2");
        
        
        DialogueManager.DialogueFinished += NextDialogue3;
        
    }

    private void NextDialogue3()
    {
        
    }


}
