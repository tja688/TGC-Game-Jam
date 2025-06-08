using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBolu : ItemBase
{
    private bool day1Talk = false;
    
    public override void Interact(GameObject instigator)
    {
        Debug.Log("{gameObject.name} day1Talk :" + day1Talk);
        
        switch (GameVariables.Day)
        {
            case 1:
                if (day1Talk) return;
                PlayerDialogue.Instance.Day1Bolu();
                day1Talk = true;
                break;
            
        }
    }

}
