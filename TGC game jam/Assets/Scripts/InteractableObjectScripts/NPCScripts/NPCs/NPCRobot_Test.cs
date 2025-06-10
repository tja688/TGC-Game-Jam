using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization; // For List

public class NPCRobot_Test : NPCBase
{
    private int interactCount = 0;
    [FormerlySerializedAs("TestItemShowPointTrans")] [SerializeField] private Transform testItemShowPointTrans; // 示例

    public override void InitiateDialogue()
    {
        interactCount++;
        
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
            default:
                interactCount = 0; // 重置计数器
                break;
        }
    }

    private void FirstMeet()
    {
        var ids = new List<string> { "meet1" };
        DialogueManager.Instance.StartDialogueSequence(ids, dialogueData, promptAnchorTransform, false);
    }

    private void SecondMeet()
    {
        if (!testItemShowPointTrans)
        {
            Debug.LogError("TestItemShowPointTrans is not set for NPCRobot_Test.SecondMeet");
            List<string> idsFallback = new List<string> { "meet2_fallback" };
            DialogueManager.Instance.StartDialogueSequence(idsFallback, dialogueData, promptAnchorTransform, false);
            return;
        }

        CameraSystem.OnCameraArrivedAtSpecialTarget += PlayMeet2DialogueAfterCameraMove;
        CameraSystem.SetSpecialCameraTarget(testItemShowPointTrans);
    }

    private void PlayMeet2DialogueAfterCameraMove()
    {
        CameraSystem.OnCameraArrivedAtSpecialTarget -= PlayMeet2DialogueAfterCameraMove;

        List<string> ids = new List<string> { "meet2" };

        DialogueManager.Instance.StartDialogueSequence(ids, dialogueData, testItemShowPointTrans, false, MoveBackCameraAfterDialogue);
    }

    private void MoveBackCameraAfterDialogue()
    {
        CameraSystem.SetSpecialCameraTarget(null); // 将相机移回
        Debug.Log("SecondMeet dialogue finished, camera moved back.");
    }

    private void ThirdMeet()
    {
        List<string> ids = new List<string> { "meet3", "meet3_extra" }; 

        DialogueManager.Instance.StartDialogueSequence(ids, dialogueData, promptAnchorTransform, true, () => {
            Debug.Log("ThirdMeet sequence completed.");
            if(InstantiatedPromptInstance)
                DestroyInteractionPrompt(); 
        });
    }
}