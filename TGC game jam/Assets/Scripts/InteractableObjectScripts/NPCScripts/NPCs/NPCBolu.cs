using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class NPCBolu : ItemBase
{
    [ConversationPopup]
    public string conversationTitle;

    public override void Interact(GameObject instigator)
    {
        DialogueManager.StartConversation(conversationTitle);
    }

}
