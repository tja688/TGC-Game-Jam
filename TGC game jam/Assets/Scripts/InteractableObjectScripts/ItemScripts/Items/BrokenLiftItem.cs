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
        
        if (BackpackManager.Instance.HasItem("LiftBase"))
        {
            lift.SetActive(true);
            MessageTipManager.ShowMessage("Elevator available.");

        }
        else
        {
            PlayerDialogue.Instance.LiftNotice();
        }
        
    }
}
