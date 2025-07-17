using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class ScrapItem : ItemBase
{
    [ConversationPopup]
    public string conversationTitle;
    
    public override void Interact(GameObject player)
    {
        AudioManager.Instance.Play(grabSound);
  
        DialogueManager.StartConversation(conversationTitle);
        
        this.gameObject.SetActive(false);
    }
}
