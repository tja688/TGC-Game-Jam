using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RestaurantItem : ItemBase
{
    private bool day1Talk = false;
    private bool day2Talk = false;

    public override void Interact(GameObject instigator)
    {
        switch (GameVariables.Day)
        {
            case 1:
                if (day1Talk) return;
                
                PlayerDialogue.Instance.Day1Restaurant();
                day1Talk = true;
                break;
            
            case 2:
                if (day2Talk) return;
                
                PlayerDialogue.Instance.Day2Restaurant();
                day2Talk = true;
                break;
        }
    }
}
