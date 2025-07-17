using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class RestaurantItem : ItemBase
{
    [ConversationPopup]
    public string conversationTitle;

    public override void Interact(GameObject instigator)
    {
        AudioManager.Instance.Play(grabSound);

        if (GameVariables.Day < 4)
        {
            DialogueManager.StartConversation(conversationTitle);
        }
    }
}
