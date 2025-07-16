using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : ItemBase
{

    public override void Interact(GameObject player)
    {
        base.Interact(player);
        
        MessageTipManager.ShowMessage("Got the post office key.");
        
        
        
    }

    
    
}
