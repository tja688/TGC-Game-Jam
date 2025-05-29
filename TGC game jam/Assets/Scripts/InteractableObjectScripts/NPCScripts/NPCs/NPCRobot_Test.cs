using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI.Core;
using Unity.VisualScripting;
using UnityEngine;

public class NPCRobot_Test : NPCBase
{

    private int interactCount = 0;
    
    private TypewriterCore typewriter;

    protected override void Start()
    {
        base.Start();
        
        typewriter = DialogueManager.Instance.Typewriter;
        
    }
    
    public override void InitiateDialogue()
    {
        interactCount++;
        
        if (Camera1) 
            PromptAnchorScreenPoint = Camera1.WorldToScreenPoint(promptAnchorTransform.position);

        switch (interactCount)
        {
            case 1:
                FirstMeet();
                break;
            case 2:
                SecondMeet();
                break;
            case 3:
                ThirdMeet();
                break;
        }
    }

    private void FirstMeet()
    {
        SendDialogueLine( new Vector2(PromptAnchorScreenPoint.x,PromptAnchorScreenPoint.y), "meet1");
    }

    private void SecondMeet()
    {
        CameraSystem.OnCameraArrivedAtSpecialTarget += PlayMeet2;
        
        CameraSystem.SetSpecialCameraTarget(TestItem.ShowPointTrans);

    }

    private void PlayMeet2()
    {
        CameraSystem.OnCameraArrivedAtSpecialTarget -= PlayMeet2;

        SendDialogueLine( Camera1.WorldToScreenPoint(TestItem.ShowPointTrans.position), "meet2");

        typewriter.onTextDisappeared.AddListener(MoveBackCamera);

    }

    private void MoveBackCamera()
    {
        typewriter.onTextDisappeared.RemoveListener(MoveBackCamera);

        CameraSystem.SetSpecialCameraTarget(null);
    }

    private void ThirdMeet()
    {
        SendDialogueLine( new Vector2(PromptAnchorScreenPoint.x,PromptAnchorScreenPoint.y), "meet3");
        
        DestroyInteractionPrompt();
    }
}
