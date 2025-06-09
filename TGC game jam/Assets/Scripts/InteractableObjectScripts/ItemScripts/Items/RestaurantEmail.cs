using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantEmail : ItemBase
{
    private bool day1Letter = false;
    private bool day2Letter = false;
    private bool day3Letter = false;

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
                if( day2Letter ) return;
                
                BackpackManager.Instance.RetrieveItem("Letter2-1");
                MessageTipManager.ShowMessage("Letter has been delivered");
                GameVariables.Day2EventCount++;
                Debug.Log("Day 2 Event:" + GameVariables.Day2EventCount);

                day2Letter= true;
                break;
            case 3:
                if( day3Letter ) return;
                
                BackpackManager.Instance.RetrieveItem("Letter3-3");
                MessageTipManager.ShowMessage("Letter has been delivered");
                GameVariables.Day3EventCount++;
                day3Letter= true;
                break;
            case 4:
            case 5:
                break;
        }
        
    }

}
