using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBolu : ItemBase
{
    private bool day1Talk = false;
    private bool day3Talk = false;

    public override void Interact(GameObject instigator)
    {
        switch (GameVariables.Day)
        {
            case 1:
                if (day1Talk) return;
                PlayerDialogue.Instance.Day1Bolu();
                day1Talk = true;
                break;
            case 3:
                if (day3Talk) return;
                PlayerDialogue.Instance.Day3ToBolu();
                BackpackManager.Instance.RetrieveItem("Letter3-1");
                MessageTipManager.ShowMessage("Letter has been delivered");
                GameVariables.Day3EventCount++;
                day3Talk = true;
                break;
            
        }
    }

}
