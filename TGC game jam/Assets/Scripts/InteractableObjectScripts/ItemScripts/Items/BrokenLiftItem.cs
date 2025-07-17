using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using PixelCrushers.DialogueSystem;

public class BrokenLiftItem : ItemBase
{
    public GameObject lift;
    [ConversationPopup]
    public string conversationTitle;
    public override void Interact(GameObject player)
    {
        AudioManager.Instance.Play(grabSound);
        
        if (GameVariables.Day == 4)
        {
            lift.SetActive(true);
            MessageTipManager.ShowMessage("Elevator available.");

            this.gameObject.SetActive(false);
        }
        else
        {
            DialogueManager.StartConversation(conversationTitle);
        }
        
    }
}
