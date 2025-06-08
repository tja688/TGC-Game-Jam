using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRella : ItemBase
{
    
    private bool day1Talk = false;
    private bool day2Talk = false;
    
    public override void Interact(GameObject instigator)
    {
        switch (GameVariables.Day)
        {
            case 1:
                if (day1Talk) return;
                PlayerDialogue.Instance.Day1Grandma();
                day1Talk = true;
                break;
            case 2:
                if (day2Talk) return;
                PlayerDialogue.Instance.Day2Grandma();
                BackpackManager.Instance.RetrieveItem("Letter2-3");
                MessageTipManager.ShowMessage("Letter has been delivered");
                GameVariables.Day2HasTalkToGrandma = true;
                GameVariables.Day2EventCount++;
                day2Talk = true;
                break;
            
            
        }
    }

}
