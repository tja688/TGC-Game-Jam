using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRobot_Test : NPCBase
{
    public override void InitiateDialogue()
    {
        if (Camera1) 
            PromptAnchorScreenPoint = Camera1.WorldToScreenPoint(promptAnchorTransform.position);
        
        SendDialogueLine( new Vector2(PromptAnchorScreenPoint.x,PromptAnchorScreenPoint.y), "meet1");
    }
}
