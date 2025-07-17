using UnityEngine;
using UniRx;
using System;
using PixelCrushers.DialogueSystem;

/// <summary>
/// 使用 UniRx 管理玩家的睡眠与昼夜更替事件流。
/// 【已修改】不再监听对话结束事件，改为固定时间等待。
/// </summary>
public class SleepCycleManager : MonoBehaviour
{
    [Header("配置")]
    [Tooltip("拖拽一个包含睡眠时对话的 Dialogue System Trigger 组件到这里")]
    [SerializeField] private DialogueSystemTrigger sleepDialogueTrigger;

    [Tooltip("从触发睡眠事件到播放对话之间的等待时间（秒）")]
    [SerializeField] private float waitBeforeDialogue = 2f;

    [Tooltip("从播放对话开始，到触发醒来事件之间的等待时间（秒）")]
    [SerializeField] private float waitAfterDialogueStarts = 4f; // 名字稍作修改以更清晰

    private CompositeDisposable disposables = new CompositeDisposable();

    private void Start()
    {
        EventCenter.AddEventListener(GameEvents.PlayerSleepAndDayChange, OnPlayerSleepAndDayChange);
    }

    private void OnPlayerSleepAndDayChange()
    {
        // --- 【核心修改】构建一个更简单的、基于固定时间等待的 UniRx 事件流 ---
        Observable.Return(Unit.Default)
            .Do(_ => EventCenter.TriggerEvent(GameEvents.PlayerSleep)) // a. 触发“玩家入睡”事件
            .Delay(TimeSpan.FromSeconds(waitBeforeDialogue)) // b. 等待第一个2秒
            .Do(_ => PlaySleepDialogue()) // c. 【新】直接触发对话，不等待其结束
            .Delay(TimeSpan.FromSeconds(waitAfterDialogueStarts)) // d. 等待对话4秒
            .Do(_ => EventCenter.TriggerEvent(GameEvents.PlayerWakesUp)) // e. 触发“玩家醒来”事件
            .Subscribe()
            // .OnError(ex => Debug.LogError($"SleepCycleManager stream error: {ex}")) // 可选：添加错误处理
            .AddTo(disposables);
    }

    /// <summary>
    /// 【新】一个简单的同步方法，用于触发对话。
    /// </summary>
    private void PlaySleepDialogue()
    {
        if (sleepDialogueTrigger != null)
        {
            // 直接调用 OnUse() 来开始对话
            sleepDialogueTrigger.OnUse();
        }
        else
        {
            Debug.LogWarning("SleepCycleManager: 未配置睡眠对话，将跳过对话环节。");
        }
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(GameEvents.PlayerSleepAndDayChange, OnPlayerSleepAndDayChange);
        disposables.Dispose();
    }
}