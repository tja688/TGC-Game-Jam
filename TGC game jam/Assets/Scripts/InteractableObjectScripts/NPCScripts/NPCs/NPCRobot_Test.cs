using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NPCRobot_Test : NPCBase
{

    private int interactCount = 0;

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
        }
    }

    private void FirstMeet()
    {
        SendDialogueLine( new Vector2(PromptAnchorScreenPoint.x,PromptAnchorScreenPoint.y), "meet1");
    }

    private void SecondMeet()
    {
        CameraSystem.SetCameraTarget(TestItem.testItem.transform);
        
        // SendDialogueLine( UIUtility.WorldToScreenSpaceOverlayPosition(TestItem.testItem.transform.position), "meet2");
        
        
    }

    void ThirdMeet()
    {
        
    }
}
