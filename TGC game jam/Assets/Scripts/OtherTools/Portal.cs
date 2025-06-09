using UnityEngine;
using System.Collections;
using System; // For Action

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Portal : MonoBehaviour
{
    [Header("传送设置")]
    public Transform teleportTargetPoint;

    public static event Action<Transform> OnTeleportationProcessWillStart; // 参数可以是玩家Transform
    public static event Action<Transform> OnPlayerActuallyTeleported;
    public static event Action<Transform> OnTeleportationProcessHasFinished;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        var playerRb = other.attachedRigidbody;
        if (!playerRb)
        {
            Debug.LogWarning(gameObject.name + ": Player Rigidbody2D not found.");
            return;
        }
        if (other.isTrigger) return; // 只响应实体碰撞体
        if (!teleportTargetPoint)
        {
            Debug.LogWarning(gameObject.name + ": Target point not set.");
            return;
        }

        StartCoroutine(TeleportSequenceWithFade(playerRb.transform));
    }

    private IEnumerator TeleportSequenceWithFade(Transform playerTransform)
    {

        OnTeleportationProcessWillStart?.Invoke(playerTransform); // 通知：传送流程即将开始

        // 1. 淡入黑屏并等待完成
        if (ScreenFadeController.Instance)
        {
            var fadeToBlackCoroutine = ScreenFadeController.Instance.BeginFadeToBlack(0.5f);
            if (fadeToBlackCoroutine != null) yield return fadeToBlackCoroutine; // 等待黑屏协程执行完毕
        }
        else
        {
            Debug.LogError(gameObject.name + ": ScreenFadeController.Instance is null! Cannot fade to black.");
        }

        // 2. 执行传送
        playerTransform.position = teleportTargetPoint.position;
        OnPlayerActuallyTeleported?.Invoke(playerTransform); // 通知：玩家已被传送

        yield return new WaitForSeconds(1f);

        var fadeToClearCoroutine = ScreenFadeController.Instance.BeginFadeToClear(2f);
        if (ScreenFadeController.Instance)
        {
            if (fadeToClearCoroutine != null) yield return fadeToClearCoroutine; // 等待恢复协程执行完毕
        }
        else
        {
            Debug.LogError(gameObject.name + ": ScreenFadeController.Instance is null! Cannot fade to clear.");
        }

        OnTeleportationProcessHasFinished?.Invoke(playerTransform); // 通知：整个传送流程（包括效果）已结束

    }

}