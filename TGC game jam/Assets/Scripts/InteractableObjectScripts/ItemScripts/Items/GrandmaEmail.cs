using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandmaEmail : ItemBase
{
    private bool day1Letter = false;
    
    public override void Interact(GameObject player)
    {

        switch( GameVariables.Day)
        {
            case 1:
                if( day1Letter ) return;
                
                BackpackManager.Instance.RetrieveItem("Letter1-1");
                MessageTipManager.ShowMessage("Letter has been delivered");
                GameVariables.Day1LetterSend++;
                day1Letter= true;
                break;
            case 2:
                MessageTipManager.ShowMessage("Try talking to Grandma today");
                break;
            case 3:
                MessageTipManager.ShowMessage("Let’s give Grandma some special gifts");
                break;
            case 4:
            case 5:
                break;
        }
        
    }

}
