using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorItem : ItemBase
{
    public SoundEffect jobFinishedSound;
    
    public override void Interact(GameObject player)
    {
        AudioManager.Instance.Play(jobFinishedSound);
        
        if (BackpackManager.Instance.HasItem("Key"))
        {
            this.gameObject.SetActive(false);
        }
        else
            MessageTipManager.ShowMessage("Key required.");
    }

}
