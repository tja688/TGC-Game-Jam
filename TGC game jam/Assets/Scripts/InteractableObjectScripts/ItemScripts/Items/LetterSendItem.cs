using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class LetterSendItem : ItemBase
{
    [ConversationPopup]
    public string conversationTitle;

    public override void Interact(GameObject player)
    {
        if (grabSound)
            AudioManager.Instance.Play(grabSound);
        
        MessageTipManager.ShowMessage("MAIL DELIVERY SYSTEM: ONLINE");
        
        DialogueManager.StartConversation(conversationTitle);
    
    }
}
