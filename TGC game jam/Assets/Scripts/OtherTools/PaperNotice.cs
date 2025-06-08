using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PaperNotice : MonoBehaviour
{
    public Transform paperTransform;
    
    public GameObject paper;
    
    private bool isNoticed = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isNoticed) return;
        if (GameVariables.Day != 2) return;
        if(!GameVariables.Day2HasTalkToGrandma) return;

        startNotice();
        isNoticed = true;
    }

    private void Start()
    {

        GameVariables.OnDayChanged += OndayChange;
    }

    private void OnDestroy()
    {
        GameVariables.OnDayChanged -= OndayChange;
    }

    private void OndayChange()
    {
        if(GameVariables.Day == 2)
        {
            paper.SetActive(true);
        }
    }

    private async void startNotice()
    {
        await Notice();
    }
    
    private async UniTask Notice()
    {
        PlayerMove.CanPlayerMove = false;
        
        CameraSystem.SetSpecialCameraTarget(paperTransform);
        
        await UniTask.WaitForSeconds(1f);
        
        CameraSystem.SetSpecialCameraTarget(PlayerMove.CurrentPlayer.transform);

        await UniTask.WaitForSeconds(1f);

        PlayerDialogue.Instance.FindPaper();
        
        await UniTask.WaitForSeconds(2f);

        PlayerMove.CanPlayerMove = true;
        
        MessageTipManager.ShowMessage("The task list has been updated.");

        QuestTipManager.Instance.AddTask("Origami", "Objective: Take the scrap paper.");


    }
}
