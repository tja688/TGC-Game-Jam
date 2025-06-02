using UnityEngine;
using UnityEngine.UI; // 需要引用 UnityEngine.UI 命名空间
using System.Collections;

public class ScreenFadeController : MonoBehaviour
{
    public Image fadeImage; // 将场景中的 ScreenFader Image 拖拽到这里
    public float defaultFadeDuration = 1f; // 默认的渐变时长

    // 可选的：单例模式，方便从其他脚本调用
    public static ScreenFadeController Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 如果需要在场景切换时保持，取消注释此行
        }
        else
        {
            Destroy(gameObject);
        }

        if (!fadeImage) return;
        fadeImage.gameObject.SetActive(true); // 确保 Image 是激活的，即使透明
    }

    /// <summary>
    /// 开始屏幕变黑效果
    /// </summary>
    /// <param name="duration">渐变时长 (秒)</param>
    public Coroutine FadeToBlack(float? duration = null)
    {
        if (fadeImage == null) return null;
        return StartCoroutine(Fade(1f, duration ?? defaultFadeDuration)); // 目标 Alpha 为 1 (不透明)
    }

    /// <summary>
    /// 开始屏幕去黑 (恢复透明) 效果
    /// </summary>
    /// <param name="duration">渐变时长 (秒)</param>
    public Coroutine FadeToClear(float? duration = null)
    {
        return !fadeImage ? null : StartCoroutine(Fade(0f, duration ?? defaultFadeDuration)); // 目标 Alpha 为 0 (透明)
    }

    /// <summary>
    /// 核心的渐变协程
    /// </summary>
    /// <param name="targetAlpha">目标 Alpha 值 (0 到 1)</param>
    /// <param name="duration">渐变时长 (秒)</param>
    private IEnumerator Fade(float targetAlpha, float duration)
    {
        if (!fadeImage) yield break;

        // 如果时长为0或负数，立即设置 Alpha 并结束
        if (duration <= 0f)
        {
            var immediateColor = fadeImage.color;
            immediateColor.a = targetAlpha;
            fadeImage.color = immediateColor;
            yield break;
        }

        var timer = 0f;
        var currentColor = fadeImage.color;
        var startAlpha = currentColor.a;

        // 激活 Image 对象以进行渐变
        fadeImage.gameObject.SetActive(true);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            var newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            currentColor.a = newAlpha;
            fadeImage.color = currentColor;
            yield return null; // 等待下一帧
        }

        // 确保最终 Alpha 值准确设置为目标值
        currentColor.a = targetAlpha;
        fadeImage.color = currentColor;

        // 如果目标是完全透明，可以选择禁用 Image 对象以节省性能 (可选)
        // if (targetAlpha == 0f)
        // {
        //     fadeImage.gameObject.SetActive(false);
        // }
    }
}