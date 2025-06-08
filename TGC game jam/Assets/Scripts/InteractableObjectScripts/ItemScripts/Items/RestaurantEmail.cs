using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantEmail : ItemBase
{
    private bool day1Letter = false;
    
    public override void Interact(GameObject player)
    {
        switch( GameVariables.Day)
        {
            case 1:
                if( day1Letter ) return;
                
                BackpackManager.Instance.RetrieveItem("Letter1-3");
                MessageTipManager.ShowMessage("Letter has been delivered");
                GameVariables.Day1LetterSend++;
                day1Letter= true;
                break;
            case 2:
            case 3:
            case 4:
            case 5:
                break;
        }
        
    }

}
