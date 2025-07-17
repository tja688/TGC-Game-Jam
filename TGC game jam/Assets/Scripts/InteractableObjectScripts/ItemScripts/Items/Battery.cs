using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class Battery : ItemBase
{
    [ConversationPopup] public string conversationTitle;

    public override void Interact(GameObject player)
    {
        DialogueManager.StartConversation(conversationTitle);

        base.Interact(player);
    }
}
