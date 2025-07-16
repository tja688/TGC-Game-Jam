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
        DialogueManagerOld.Instance.StartDialogueSequence(ids, dialogueData, promptAnchorTransform, false);
    }

    private void SecondMeet()
    {
        // 演示：如果第二句对话需要在特定物体处显示，并有后续动作
        if (!testItemShowPointTrans)
        {
            Debug.LogError("TestItemShowPointTrans is not set for NPCRobot_Test.SecondMeet");
            // Fallback to default anchor or skip
            List<string> idsFallback = new List<string> { "meet2_fallback" }; // 假设有备用对话
            DialogueManagerOld.Instance.StartDialogueSequence(idsFallback, dialogueData, promptAnchorTransform, false);
            return;
        }

        // 相机移动逻辑（如果需要）
        CameraSystem.OnCameraArrivedAtSpecialTarget += PlayMeet2DialogueAfterCameraMove;
        CameraSystem.SetSpecialCameraTarget(testItemShowPointTrans);
    }

    private void PlayMeet2DialogueAfterCameraMove()
    {
        CameraSystem.OnCameraArrivedAtSpecialTarget -= PlayMeet2DialogueAfterCameraMove;

        List<string> ids = new List<string> { "meet2" };
        // 对话锚点是TestItemShowPointTrans
        // false: 等待交互继续
        // MoveBackCameraAfterDialogue 是整个meet2序列（这里只有一句）完成后执行的回调
        DialogueManagerOld.Instance.StartDialogueSequence(ids, dialogueData, testItemShowPointTrans, false, MoveBackCameraAfterDialogue);
    }

    private void MoveBackCameraAfterDialogue()
    {
        // 这个方法会在 "meet2" 对话（或序列）结束后被调用
        CameraSystem.SetSpecialCameraTarget(null); // 将相机移回
        Debug.Log("SecondMeet dialogue finished, camera moved back.");
    }

    private void ThirdMeet()
    {
        List<string> ids = new List<string> { "meet3", "meet3_extra" }; // 示例：第三次交互播放两句话
        // false: 自动播放这两句话，但整个序列说完后（即meet3_extra说完），会等待下一次交互（如果还有后续交互的话）
        // 这里最后一个参数是 onComplete 回调
        DialogueManagerOld.Instance.StartDialogueSequence(ids, dialogueData, promptAnchorTransform, true, () => {
            Debug.Log("ThirdMeet sequence completed.");
            if(InstantiatedPromptInstance)
                DestroyInteractionPrompt(); // 例如，在第三次对话结束后销毁交互提示
        });
    }
}