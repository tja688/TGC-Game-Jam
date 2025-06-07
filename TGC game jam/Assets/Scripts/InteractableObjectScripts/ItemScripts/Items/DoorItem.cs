using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorItem : ItemBase
{
    public SoundEffect jobFinishedSound;

    public GameObject brokenLift; 
    
    public override void Interact(GameObject player)
    {
        if (BackpackManager.Instance.HasItem("Key"))
        {
            MessageTipManager.ShowMessage("Door Opened.");
            
            FinishJob();
            
            brokenLift.SetActive(true);
            
            this.gameObject.SetActive(false);
        }
        else
            MessageTipManager.ShowMessage("Key required.");
    }
    
    private async void FinishJob()
    {
        await JobTips();
    }

    private async UniTask JobTips()
    {
        await UniTask.WaitForSeconds(0.5f);
        
        MessageTipManager.ShowMessage("Mission Complete: Explore the Post Office.");

        await UniTask.WaitForSeconds(0.2f);

        QuestTipManager.Instance.CompleteTask("ExplorePostOffice");
        
        AudioManager.Instance.Play(jobFinishedSound);

        
        
    }
}
