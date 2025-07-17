using UnityEngine;
using System.Collections;

/// <summary>
/// 【版本一：单例模式】
/// 管理睡眠与昼夜更替的视觉序列。
/// 提供一个公共的单例方法，可被对话系统等外部模块直接调用。
/// </summary>
public class SleepCycleManager : MonoBehaviour
{
    // 1. 添加静态单例实例
    public static SleepCycleManager Instance { get; private set; }

    private void Awake()
    {
        // 2. 实现单例模式的初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // 如果需要在场景切换时不被销毁，可以取消下面这行的注释
            // DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// 3. 创建一个公开的启动方法，供外部调用
    /// </summary>
    public void TriggerSleepSequence()
    {
        // 这个方法会启动整个渐变和等待的协程
        StartCoroutine(SleepAndWakeSequence());
    }
    
    // 4. 原始的协程逻辑保持不变，可以改个更贴切的名字
    private IEnumerator SleepAndWakeSequence()
    {
        Debug.Log("开始睡眠序列：屏幕渐黑...");
        if (ScreenFadeController.Instance)
        {
            // 等待渐变至黑屏完成
            yield return ScreenFadeController.Instance.BeginFadeToBlack(1f);
        }
        
        // 模拟中间的过渡时间
        yield return new WaitForSeconds(1f);

        Debug.Log("...睡醒了：屏幕渐亮。");
        if (ScreenFadeController.Instance)
        {
            // 等待从黑屏恢复完成
            yield return ScreenFadeController.Instance.BeginFadeToClear(2f);
        }
    }
}