using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BrokenLiftItem : ItemBase
{
    public GameObject lift;

    public override void Interact(GameObject player)
    {
        AudioManager.Instance.Play(grabSound);
        
        if (GameVariables.IsHaveBattery)
        {
            lift.SetActive(true);
            MessageTipManager.ShowMessage("Elevator available.");

            this.gameObject.SetActive(false);
        }
        else
        {
            PlayerDialogue.Instance.LiftNotice();
        }
        
    }
}
