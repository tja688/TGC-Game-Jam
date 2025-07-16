using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class KeyItem : ItemBase
{
    [ConversationPopup]
    public string conversationTitle;
    
    public override void Interact(GameObject player)
    {
        base.Interact(player);
        
        MessageTipManager.ShowMessage("Got the post office key.");
        
        DialogueManager.StartConversation(conversationTitle);
        
    }

    
    
}
