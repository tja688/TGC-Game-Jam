using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class OtherDoor : ItemBase
{

    [ConversationPopup] public string conversationTitle;

    public override void Interact(GameObject instigator)
    {
        AudioManager.Instance.Play(grabSound);

        DialogueManager.StartConversation(conversationTitle);
    }
}
