using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCnewBolu : ItemBase
{
    
    private bool day4Talk = false;

    public override void Interact(GameObject instigator)
    {
        switch (GameVariables.Day)
        {
            case 4:
                if (day4Talk) return;
                PlayerDialogue.Instance.Day4Bolu();
                BackpackManager.Instance.RetrieveItem("Letter4-2");
                MessageTipManager.ShowMessage("Letter has been delivered");
                GameVariables.Day4EventCount++;

                day4Talk = true;
                break;

            
        }
    }
}
