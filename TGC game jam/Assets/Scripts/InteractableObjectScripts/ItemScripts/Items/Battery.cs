using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : ItemBase
{
    public override void Interact(GameObject player)
    {
        GameVariables.IsHaveBattery = true;
        
        MessageTipManager.ShowMessage("Time to fix that post office elevator.");

        GameVariables.Day3EventCount++;
        
        base.Interact(player);
    }
}
