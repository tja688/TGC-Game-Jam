using UnityEngine;
using System.Collections;
using System; // For Action

/// <summary>
/// 使用 UniTask 管理玩家的睡眠与昼夜更替事件流。
/// 【已修改】通过 async/await 确保在屏幕渐隐完成后再开始对话。
/// </summary>
public class SleepCycleManager : MonoBehaviour
{
    
    private void Start()
    {
        EventCenter.AddEventListener(GameEvents.PlayerSleepAndDayChange, OnPlayerSleepAndDayChange);
    }

    /// <summary>
    /// 2. 将事件处理函数修改为 async void，使其可以支持 await 操作。
    /// </summary>
    private void OnPlayerSleepAndDayChange()
    {
        StartCoroutine(TeleportSequenceWithFade());
    }
    


    private IEnumerator TeleportSequenceWithFade()
    {
        // 1. 淡入黑屏并等待完成
        if (ScreenFadeController.Instance)
        {
            var fadeToBlackCoroutine = ScreenFadeController.Instance.BeginFadeToBlack(1f);
            if (fadeToBlackCoroutine != null) yield return fadeToBlackCoroutine; // 等待黑屏协程执行完毕
        }
        
        yield return new WaitForSeconds(1f);

        var fadeToClearCoroutine = ScreenFadeController.Instance.BeginFadeToClear(2f);
        
        if (ScreenFadeController.Instance)
        {
            if (fadeToClearCoroutine != null) yield return fadeToClearCoroutine; // 等待恢复协程执行完毕
        }
        
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(GameEvents.PlayerSleepAndDayChange, OnPlayerSleepAndDayChange);
    }
}