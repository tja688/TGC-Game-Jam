using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterSendItem : ItemBase
{
    public override void Interact(GameObject player)
    {
        GameVariables.Day4OpenDoor = true;
        
        if (grabSound)
            AudioManager.Instance.Play(grabSound);
        
        MessageTipManager.ShowMessage("MAIL DELIVERY SYSTEM: ONLINE");

    }
}
