using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class NPCRella : ItemBase
{
    [ConversationPopup]
    public string conversationTitle;

    public override void Interact(GameObject instigator)
    {
        DialogueManager.StartConversation(conversationTitle);
    }

}
