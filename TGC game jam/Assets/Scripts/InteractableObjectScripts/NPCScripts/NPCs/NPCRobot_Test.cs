using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRobot_Test : NPCBase
{
    protected override void InitiateDialogue()
    {
        if (Camera.main) 
            PromptAnchorScreenPoint = Camera.main.WorldToScreenPoint(promptAnchorTransform.position);
        
        SendDialogueLine( new Vector2(PromptAnchorScreenPoint.x,PromptAnchorScreenPoint.y), "meet1");
    }
}
