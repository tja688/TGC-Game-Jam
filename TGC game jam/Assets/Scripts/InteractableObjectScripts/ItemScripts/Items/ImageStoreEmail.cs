using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageStoreEmail : ItemBase
{
    private bool isDay2Send = false;
    
    public override void Interact(GameObject player)
    {
        switch( GameVariables.Day)
        {
            case 1:
                PlayerDialogue.Instance.Day1ImagingStoreEmail();
                break;
            case 2:
                if(isDay2Send) return;
                AudioManager.Instance.Play(grabSound);
                BackpackManager.Instance.RetrieveItem("Letter3-1");
                MessageTipManager.ShowMessage("Letter has been delivered");
                GameVariables.Day2EventCount++;
                Debug.Log("Day 2 Event:" + GameVariables.Day2EventCount);

                isDay2Send =  true;
                break;
            case 3:
                MessageTipManager.ShowMessage("The shop door is open—let’s go see the boss");
                break;
            case 4:
            case 5:
                break;
        }
        
    }
}
