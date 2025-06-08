using UnityEngine;

/// <summary>
/// 这个脚本用于处理玩家与传送门的交互触发。
/// 当玩家进入触发区域时，它会根据当前天数给出不同的反馈。
/// </summary>
public class PortalInteractionTrigger : MonoBehaviour
{
    private bool isNoticed = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (isNoticed) return;
        if (GameVariables.Day != 2) return;
        
        PlayerDialogue.Instance.StoreClosed();
        isNoticed = true;
    }
}